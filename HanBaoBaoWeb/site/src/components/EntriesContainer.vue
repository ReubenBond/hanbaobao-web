<template>
  <div class="container">
    <div class="row">
      <p>Search query {{ $store.state.searchQuery }} </p>
    </div>
    <div class="row" v-for="entry in entries" :key="entry.id">
        <DictionaryEntry :entry="entry" />
    </div>
  </div>
</template>

<script>
import DictionaryEntry from './DictionaryEntry.vue'

export default {
  name: 'EntriesContainer',
  components: {
    DictionaryEntry
  },
  props: {
    entries: {} 
  },
  data() {
    return {
      count: 0
    }
  }
}
</script>
