using HanBaoBao;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HanBaoBao
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public SearchController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetByQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Provided an empty query");
            }

            #region What if we wanted to implement throttling? 
            // We can implement and anti-abuse system by creating a grain for each user, keyed by the IP.
            // All calls made by the user go through that grain. The grain monitors its own request rate.
            // If some abuse is detected, the grain sets a "banned" flag on its state, and an administrator
            // is notified. The administrator can review the user's behavior and decide to unban them.
            var clientId = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            var userAgentGrain = _grainFactory.GetGrain<IUserAgentGrain>(clientId);
            try
            {
                var results = await userAgentGrain.GetSearchResultsAsync(query);
                return Ok(results);
            }
            catch (ThrottlingException exc)
            {
                return this.StatusCode(429, exc.Message);
            }
            #endregion
            /*
            // Get a grain identified by the query string and ask for its search results
            var searchGrain = _grainFactory.GetGrain<ISearchGrain>(query);
            var results = await searchGrain.GetSearchResultsAsync();
            return Ok(results);
            */
        }
    }
    
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
