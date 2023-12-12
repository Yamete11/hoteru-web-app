import { createApp } from 'vue'
import App from './App.vue'
import components from './components/UI';
import router from './router/router.js'
import VIntersection from "./components/directives/VIntersection.js";

const app = createApp(App);

components.forEach(component => {
    app.component(component.name, component)
})

router.beforeEach((to, from, next) => {
    const requiresAuth = !['/', '/registration'].includes(to.path);
    const token = localStorage.getItem('token');

    if (requiresAuth && !token) {
        next('/');
    } else {
        next();
    }
});


app.directive('intersection', VIntersection)

app
    .use(router)
    .mount('#app')