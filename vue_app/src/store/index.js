import { createStore } from 'vuex';
import axios from 'axios';
import { auth, users } from '@/api';

function base64UrlToJson(b64url) {
    const pad = '='.repeat((4 - (b64url.length % 4)) % 4);
    const b64 = (b64url + pad).replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(atob(b64));
}
function decodeJwt(token) {
    try { return base64UrlToJson(token.split('.')[1]); } catch { return null; }
}
function decodeJwtRole(token) {
    const p = decodeJwt(token);
    return p?.role ?? null;
}
function isExpired(token, skewSec = 30) {
    const p = decodeJwt(token);
    if (!p?.exp) return true;
    return Math.floor(Date.now() / 1000) >= (p.exp - skewSec);
}

function extractCompanyName(userOrWrapper) {
    const u = userOrWrapper?.data ?? userOrWrapper;
    return typeof u?.companyTitle === 'string' && u.companyTitle.trim()
        ? u.companyTitle.trim()
        : null;
}


export default createStore({
    state: {
        token: null,
        userData: null,
        userRole: null,
    },
    getters: {
        isAuthenticated: s => !!s.token,
        getToken: s => s.token,
        getUserData: s => s.userData,
        getUserRole: s => s.userRole,
        getCompanyName: s => extractCompanyName(s.userData) || 'No Company',
        getPersonId: s => {
            const t = s.token || localStorage.getItem('token');
            if (!t) return null;
            try {
                const p = decodeJwt(t);
                return p?.sub ? Number(p.sub) : null;
            } catch { return null; }
        },
    },
    mutations: {
        setToken(state, token) {
            state.token = token;
            if (token) {
                localStorage.setItem('token', token);
                axios.defaults.headers.common.Authorization = `Bearer ${token}`;
            } else {
                localStorage.removeItem('token');
                delete axios.defaults.headers.common.Authorization;
            }
        },
        setUserData(state, val) {
            state.userData = val;
            if (val) localStorage.setItem('userData', JSON.stringify(val));
            else localStorage.removeItem('userData');
        },
        setUserRole(state, role) {
            state.userRole = role || null;
            if (role) localStorage.setItem('userRole', role);
            else localStorage.removeItem('userRole');
        },
    },
    actions: {
        initializeStore({ commit }) {
            const token = localStorage.getItem('token');

            if (token && !isExpired(token)) {
                commit('setToken', token);
                commit('setUserRole', localStorage.getItem('userRole') || decodeJwtRole(token));

                const raw = localStorage.getItem('userData');
                if (raw) {
                    try {
                        const parsed = JSON.parse(raw);
                        const normalized = parsed?.data ? { ...parsed.data } : parsed;
                        const name = extractCompanyName(parsed);
                        const enriched = name ? { ...normalized, companyTitle: name } : normalized;

                        commit('setUserData', enriched);
                        localStorage.setItem('userData', JSON.stringify(enriched));
                    } catch {}
                }
            } else {
                commit('setToken', null);
                commit('setUserRole', null);
                commit('setUserData', null);
                localStorage.removeItem('userData');
                localStorage.removeItem('userRole');
            }
        },


        async login({ commit, dispatch }, { login, password }) {
            const res = await auth.login({ login, password });
            const token = res?.data?.token;
            if (!token) throw new Error('No token in response');

            commit('setToken', token);
            commit('setUserRole', decodeJwtRole(token));

            await dispatch('fetchUserData', login);
            return res;
        },

        async fetchUserData({ commit }, userName) {
            try {
                const raw = await users.getByName(userName);
                const payload = raw?.data ?? raw;
                const name = extractCompanyName(raw);
                const enriched = name ? { ...payload, companyTitle: name } : payload;

                commit('setUserData', enriched);
                localStorage.setItem('userData', JSON.stringify(enriched));
            } catch (err) {
                console.error('fetchUserData failed', err);
                commit('setUserData', null);
                localStorage.removeItem('userData');
                throw err;
            }
        },


        logout({ commit }) {
            commit('setToken', null);
            commit('setUserRole', null);
            commit('setUserData', null);

            localStorage.removeItem('token');
            localStorage.removeItem('userData');
            localStorage.removeItem('userRole');
        },
    },
});
