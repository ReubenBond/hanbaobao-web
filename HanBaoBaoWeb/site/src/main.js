import { createApp } from 'vue'
import { createStore } from 'vuex'
import App from './App.vue'
import './index.css'

const sampleData = 
[
  {
    "id": 28693,
    "simplified": "好",
    "traditional": "好",
    "pinyin": "hao3",
    "definition": "Good, well, proper, good to, easy to, very, so, (suffix indicating completion or readiness), (of two people) close, on intimate terms, (after a personal pronoun) hello",
    "classifier": null,
    "concept": null,
    "hskLevel": 1,
    "topic": null,
    "parentTopic": null,
    "notes": null,
    "frequency": -2.4322826595601517,
    "partOfSpeech": [
      "ADJECTIVE",
      "ADVERB",
      "INTERJECTION",
      "NOUN",
      "SUFFIX",
      "VERB"
    ]
  },
  {
    "id": 7197,
    "simplified": "你好",
    "traditional": "你好",
    "pinyin": "ni3 hao3",
    "definition": "Hello, hi",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": "Social Interaction",
    "notes": "The tone of the first syllable is changed from third to second, in accordance with tone sandhi 变调, because the second syllable is also third tone (Sun 2006, loc. 551).",
    "frequency": -3.723628652730721,
    "partOfSpeech": [
      "NOUN",
      "PERSON",
      "PHRASE"
    ]
  },
  {
    "id": 21450,
    "simplified": "喂",
    "traditional": "喂",
    "pinyin": "wei2",
    "definition": "Hello (when answering the phone)",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": "Interjection",
    "notes": "(Unihan '喂')",
    "frequency": -3.797508592357887,
    "partOfSpeech": [
      "INTERJECTION",
      "PHRASE",
      "VERB"
    ]
  },
  {
    "id": 44261,
    "simplified": "招呼",
    "traditional": "招呼",
    "pinyin": "zhao1 hu5",
    "definition": "To call out to, to greet, to say hello to, to inform, to take care of, to take care that one does not",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Literary Chinese",
    "parentTopic": "Social Interaction",
    "notes": "In the sense of 通知 (Guoyu '招呼' zhāohū 4; Mathews 1931 '招呼', p. 27)",
    "frequency": -4.906443006661986,
    "partOfSpeech": [
      "VERB"
    ]
  },
  {
    "id": 40154,
    "simplified": "您好",
    "traditional": "您好",
    "pinyin": "nin2 hao3",
    "definition": "Hello (polite)",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": "Social Interaction",
    "notes": "(CC-CEDICT '您好')",
    "frequency": -4.909437579213896,
    "partOfSpeech": [
      "NOUN"
    ]
  },
  {
    "id": 20723,
    "simplified": "哈罗",
    "traditional": "哈羅",
    "pinyin": "ha1 luo2",
    "definition": "Hello (loanword)",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": null,
    "notes": "Loanword (CC-CEDICT '哈羅')",
    "frequency": -5.150020216785503,
    "partOfSpeech": [
      "NOUN"
    ]
  },
  {
    "id": 20672,
    "simplified": "哈啰",
    "traditional": "哈囉",
    "pinyin": "ha1 luo1",
    "definition": "Hello (loanword)",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": null,
    "notes": "Loanword (CC-CEDICT '哈囉')",
    "frequency": -5.254438775984993,
    "partOfSpeech": [
      "NOUN"
    ]
  },
  {
    "id": 21292,
    "simplified": "问好",
    "traditional": "問好",
    "pinyin": "wen4 hao3",
    "definition": "To say hello to, to send one's regards to",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": "Social Interaction",
    "notes": "(CC-CEDICT '問好'; Guoyu '問好')",
    "frequency": -5.362528940147715,
    "partOfSpeech": [
      "VERB"
    ]
  },
  {
    "id": 16004,
    "simplified": "午安",
    "traditional": "午安",
    "pinyin": "wu3 an1",
    "definition": "Good afternoon!, Hello (daytime greeting)",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": null,
    "notes": "(CC-CEDICT '午安')",
    "frequency": -5.671448895670608,
    "partOfSpeech": [
      "INTERJECTION"
    ]
  },
  {
    "id": 20671,
    "simplified": "哈喽",
    "traditional": "哈嘍",
    "pinyin": "ha1 lou2",
    "definition": "Hello (loanword)",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": "Social Interaction",
    "notes": "Informal and used in jest",
    "frequency": -5.952670497981036,
    "partOfSpeech": [
      "INTERJECTION"
    ]
  },
  {
    "id": 221571,
    "simplified": "你们好",
    "traditional": "你們好",
    "pinyin": "ni3 men5 hao3",
    "definition": "Hello",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": null,
    "parentTopic": null,
    "notes": null,
    "frequency": -6.953593547174215,
    "partOfSpeech": [
      "PHRASE"
    ]
  },
  {
    "id": 12413,
    "simplified": "凯蒂猫",
    "traditional": "凱蒂貓",
    "pinyin": "Kai3 di4 Mao1",
    "definition": "Hello Kitty",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": "Modern Chinese",
    "parentTopic": null,
    "notes": "(CC-CEDICT '凱蒂貓')",
    "frequency": 0.0,
    "partOfSpeech": [
      "PROPER NOUN"
    ]
  },
  {
    "id": 184757,
    "simplified": "大家好",
    "traditional": "大家好",
    "pinyin": "da4 jia1 hao3 ",
    "definition": "Hello everyone",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": null,
    "parentTopic": null,
    "notes": null,
    "frequency": 0.0,
    "partOfSpeech": [
      "PHRASE"
    ]
  },
  {
    "id": 209761,
    "simplified": "老师好",
    "traditional": "老師好",
    "pinyin": "lao3 shi1 hao3",
    "definition": "Hello teacher",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": null,
    "parentTopic": null,
    "notes": null,
    "frequency": 0.0,
    "partOfSpeech": [
      "PHRASE"
    ]
  },
  {
    "id": 290128,
    "simplified": "喴",
    "traditional": "喴",
    "pinyin": "wei1",
    "definition": "Hello,  (Cant.) phonetic",
    "classifier": null,
    "concept": null,
    "hskLevel": 0,
    "topic": null,
    "parentTopic": null,
    "notes": null,
    "frequency": -8.625691405109933,
    "partOfSpeech": null
  }
]

const store = createStore({
    state () {
        return {
            searchQuery: '你好',
            searchResults: sampleData,
            editingEntry: { simplified: 'hi' } 
        }
    },
    mutations: {
        setSearchQuery(state, query) {
            state.searchQuery = query
        },
        setEditing(state, entry) {
            if (state.editingEntry) {
                state.editingEntry.editing = false;
                state.editingEntry = null;
                // should commit or abort the existing edit here
            }

            entry.editing = true
            state.editingEntry = entry 
        }
    },
    actions: {
        search(context, query) {
            state.searchQuery = query;
        },
        addOrEdit(context, entry) {
            this.commit('setEditing', entry)
        }
    },
    getters: {
        searchQuery:  (state, getters) => { return getters.searchQuery },
        searchResults:  (state, getters) => { return getters.searchResults },
        editingEntry: (state, getters) => { return getters.editingEntry }
    }
})

const app = createApp(App)
app.use(store);
app.mount('#app')
