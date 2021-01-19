using DictionaryApp;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace HanBaoBaoWeb
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

            // We can implement and anti-abuse system by creating a grain for each user, keyed by the IP.
            // All calls made by the user go through that grain. The grain monitors its own request rate.
            // If some abuse is detected, the grain sets a "banned" flag on its state, and an administrator
            // is notified. The administrator can review the user's behavior and decide to unban them.
#if false
            var clientId = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            var userAgentGrain = _grainFactory.GetGrain<IUserAgentGrain>(clientId);
            var results = await userAgentGrain.GetSearchResultsAsync(query);
#endif

            // Get a grain identified by the query string and ask for its search results
            var searchGrain = _grainFactory.GetGrain<ISearchGrain>(query);
            var results = await searchGrain.GetSearchResultsAsync();
            return Ok(results);
        }
    }
}
