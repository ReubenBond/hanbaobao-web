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

            // Get a grain identified by the query string and ask for its search results
            var searchGrain = _grainFactory.GetGrain<ISearchGrain>(query);
            var results = await searchGrain.GetSearchResultsAsync();
            return Ok(results);
        }
    }
}
