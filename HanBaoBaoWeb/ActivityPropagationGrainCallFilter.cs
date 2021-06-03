using Orleans;
using Orleans.Runtime;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HanBaoBaoWeb
{
    // NOTE: See the PR here: https://github.com/dotnet/orleans/pull/6853
    public abstract class ActivityPropagationGrainCallFilter
    {
        protected const string TraceParentHeaderName = "traceparent";
        protected const string TraceStateHeaderName = "tracestate";

        protected const string ActivitySourceName = "orleans.runtime.graincall";
        protected const string ActivityNameIn = "Orleans.Runtime.GrainCall.In";
        protected const string ActivityNameOut = "Orleans.Runtime.GrainCall.Out";

        protected static readonly ActivitySource activitySource = new(ActivitySourceName);

        protected static async Task ProcessNewActivity(IGrainCallContext context, ActivityKind activityKind, ActivityContext activityContext)
        {
            Activity activity = default;
            try
            {
                // Start an activity if this is an application call
                if (context.Grain is { } grain && !grain.GetType().Assembly.GetName().Name.StartsWith("Orleans"))
                {
                    activity = StartActivity(context, activityKind, activityContext);
                }

                if (activity is not null)
                {
                    RequestContext.Set(TraceParentHeaderName, activity.Id);
                }

                // This calls the next filter, and eventually the grain method
                await context.Invoke();
            }
            catch (Exception e)
            {
                if (activity is { IsAllDataRequested: true })
                {
                    // exception attributes from https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/exceptions.md
                    activity.SetTag("exception.type", e.GetType());
                    activity.SetTag("exception.message", e.Message);
                    activity.SetTag("exception.stacktrace", e.StackTrace);
                    activity.SetTag("exception.escaped", true);
                }

                throw;
            }
            finally
            {
                // Activity complete
                activity?.Dispose();
            }
        }

        private static Activity StartActivity(IGrainCallContext context, ActivityKind activityKind, ActivityContext activityContext)
        {
            ActivityTagsCollection tags = null;
            var target = context.Grain.GetPrimaryKey(out var grainIdStr);

            string activityName = $"{context.Grain}/{grainIdStr ?? target.ToString()}/{context.InterfaceMethod?.Name}";
            if (activitySource.HasListeners())
            {
                // rpc attributes from https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/rpc.md
                tags = new ActivityTagsCollection
                        {
                            {"rpc.service", context.InterfaceMethod?.DeclaringType?.ToString()},
                            {"rpc.method", context.InterfaceMethod?.Name},
                            {"net.peer.name", context.Grain?.ToString() + "/" + grainIdStr ?? target.ToString()},
                        };
            }

            var activity = activitySource.StartActivity(activityName, activityKind, activityContext, tags);
            return activity;
        }
    }

    public class ActivityPropagationOutgoingGrainCallFilter : ActivityPropagationGrainCallFilter, IOutgoingGrainCallFilter
    {
        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            if (Activity.Current != null)
            {
                // Copy existing activity to RequestContext
                await ProcessCurrentActivity(context);
            }
            else
            {
                // Create new activity and copy to RequestContext
                await ProcessNewActivity(context, ActivityKind.Client, new ActivityContext());
            }
        }

        private static Task ProcessCurrentActivity(IOutgoingGrainCallContext context)
        {
            var currentActivity = Activity.Current;

            if (currentActivity is not null &&
                currentActivity.IdFormat == ActivityIdFormat.W3C)
            {
                RequestContext.Set(TraceParentHeaderName, currentActivity.Id);
                if (currentActivity.TraceStateString is not null)
                {
                    RequestContext.Set(TraceStateHeaderName, currentActivity.TraceStateString);
                }
            }

            return context.Invoke();
        }
    }

    public class ActivityPropagationIncomingGrainCallFilter : ActivityPropagationGrainCallFilter, IIncomingGrainCallFilter
    {
        public Task Invoke(IIncomingGrainCallContext context)
        {
            // Read trace context from RequestContext
            var traceParent = RequestContext.Get(TraceParentHeaderName) as string;
            var traceState = RequestContext.Get(TraceStateHeaderName) as string;
            var parentContext = new ActivityContext();

            if (traceParent is not null)
            {
                parentContext = ActivityContext.Parse(traceParent, traceState);
            }

            // Start the activity, invoke the request, and stop the activity when done
            return ProcessNewActivity(context, ActivityKind.Server, parentContext);
        }
    }
}
