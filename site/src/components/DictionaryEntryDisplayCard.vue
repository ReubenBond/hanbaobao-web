<template>
  <div class="container-sm entry-card my-1 mx-1-md p-1 card">
    <div class="row card-body">
      <div class="col-4 col-sm-2 card-title">
        <div class="m-1">
          <h1 class="">{{ entry.simplified }} </h1>
          <h6 class="text-muted">{{ convertPinyin(entry.pinyin) }}</h6>
        </div>
      </div>
      <div class="col-8 col-sm-10">
        <p>
          {{ entry.definition }}
          <small><router-link class="text-muted" :to="{ name: 'Editor', params: { id: entry.simplified } }">[edit]</router-link></small>
        </p>
        <p v-if="entry.notes"> 
          <small><mark>NOTE</mark>: {{ entry.notes}}</small>
        </p>
        <div>
          <span class="badge m-1" v-if="entry.hskLevel > 0" :class="hskClass(entry.hskLevel)">HSK {{ entry.hskLevel }}</span>
          <span v-for="pos in entry.partOfSpeech" :key="pos" class="badge m-1" :class="posClass(pos)"> {{ pos }} </span>
          <span class="badge m-1 concept-badge" v-if="entry.concept">{{ entry.concept }}</span>
          <span v-if="entry.topic ^ entry.parentTopic">
            <span class="badge m-1 parent-topic-badge" v-if="entry.parentTopic != null">{{ entry.parentTopic }}</span>
            <span class="badge m-1 topic-badge" v-if="entry.topic != null">{{ entry.topic }}</span>
          </span>
          <span class="badge m-1 topic-badge" v-if="entry.topic && entry.parentTopic">{{ entry.parentTopic }} > {{ entry.topic }}</span>
        </div>
      </div>
      <div class="float-right d-inline">
      </div>
    </div>
  </div>
</template>

<script>
import { PinyinConverter } from '../pinyin_converter.js' 

export default {
  name: 'DictionaryEntryDisplayCard',
  props: {
    entry: {}
  },
  data() {
    return {
      count: 0
    }
  },
  methods: {
    convertPinyin(value) {
      return PinyinConverter.convert(value)
    },
    posClass(pos) { 
      switch (pos.toLowerCase()) {
        case 'noun': return 'pos-noun'
        case 'proper noun': return 'pos-propernoun'
        case 'verb': return 'pos-verb'
        case 'adjective': return 'pos-adjective'
        case 'adverb': return 'pos-adverb'
        case 'interjection': return 'pos-interjection'
        case 'suffix': return 'pos-suffix'
        case 'person': return 'pos-person'
        case 'phrase': return 'pos-phrase'
        default:
          return 'pos-misc'
      }
    },
    hskClass(hsk) {
      return 'hsk' + hsk + '-badge';
    }
  }
}
</script>

<style scoped>
.entry-card {
  background: #f7f7d6;
}

.badge {
  color: #fff;
}

.pos-noun {
  background-color: rgb(202, 42, 42);
}

.pos-propernoun {
  background-color: rgb(85, 42, 202);
}

.pos-verb {
  background-color: rgb(39, 50, 204);
}

.pos-adjective {
  background-color: rgb(51, 189, 39);
}

.pos-adverb {
  background-color: rgb(151, 42, 128);
}

.pos-interjection {
  background-color: rgb(255, 90, 24);
}

.pos-suffix {
  background-color: rgb(0, 129, 204);
}

.pos-person {
  background-color: rgb(6, 131, 43);
}

.pos-phrase {
  background-color: rgb(204, 0, 153);
}


.pos-misc {
  background-color: rgb(196, 196, 196);
}

.hsk1-badge {
  background-color: #4a90e2;
}

.hsk2-badge {
  background-color: #4fc7ac;
}

.hsk3-badge {
  background-color: #7abc32;
}

.hsk4-badge {
  background-color: #ffb218;
}

.hsk5-badge {
  background-color: #ff7d25;
}

.hsk6-badge {
  background-color: #d0021b;
}

.concept-badge {
  background-color: #2e7d32;
}

.topic-badge {
  background-color: #e65100;
}

.parent-topic-badge {
  background-color: #ad1457;
}
</style>
