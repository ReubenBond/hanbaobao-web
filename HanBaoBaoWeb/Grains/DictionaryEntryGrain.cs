using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApp
{
    internal interface IDictionaryEntryGrain : IGrainWithStringKey
    {
        Task<List<TermDefinition>> GetDefinitionsAsync();
    }

    internal class DictionaryEntryGrain : Grain, IDictionaryEntryGrain
    {
        private readonly IPersistentState<DictionaryEntryState> _definitions;

        public DictionaryEntryGrain(
            // Inject some storage. We will use the "definitions" storage provider configured in Program.cs
            // and we will call this piece of state "defs", to distinguish it from any other state we might want to have
            [PersistentState(stateName: "defs", storageName: "definitions")] IPersistentState<DictionaryEntryState> defs)
        {
            _definitions = defs;
        }

        public override async Task OnActivateAsync()
        {
            // If there is no state saved for this entry yet, load the state from the reference dictionary and store it.
            if (_definitions.State?.Definitions is null or { Count: 0 })
            {

                // Store the new state.
                await _definitions.WriteStateAsync();
            }

        }

        public Task<List<TermDefinition>> GetDefinitionsAsync() => Task.FromResult(_definitions.State.Definitions);
    }

    [Serializable]
    internal class DictionaryEntryState
    {
        public List<TermDefinition> Definitions { get; set; }
    }
}
