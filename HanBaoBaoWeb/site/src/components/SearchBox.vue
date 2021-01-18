<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light">
    <a class="navbar-brand" href="#">HanBaoBao</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>

    <div class="collapse navbar-collapse" id="navbarSupportedContent">
      <form class="form-inline my-2 my-lg-0" @submit.prevent="submit">
        <input class="form-control mr-sm-2" type="search" placeholder="Search" aria-label="Search" v-model="internalQueryText" @keyup.enter="submit">
        <button class="btn btn-outline-success my-2 my-sm-0" type="submit">Search</button>
      </form>
    </div>
  </nav>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
export default {
  name: 'SearchBox',
  data() {
    return {
      internalQueryText: ''
    }
  },
  computed: {
    searchQuery() {
      return this.$store.state.searchQuery
    }
  },
  methods: {
    submit(value) {
      this.$store.dispatch('search', this.$data.internalQueryText)
    }
  }
}
</script>
