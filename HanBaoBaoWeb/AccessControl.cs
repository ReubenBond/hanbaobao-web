using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Linq;
using System.Threading.Tasks;

namespace HanBaoBao
{
    internal interface IUserAgentGrain : IGrainWithStringKey
    {
        Task<List<TermDefinition>> GetSearchResultsAsync(string query);
        Task<TermDefinition> GetDefinitionAsync(string id);
        Task UpdateDefinitionAsync(string id, TermDefinition value);
    }

    /// <summary>
    /// This grain demonstrates a simple way to throttle a given client (identified by their IP address, which is used as the primary key of the grain)
    /// It uses a call filter to maintain a count of recent calls and throttles if they exceed a defined threshold. The score decays over time until, allowing
    /// the client to resume making calls.
    /// </summary>
    internal class UserAgentGrain : Grain, IUserAgentGrain, IIncomingGrainCallFilter
    {
        private const int ThrottleThreshold = 3;
        private const int DecayPeriod = 5;
        private const double DecayRate = (double)ThrottleThreshold / (double)DecayPeriod;
        private double _throttleScore;
        private Stopwatch _stopwatch = new Stopwatch();
        private readonly IGrainFactory _grainFactory;
        private readonly ILogger<UserAgentGrain> _logger;

        public UserAgentGrain(IGrainFactory grainFactory, ILogger<UserAgentGrain> logger)
        {
            _grainFactory = grainFactory;
            _logger = logger;
        }

        public Task<TermDefinition> GetDefinitionAsync(string id) => _grainFactory.GetGrain<IDictionaryEntryGrain>(id).GetDefinitionAsync();
        public Task<List<TermDefinition>> GetSearchResultsAsync(string query) => _grainFactory.GetGrain<ISearchGrain>(query).GetSearchResultsAsync();
        public Task UpdateDefinitionAsync(string id, TermDefinition value) => _grainFactory.GetGrain<IDictionaryEntryGrain>(id).UpdateDefinitionAsync(value);

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            if (context.Arguments?.FirstOrDefault() is string phrase && string.Equals("please", phrase))
            {
                _throttleScore = 0;
            }

            // Work out how long it's been since the last call
            var elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();

            // Calculate a new score based on a constant rate of score decay and the time which elapsed since the last call.
            _throttleScore = Math.Max(0, _throttleScore - elapsedSeconds * DecayRate) + 1;

            // If the user has exceeded the threshold, deny their request and give them a helpful warning.
            if (_throttleScore > ThrottleThreshold)
            {
                var remainingSeconds = Math.Max(0, (int)Math.Ceiling((_throttleScore - (ThrottleThreshold - 1)) / DecayRate));
                _logger.LogError("Throttling");
                throw new ThrottlingException($"Request rate exceeded, wait {remainingSeconds}s before retrying"); 
            }

            await context.Invoke();
        }
    }

    [Serializable]
    public class ThrottlingException : Exception
    {
        public ThrottlingException(string message) : base(message) { }
        public ThrottlingException(string message, Exception innerException) : base(message, innerException) { }
        protected ThrottlingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
