<template>
  <div class="container entry-card-editing m-1 card">
    <form class="p-1 m-1 card-body">
      <h4 class="card-title">Edit entry</h4>
      <div class="form-row">
        <div class="col">
          <div class="form-group">
            <label for="definition">Simplified</label>
            <input type="text" class="form-control" :value="entry.simplified" required>
            <small class="form-text text-muted">
              The <em>Simplified Chinese</em> 汉字 representation of this term.
            </small>
          </div>
        </div>
        <div class="col">
          <div class="form-group">
            <label for="definition">Traditional</label>
            <input type="text" class="form-control" :value="entry.traditional">
            <small class="form-text text-muted">
              The <em>Traditional Chinese<em> 漢字 representation of this term.
            </small>
          </div>
        </div>
        <div class="col">
          <div class="form-group">
            <label for="definition">Pinyin</label>
            <input type="text" class="form-control" :value="entry.pinyin">
            <small class="form-text text-muted">
            </small>
            <small class="form-text text-muted mr-1" style="text-align: right">
              {{ convertPinyin(entry.pinyin) }}<br>
            </small>
            <small class="form-text text-muted">
              Pinyin with tones, such as "ni3 hao3".
            </small>
          </div>
        </div>
        <div class="col">
          <div class="form-group">
            <label for="definition">HSK Level</label>
            <input type="text" class="form-control" :value="entry.hskLevel">
            <small class="form-text text-muted">
              The HSK level in which this term is introduced. Leave empty if the term is not included in the HSK.
            </small>
          </div>
        </div>
      </div>
      <div class="form-group">
        <label for="definition">Definition</label>
        <textarea class="form-control" lines="3" :value="entry.definition"></textarea>
      </div>
      <div class="form-group">
        <label for="notes">Notes</label>
        <input type="text" class="form-control" :value="entry.notes">
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
          <button type="submit" class="btn btn-secondary m-2" >Cancel</button>
          <button type="submit" class="btn btn-primary m-2" >Save</button>
      </div>
    </form>
  </div>
</template>

<script>
import { PinyinConverter } from '../pinyin_converter.js' 

export default {
  name: 'DictionaryEntryEditor',
  props: {
    entry: {}
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
    }
  }
}
</script>

<style scoped>
</style>
