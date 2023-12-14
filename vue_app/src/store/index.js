import { createStore } from 'vuex';
import axios from "axios";

export default createStore({
    state: {
        token: null,
        userData: null,
    },
    getters: {
        isAuthenticated: state => !!state.token,
        getToken: state => state.token,
        getUserData: state => state.userData,
    },
    mutations: {
        setToken(state, token) {
            state.token = token;
        },
        clearToken(state) {
            state.token = null;
        },
        setUserData(state, userData) {
            state.userData = userData;
        },
        clearUserData(state) {
            state.userData = null;
        },
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
        },
        async fetchUserData({ commit, getters }, userName) {
            try {
                const response = await axios.get('https://localhost:44384/api/User/' + userName, {
                    headers: {
                        'Authorization': `Bearer ${getters.getToken}`
                    },
                });
                commit('setUserData', response.data);
            } catch (error) {
                console.error(error);
                commit('clearUserData');
            }
        }
    }
});
