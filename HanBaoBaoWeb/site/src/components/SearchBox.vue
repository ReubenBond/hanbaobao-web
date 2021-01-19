<template>
<!--
  <nav class="navbar navbar-expand-lg navbar-light bg-light">
    <a class="navbar-brand" href="#">HanBaoBao</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>

    <div class="collapse navbar-collapse navbar-ex1-collapse" id="navbarSupportedContent">
      <div class="nav navbar-nav">
        <form class="form-inline my-2 my-lg-0" @submit.prevent="submit">
          <input class="form-control mr-sm-2" type="search" placeholder="Search" aria-label="Search" v-model="editableSearchQuery" @keyup.enter="submit">
          <button class="btn btn-outline-success my-2 my-sm-0" type="submit">Search</button>
        </form>
      </div>
    </div>
  </nav>
  -->
  <div class="container">
    <div class="row">
      <div class="col-12">
        <h1 style="text-align: center" class="m-3">
          HànBǎoBāo<br class="d-md-none">
          <small class="text-muted subtitle"> Chinese-English dictionary</small>
          </h1>
      </div>
    </div>
    <div class="row">
      <div class="col-1"></div>
      <div class="col-10">
        <form class="m-1" @submit.prevent="submit">
          <div class="search-bar-container">
            <input class="form-control mr-sm-2" type="search" placeholder="Search" aria-label="Search" v-model="editableSearchQuery" @keyup.enter="submit">
            <button class="btn btn-outline-success my-2 my-sm-0" type="submit">Search</button>
          </div>
        </form>
      </div>
      <div class="col-1"></div>
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
export default {
  name: 'SearchBox',
  data() {
    return {
      editableSearchQuery: ''
    }
  },
  mounted() {
      const { id } = this.$route.params;
      this.$store.commit('setSearchQuery', id)
      this.$store.dispatch('search', id)
    /*
    this.$nextTick(function () {
      var query = this.$data.editableSearchQuery || this.$store.state.searchQuery || this.$route.params.id
      this.$data.editableSearchQuery = query;
      var state = this.$store.state;
      if (!state.isSearching && query && state.searchResults.length === 0) {
        this.$store.dispatch('search', query)
      }
    })
    */
    console.log("mounted search with " + this.$data.editableSearchQuery)
  },
  watch: {
    '$route.params.id': {
      handler(id) {
        this.$data.editableSearchQuery = id
        console.log("watched and saw " + id)
      },
      deep: true,
      immediate: true
    }
  },
  methods: {
    submit(value) {
      var query = this.$data.editableSearchQuery
      this.$store.commit('setSearchQuery', query)

      if (!query) {
        this.$router.push({name: 'Home' })
      } else {
        this.$store.dispatch('search', query)
        this.$router.push({name: 'Search', params: { id: query }})
      }
    }
  },
}
</script>

<style scoped>
.subtitle {

}

.search-bar-container {
      display: flex;
      flex-direction: row;
      justify-content: center;
  }
</style>
