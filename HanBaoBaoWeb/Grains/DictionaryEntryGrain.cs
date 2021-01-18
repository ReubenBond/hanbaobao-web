using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApp
{
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
            [PersistentState(stateName: "def", storageName: "definitions")] IPersistentState<DictionaryEntryState> defs,
            ReferenceDataService referenceDataService)
        {
            _state = defs;
            _referenceDataService = referenceDataService;
        }

        public override async Task OnActivateAsync()
        {
            // If there is no state saved for this entry yet, load the state from the reference dictionary and store it.
            if (_state.State?.Definition is null)
            {
                var headword = this.GetPrimaryKeyString();
                var result = await _referenceDataService.QueryByHeadwordAsync(headword);

                if (result is { Count: > 0 } && result.FirstOrDefault() is TermDefinition definition)
                {
                    _state.State.Definition = definition;

                    // Store the new state.
                    await _state.WriteStateAsync();
                }
            }
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

        public Task<TermDefinition> GetDefinitionAsync() => Task.FromResult(_state.State.Definition);
    }

    [Serializable]
    internal class DictionaryEntryState
    {
        public TermDefinition Definition { get; set; }
    }
}
