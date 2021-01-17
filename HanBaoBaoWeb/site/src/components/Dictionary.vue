<template>
  <div>
    <SearchBox />
    <div class="container">
      <EntriesContainer :entries="$store.state.searchResults" />
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import EntriesContainer from './EntriesContainer.vue'
import SearchBox from './SearchBox.vue'

export default {
  name: 'Dictionary',
  components: {
    SearchBox,
    EntriesContainer
  },
  methods: {
  },
  computed: {
  }
}
</script>
