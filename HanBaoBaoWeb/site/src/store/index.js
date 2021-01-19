
import {createStore} from 'vuex'
import axios from 'axios'
import { onErrorCaptured } from 'vue';

export default createStore({
    state: {
        searchQuery: '',
        searchResults: [],
        editingEntry: {},
        isSearching: false,
        errorText: null,
        errorCount: 0,
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
        },
        setErrorText(state, errorText) {
            state.errorText = errorText
            ++state.errorCount
        }
    },
    actions: {
        async search(context, query) {
            try {
                context.commit('setSearchQuery', query)
                context.commit('setSearchResults', null) 
                context.commit('setIsSearching', true)
                var results = await axios.get('/api/search?query=' + query);
                context.commit('setSearchResults', results.data);
            } catch (error) {
                context.commit('setSearchResults', null);
                context.dispatch('onError', error);
            } finally {
                context.commit('setIsSearching', false)
            }

        },
        addOrEdit(context, entry) {
            this.commit('setEditing', entry)
        },
        async write(context, entry) {
            try {
                var result = await axios.post('/api/entry?entry=' + entry.simplified, entry)
                await this.dispatch('addOrEdit', null)
            } catch (error) {
                context.dispatch('onError', error);
            }
        },
        async getById(context, id) {
            try {
                return (await axios.get('/api/entry?id=' + id)).data;
            } catch (error) {
                context.dispatch('onError', error);
            }
        },
        onError(context, error) {
            console.log(error)

            var errorText = null;
            if (error.response && typeof error.response.data === 'string') {
                errorText = error.response.data
            } else if (error) {
                errorText = error.toString()
            }

            context.commit('setErrorText', errorText)
            var count = context.state.errorCount;
            setTimeout(() => {
                if (context.state.errorCount === count) {
                    context.commit('setErrorText', null);
                }
            }, 10000)
        }
    },
    getters: {
        searchQuery:  (state, getters) => { return getters.searchQuery },
        searchResults:  (state, getters) => { return getters.searchResults },
        editingEntry: (state, getters) => { return getters.editingEntry }
    }
})