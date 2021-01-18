<template>
  <div class="container entry-card-editing m-1 card">
    <form class="p-1 m-1 card-body">
      <h4 class="card-title">Edit entry</h4>
      <div class="form-row">
        <div class="col-3">
          <div class="form-group">
            <label for="definition">Simplified</label>
            <input type="text" class="form-control" v-model="entry.simplified" required>
            <small class="form-text text-muted">
              The <em>Simplified Chinese</em> 汉字 representation of this term.
            </small>
          </div>
        </div>
        <div class="col-3">
          <div class="form-group">
            <label for="definition">Traditional</label>
            <input type="text" class="form-control" v-model="entry.traditional">
            <small class="form-text text-muted">
              The <em>Traditional Chinese<em> 漢字 representation of this term.
            </small>
          </div>
        </div>
        <div class="col-5">
          <div class="form-group">
            <label for="definition">Pinyin</label>
            <div class="input-group">
              <input type="text" class="form-control" v-model="entry.pinyin">
              <input type="text" class="form-control" :value="pinyinWithTones" disabled>
            </div>
            <small class="form-text text-muted">
              Pinyin with tones, such as "ni3 hao3".
            </small>
          </div>
        </div>
        <div class="col-1">
          <div class="form-group">
            <label for="hskLevel">HSK Level</label>
            <select class="form-control" id="hskLevel" v-model="entry.hskLevel">
              <option value="0">N/A</option>
              <option>1</option>
              <option>2</option>
              <option>3</option>
              <option>4</option>
              <option>5</option>
              <option>6</option>
            </select>
          </div>
        </div>
      </div>
      <div class="form-group">
        <label for="definition">Definition</label>
        <textarea class="form-control" lines="3" v-model="entry.definition"></textarea>
      </div>
      <div class="form-group">
        <label for="notes">Notes</label>
        <input type="text" class="form-control" v-model="entry.notes">
      </div>
      <div class="form-group">
        <label>Part of speech</label><br>
        <div>
          <div class="form-check form-check-inline" v-for="pos in allPoS" :key="pos">
            <input class="form-check-input" type="checkbox" :value="pos" :id="pos" v-model="entry.partOfSpeech">
            <label class="form-check-label" :for="pos">{{ titleCase(pos) }}</label>
          </div>
        </div>
        <small>Select all that apply.</small>
      </div>
      <div class="form-row justify-content-center">
          <button type="submit" class="btn btn-secondary m-2" @click="cancel">Cancel</button>
          <button type="submit" class="btn btn-primary m-2" @click="save">Save</button>
      </div>
    </form>
  </div>
</template>

<script>
import { PinyinConverter } from '../pinyin_converter.js' 

export default {
  name: 'DictionaryEntryEditor',
  props: {
    entry: { pinyin: 'hao3'}
  },
  data() {
    return {
      count: 0,
      allPoS: [
        "ADDRESS", "ADJECTIVE", "ADVERB", "AUXILIARY VERB", "BOUND MORPHEME", "SET PHRASE", "CITY", "COMPLEMENT",
        "CONJUNCTION", "COUNTRY", "DATE", "DETERMINER", "DIRECTIONAL", "EXPRESSION", "FOREIGN TERM", "GEOGRAPHY",
        "IDIOM", "INTERJECTION", "MEASURE WORD", "MEASUREMENT", "NAME", "NOUN", "NUMBER", "NUMERAL", "ONOMATOPOEIA",
        "ORDINAL", "ORGANIZATION", "PARTICLE", "PERSON", "PHONETIC", "PHRASE", "PLACE", "PREFIX", "PREPOSITION",
        "PRONOUN", "PROPER NOUN", "QUANTITY", "RADICAL", "SUFFIX", "TEMPORAL", "TIME", "VERB"
      ]
    }
  },
  methods: {
    convertPinyin(value) {
      if (!value) return undefined;
      return PinyinConverter.convert(value)
    },
    titleCase(str) {
      var splitStr = str.toLowerCase().split(' ');
      for (var i = 0; i < splitStr.length; i++) {
          splitStr[i] = splitStr[i].charAt(0).toUpperCase() + splitStr[i].substring(1);     
      }

      return splitStr.join(' '); 
    },
    save() {
      //  console.log(this.entry)
      this.$store.dispatch('write', this.entry);
    }
  },
  computed: {
    pinyinWithTones() {
      console.log(this)
      var value = this.entry.pinyin
      if (!value) return undefined;
      return PinyinConverter.convert(value)
    }
  }
}
</script>

<style scoped>
</style>
