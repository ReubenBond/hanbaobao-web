<template>
  <div class="container">
    <div class="row" v-for="entry in entries" :key="entry.id">
        <DictionaryEntryDisplayCard :entry="entry" v-show="!entry.editing" />
    </div>
  </div>
</template>

<script>
import DictionaryEntryDisplayCard from './DictionaryEntryDisplayCard.vue'

export default {
  name: 'EntriesContainer',
  components: {
    DictionaryEntryDisplayCard
  },
  props: {
    entries: {} 
  },
  data() {
    return {
    }
  },
  methods: {
    startNewEntry() {
      this.$store.dispatch('addOrEdit', { id: -1, editing: true });
    }
  }
}
</script>
