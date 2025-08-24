import { createApp } from 'vue';
import App from './App.vue';
import components from './components/UI';
import router from './router/router.js';
import VIntersection from './components/directives/VIntersection.js';
import store from './store';
import './assets/global.css';
import Notifications from '@kyvg/vue3-notification';

const app = createApp(App);

components.forEach(c => app.component(c.name, c));

app.directive('intersection', VIntersection);

app
    .use(router)
    .use(store)
    .use(Notifications)
    .mount('#app');
