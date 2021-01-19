
import {createStore} from 'vuex'
import axios from 'axios'

export default createStore({
    state: {
        searchQuery: '',
        searchResults: [],
        editingEntry: {},
        isSearching: false,
    },
    mutations: {
        setIsSearching(state, isSearching) {
            state.isSearching = isSearching;
        },
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
            context.commit('setIsSearching', true)
            var results = await axios.get('/api/search?query=' + query);
            context.commit('setSearchResults', results.data);
            context.commit('setIsSearching', false)

        },
        addOrEdit(context, entry) {
            this.commit('setEditing', entry)
        },
        async write(context, entry) {
            var result = await axios.post('/api/entry?entry=' + entry.simplified, entry)
            await this.dispatch('addOrEdit', null)
        },
        async getById(context, id) {
            return (await axios.get('/api/entry?id=' + id)).data;
        }
    },
    getters: {
        searchQuery:  (state, getters) => { return getters.searchQuery },
        searchResults:  (state, getters) => { return getters.searchResults },
        editingEntry: (state, getters) => { return getters.editingEntry }
    }
})