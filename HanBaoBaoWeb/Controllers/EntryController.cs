using DictionaryApp;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace HanBaoBaoWeb
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryController : ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public EntryController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Provided an invalid id");
            }

            var entryGrain = _grainFactory.GetGrain<IDictionaryEntryGrain>(id);
            var result = await entryGrain.GetDefinitionAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TermDefinition entry)
        {
            if (string.IsNullOrWhiteSpace(entry?.Simplified))
            {
                return BadRequest("Provided an invalid entry");
            }

            var entryGrain = _grainFactory.GetGrain<IDictionaryEntryGrain>(entry.Simplified);
            await entryGrain.UpdateDefinitionAsync(entry);
            return Ok();
        }
    }
}
