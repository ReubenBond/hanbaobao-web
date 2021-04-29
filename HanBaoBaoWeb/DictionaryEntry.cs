using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using Orleans.Runtime;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HanBaoBao
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

    [Serializable]
    public class TermDefinition
    {
        public long Id { get; set; }
        public string Simplified { get; set; }
        public string Traditional { get; set; }
        public string Pinyin { get; set; }
        public string Definition { get; set; }
        public string Classifier { get; set; }
        public string Concept { get; set; }
        public int HskLevel { get; set; }
        public string Topic { get; set; }
        public string ParentTopic { get; set; }
        public string Notes { get; set; }
        public double Frequency { get; set; }
        public List<string> PartOfSpeech { get; set; }
    }

    internal interface IDictionaryEntryGrain : IGrainWithStringKey
    {
        Task<TermDefinition> GetDefinitionAsync();
        Task UpdateDefinitionAsync(TermDefinition value);
    }

    internal class DictionaryEntryGrain : Grain, IDictionaryEntryGrain
    {
        private readonly IPersistentState<DictionaryEntryState> _state;
        private readonly ReferenceDataService _referenceDataService;

        public DictionaryEntryGrain(
            // Inject some storage. We will use the "definitions" storage provider configured in Program.cs
            // and we will call this piece of state "def", to distinguish it from any other state we might want to have
            [PersistentState(stateName: "def", storageName: "definitions")]
                IPersistentState<DictionaryEntryState> defs,
            ReferenceDataService referenceDataService)
        {
            _state = defs;
            _referenceDataService = referenceDataService;
        }

        public override Task OnActivateAsync()
        {
            // If there is no state saved for this entry yet, load the state from the reference dictionary and store it.
            if (_state.State?.Definition is null)
            {
                // Find the definiton from the reference data, using this grain's id to look it up
                var headword = this.GetPrimaryKeyString();
                var result = _referenceDataService.QueryByHeadwordAsync(headword);

                if (result is { Count: > 0 } && result.FirstOrDefault() is TermDefinition definition)
                {
                    _state.State.Definition = definition;

                    // Write the state but don't wait for completion. If it fails, we will write it next time. 
                    _state.WriteStateAsync().Ignore();
                }
            }

            return Task.CompletedTask;
        }

        public async Task UpdateDefinitionAsync(TermDefinition value)
        {
            if (!string.Equals(value.Simplified, this.GetPrimaryKeyString(), StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Cannot change the headword for a definition");
            }

            _state.State.Definition = value;
            await _state.WriteStateAsync();
        }

        public Task<TermDefinition> GetDefinitionAsync()
            => Task.FromResult(_state.State.Definition);
    }

    [Serializable]
    internal class DictionaryEntryState
    {
        public TermDefinition Definition { get; set; }
    }
}
