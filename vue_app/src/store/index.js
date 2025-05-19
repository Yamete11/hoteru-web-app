import { createStore } from 'vuex';
import axios from "axios";

export default createStore({
    state: {
        token: null,
        userData: null,
        userRole: null
    },
    getters: {
        isAuthenticated: state => !!state.token,
        getToken: state => state.token,
        getUserData: state => state.userData,
        getUserRole: state => state.userRole
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
        setUserRole(state, role) {
            state.userRole = role;
            localStorage.setItem('userRole', role);
        },
    },
    actions: {
        initializeStore({ commit }) {
            const token = localStorage.getItem('token');
            if (token) {
                commit('setToken', token);
            }

            const userData = JSON.parse(localStorage.getItem('userData'));
            if (userData) {
                commit('setUserData', userData);
            }

            const userRole = localStorage.getItem('userRole');
            if (userRole) {
                commit('setUserRole', userRole);
            }
        },
        async fetchUserData({ commit, getters }, userName) {
            try {
                const response = await axios.get('https://localhost:44384/api/User/' + userName, {
                    headers: {
                        'Authorization': `Bearer ${getters.getToken}`
                    },
                });
                const userData = response.data;
                commit('setUserData', userData);

                localStorage.setItem('userData', JSON.stringify(userData));
            } catch (error) {
                console.error(error);
                commit('clearUserData');
                localStorage.removeItem('userData');
            }
        }
    }

});
