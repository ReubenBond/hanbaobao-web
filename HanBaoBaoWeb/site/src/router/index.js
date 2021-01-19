import { createRouter, createWebHashHistory } from 'vue-router';

import SearchBox from '../components/SearchBox.vue'
import Dictionary from '../components/Dictionary.vue'
import DictionaryEntryEditor from '../components/DictionaryEntryEditor.vue'

export default createRouter({
    history: createWebHashHistory(),
    routes: [ {
        name: 'Home',
        path: '/',
        components: {
            default: Dictionary,
            nav: SearchBox
        }
    }, {
        name: 'Search',
        path: '/search/:id',
        components: {
            default: Dictionary,
            nav: SearchBox
        }
    }, {
        name: 'Editor',
        path: '/entry/:id/edit',
        components: {
            default: DictionaryEntryEditor,
            nav: SearchBox
        }
    }]
});