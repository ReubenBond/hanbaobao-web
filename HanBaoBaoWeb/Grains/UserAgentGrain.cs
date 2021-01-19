using HanBaoBaoWeb;
using Orleans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DictionaryApp
{
    internal interface IUserAgentGrain : IGrainWithStringKey
    {
        Task<List<TermDefinition>> GetSearchResultsAsync(string query);
        Task<TermDefinition> GetDefinitionAsync(string id);
        Task UpdateDefinitionAsync(string id, TermDefinition value);
    }

    /// <summary>
    /// This grain demonstrates a simple way to throttle a given user (identified by some key, in this case, their IP address is the primary key of the grain)
    /// It uses a call filter to maintain a count of recent calls and throttles if they exceed a score of 20. The score decays by 1 every 5 seconds.
    /// </summary>
    internal class UserAgentGrain : Grain, IUserAgentGrain, IIncomingGrainCallFilter
    {
        private const int DecayPeriod = 5;
        private const int ThrottleThreshold = 20;
        private int _callCount;
        private Stopwatch _timeSinceLastCall = new Stopwatch();
        private readonly IGrainFactory _grainFactory;
        public UserAgentGrain(IGrainFactory grainFactory) => _grainFactory = grainFactory;

        public Task<TermDefinition> GetDefinitionAsync(string id) => _grainFactory.GetGrain<IDictionaryEntryGrain>(id).GetDefinitionAsync();
        public Task<List<TermDefinition>> GetSearchResultsAsync(string query) => _grainFactory.GetGrain<ISearchGrain>(query).GetSearchResultsAsync();
        public Task UpdateDefinitionAsync(string id, TermDefinition value) => _grainFactory.GetGrain<IDictionaryEntryGrain>(id).UpdateDefinitionAsync(value);

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            if (_timeSinceLastCall.Elapsed > TimeSpan.FromSeconds(DecayPeriod) && _callCount > 0)
            {
                // Allow the score to decay every DecayPeriod
                _callCount = Math.Max(0, (int)(_callCount - _timeSinceLastCall.Elapsed.TotalSeconds / DecayPeriod));
            }

            if (_callCount == 0)
            {
                _timeSinceLastCall.Restart();
            }

            ++_callCount;

            if (_callCount > ThrottleThreshold)
            {
                var remainingSeconds = (_callCount - ThrottleThreshold) * DecayPeriod;
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
