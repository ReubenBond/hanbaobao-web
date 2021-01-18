<template>
  <div class="container">
    <div class="row">
      <p>Search query {{ $store.state.searchQuery }} </p>
    </div>
    <div class="row">
      <div class="col-12">
        <button class="btn btn-primary">New definition</button>
      </div>
    </div>
    <div class="row">
      <DictionaryEntryEditor :entry="$store.state.editingEntry" />
    </div>
    <div class="row" v-for="entry in entries" :key="entry.id">
        <DictionaryEntryDisplay :entry="entry" v-show="!entry.editing" />
    </div>
  </div>
</template>

<script>
import DictionaryEntryDisplay from './DictionaryEntryDisplay.vue'
import DictionaryEntryEditor from './DictionaryEntryEditor.vue'

export default {
  name: 'EntriesContainer',
  components: {
    DictionaryEntryDisplay,
    DictionaryEntryEditor
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
      this.$state.actions.addOrEdit({ id: -1});
    }
  }
}
</script>
