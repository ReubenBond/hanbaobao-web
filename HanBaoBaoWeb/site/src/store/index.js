
import {createStore} from 'vuex'
import axios from 'axios'

export default createStore({
    state: {
        searchQuery: '你好',
        searchResults: [],
        editingEntry: {}
    },
    mutations: {
        setSearchQuery(state, query) {
            state.searchQuery = query
        },
        setSearchResults(state, results) {
            state.searchResults = results;
        },
        setEditing(state, entry) {
            if (state.editingEntry) {
                state.editingEntry.editing = false;
                state.editingEntry = null;
                // should commit or abort the existing edit here
            }

            if (entry) {
                entry.editing = true
            }

            state.editingEntry = entry 
        }
    },
    actions: {
        async search(context, query) {
            context.commit('setSearchQuery', query)
            context.commit('setSearchResults', [])
            var results = await axios.get('/api/search?query=' + query);
            context.commit('setSearchResults', results.data);

        },
        addOrEdit(context, entry) {
            this.commit('setEditing', entry)
        },
        async write(context, entry) {
            var result = await axios.post('/api/entry?entry=' + entry.simplified, entry)
            await this.dispatch('addOrEdit', null)
        }
    },
    getters: {
        searchQuery:  (state, getters) => { return getters.searchQuery },
        searchResults:  (state, getters) => { return getters.searchResults },
        editingEntry: (state, getters) => { return getters.editingEntry }
    }
})