<template>
  <div class="container-sm">
    <div class="row">
      <div class="col-12">
        <h1 style="text-align: center" class="m-3 banner">
          <img class="logo" src="../assets/logo.png"/>
          <span class="banner">&nbsp;HànBǎoBāo<br class="d-md-none">
            <small class="text-muted subtitle"> Chinese-English dictionary</small>
          </span>
          </h1>
      </div>
    </div>
    <div class="row">
      <div class="col-1 d-md-block d-none"></div>
      <div class="col-1 d-lg-block d-none"></div>
      <div class="col-12 col-md-10 col-lg-8">
        <form class="m-y-1" @submit.prevent="submit">
          <div class="search-bar-container input-group">
            <input class="form-control" type="search" placeholder="Search" aria-label="Search" v-model="editableSearchQuery" @keyup.enter="submit">
            <div class="input-group-append">
              <button class="btn btn-primary" type="submit">Search</button>
            </div>
          </div>
        </form>
      </div>
      <div class="col-1 d-md-block d-none"></div>
      <div class="col-1 d-lg-block d-none"></div>
    </div>
    <div class="row" v-if="hasError">
      <div class="col">
        <div class="alert alert-danger m-1" role="alert">
          {{ $store.state.errorText }}
        </div>
      </div>
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
    this.onRouteUpdated()
  },
  watch: {
    '$route.params.id': {
      handler(id) {
        this.onRouteUpdated()
      },
      deep: true,
      immediate: true
    }
  },
  computed: {
    hasError() {
      return this.$store.state.errorText !== null;
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
    },
    onRouteUpdated() {
      if (this.$route.name !== 'Editor') {
        const { id } = this.$route.params;
        this.$data.editableSearchQuery = id
        this.$store.commit('setSearchQuery', id)
        this.$store.dispatch('search', id)
      }
    }
  },
}
</script>

<style scoped>
.logo {
  width:48px;
  vertical-align: bottom;
}
.banner {
  text-align: center;
  vertical-align: middle;
}
.search-bar-container {
      display: flex;
      flex-direction: row;
      justify-content: center;
  }
</style>
