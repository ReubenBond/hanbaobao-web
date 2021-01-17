<template>
  <div class="container">
    <div class="row">
      <div class="col-2 border">
          <h1 class="">{{ entry.simplified }} </h1>
          <h6 class="card-subtitle mb-2 text-muted">{{ entry.pinyin }}</h6>
          <!--<p class="card-text">Some quick example text to build on the card title and make up the bulk of the card's content.</p>-->
      </div>
      <div class="col-10 border">
        <div class="row">
          <div class="col-12">
            {{ entry.definition }}
          </div>
        </div>
        <div class="row">
          <div class="col border">
            {{ entry.partOfSpeech }}
          </div>
          <div class="col border">
            {{ entry.hskLevel }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DictionaryEntry',
  props: {
    entry: {}
  },
  data() {
    return {
      count: 0
    }
  }
}
</script>
