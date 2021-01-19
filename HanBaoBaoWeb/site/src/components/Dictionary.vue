<template>
  <div>
    <div class="m-2">
        <div class="container" v-if="!isSearching">
        <EntriesContainer :entries="searchResults" />
        </div>
    </div>
    <div class="w-100 h-100 m-5" v-if="isSearching">
        <h2 class="text-muted searching-text align-middle">Searching...</h2>
    </div>
    <div class="w-100 h-100 m-5" v-if="noResults">
        <h2 class="text-muted searching-text align-middle">No results found</h2>
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import EntriesContainer from './EntriesContainer.vue'

export default {
  name: 'Dictionary',
  components: {
    EntriesContainer
  },
  computed: {
      searchResults() {
        return this.$store.state.searchResults
      },
      isSearching() {
        return this.$store.state.isSearching
      },
      noResults() {
        var state = this.$store.state;
        return !state.isSearching && state.searchQuery && state.searchResults !== null && state.searchResults.length === 0;
      }
  }
}
</script>


<style scoped>
.searching-text {
    text-align: center;
    justify-self: center;
}
</style>