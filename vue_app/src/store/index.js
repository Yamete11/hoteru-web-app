import { createStore } from 'vuex';

export default createStore({
    state: {
        token: null,
    },
    getters: {
        isAuthenticated: state => !!state.token,
        getToken: state => state.token,
    },
    mutations: {
        setToken(state, token) {
            state.token = token;
        },
        clearToken(state) {
            state.token = null;
        }
    },
    actions: {
        login({ commit }, token) {
            commit('setToken', token);
            localStorage.setItem('token', token);
        },
        logout({ commit }) {
            commit('clearToken');
            localStorage.removeItem('token');
        },
        initializeStore({ commit }) {
            const token = localStorage.getItem('token');
            if (token) {
                commit('setToken', token);
            } else {
                commit('clearToken');
            }
        }
    }
});
