using DictionaryApp;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HanBaoBaoWeb
{
    internal interface ISearchGrain : IGrainWithStringKey
    {
        Task<List<TermDefinition>> GetSearchResultsAsync();
    }

    internal class SearchGrain : Grain, ISearchGrain
    {
        private readonly ReferenceDataService _searchDatabase;
        private readonly IGrainFactory _grainFactory;
        private List<TermDefinition> _cachedResult;
        private Stopwatch _timeSinceLastUpdate = new Stopwatch();

        public SearchGrain(
            ReferenceDataService searchDatabase,
            IGrainFactory grainFactory)
        {
            _searchDatabase = searchDatabase;
            _grainFactory = grainFactory;
        }

        public async Task<List<TermDefinition>> GetSearchResultsAsync()
        {
            // If the query has already been performed, return the result from cache.
            if (_cachedResult is object && _timeSinceLastUpdate.Elapsed < TimeSpan.FromSeconds(10))
            {
                return _cachedResult;
            }

            // This grain is keyed on the search query, so use that to search
            var query = this.GetPrimaryKeyString();

            // Search for possible matches from the full-text-search database
            var headwords = await _searchDatabase.QueryHeadwordsByAnyAsync(query);

            // Fan out and get all of the definitions for each matching headword
            var tasks = new List<Task<TermDefinition>>();
            foreach (var headword in headwords.Take(25))
            {
                var entryGrain = _grainFactory.GetGrain<IDictionaryEntryGrain>(headword);
                tasks.Add(entryGrain.GetDefinitionAsync());
            }

            // Wait for all calls to complete
            await Task.WhenAll(tasks);

            // Collect the results into a list to return
            var results = new List<TermDefinition>(tasks.Count);
            foreach (var task in tasks)
            {
                results.Add(await task);
            }

            // Cache the result for next time
            _cachedResult = results;
            _timeSinceLastUpdate.Restart();

            return results;
        }
    }
}
