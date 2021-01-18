import { createRouter, createWebHistory } from 'vue-router';

import Dictionary from '../components/Dictionary.vue'
import DictionaryEntryEditor from '../components/DictionaryEntryEditor.vue'

export default createRouter({
    history: createWebHistory(),
    routes: [ {
        name: 'Home',
        path: '/',
        component: Dictionary
    }, {
        name: 'Editor',
        path: '/editor',
        component: DictionaryEntryEditor 
    }]
});