<template>
  <div>
    <div class="my-2 mx-2-sm">
        <div class="container-sm" v-if="!isSearching">
        <EntriesContainer :entries="searchResults" />
        </div>
    </div>
    <div class="m-5" v-if="isSearching">
        <h2 class="text-muted searching-text align-middle">Searching...</h2>
    </div>
    <div class="m-5" v-if="noResults">
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
        return !state.isSearching && state.searchQuery && (!state.searchResults || state.searchResults.length === 0)
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