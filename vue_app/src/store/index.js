import { createStore } from 'vuex';
import { setAuthToken } from '@/lib/http'
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
        refreshTimerId: null,
        companyName: '',
    },
    getters: {
        isAuthenticated: s => !!s.token,
        getToken: s => s.token,
        getUserData: s => s.userData,
        getUserRole: s => s.userRole,
        getCompanyName: (s) =>
            (s.companyName && s.companyName.trim()) ||
            extractCompanyName(s.userData) ||
            'No Company',
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
            state.token = token
            localStorage[token ? 'setItem' : 'removeItem']('token', token)
            setAuthToken(token)
        },
        setUserData(state, val) {
            state.userData = val
            if (val) {
                localStorage.setItem('userData', JSON.stringify(val))
                const name = extractCompanyName(val)
                if (name) state.companyName = name
            } else {
                localStorage.removeItem('userData')
                state.companyName = ''
            }
        },
        setUserRole(state, role) {
            state.userRole = role || null;
            if (role) localStorage.setItem('userRole', role);
            else localStorage.removeItem('userRole');
        },
        setRefreshTimerId(state, id) {
            if (state.refreshTimerId) clearTimeout(state.refreshTimerId);
            state.refreshTimerId = id || null;
        },
        setCompanyName(state, name) {
            state.companyName = String(name || '')
        },
    },
    actions: {
        initializeStore({ commit, dispatch  }) {
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
                dispatch('scheduleRefresh', { token });
            } else {
                commit('setToken', null);
                commit('setUserRole', null);
                commit('setUserData', null);
                localStorage.removeItem('userData');
                localStorage.removeItem('userRole');
            }
        },

        setCompanyName({ commit }, name) {
            commit('setCompanyName', name)
        },


        async login({ commit, dispatch }, { login, password }) {
            const res = await auth.login({ login, password });

            const body  = res?.data ?? res;
            const token = body?.token ?? res?.data?.data?.token;
            const expiresAtUtc = body?.expiresAtUtc ?? res?.data?.data?.expiresAtUtc;

            if (!token) {
                console.log('Login response shape:', res);
                throw new Error('No token in response');
            }

            commit('setToken', token);
            commit('setUserRole', decodeJwtRole(token));

            dispatch('scheduleRefresh', { token, expiresAtUtc });

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

        async refresh({ commit, dispatch }) {
            const res = await auth.refresh();
            const body  = res?.data ?? res;
            const token = body?.token ?? res?.data?.data?.token;
            const expiresAtUtc = body?.expiresAtUtc ?? res?.data?.data?.expiresAtUtc;
            if (!token) throw new Error('No token from refresh');

            commit('setToken', token);
            commit('setUserRole', decodeJwtRole(token));
            dispatch('scheduleRefresh', { token, expiresAtUtc });
        },

        scheduleRefresh({ commit, dispatch }, { token, expiresAtUtc }) {
            let tExpMs = expiresAtUtc ? Date.parse(expiresAtUtc) : (decodeJwt(token)?.exp || 0) * 1000;
            const skewMs = 60 * 1000;
            let delay = tExpMs - Date.now() - skewMs;
            if (!Number.isFinite(delay)) delay = 30 * 1000;
            delay = Math.max(delay, 5 * 1000);

            const id = setTimeout(() => {
                dispatch('refresh').catch(console.error);
            }, delay);

            commit('setRefreshTimerId', id);
        },


        logout({ commit }) {
            commit('setRefreshTimerId', null);
            commit('setToken', null);
            commit('setUserRole', null);
            commit('setUserData', null);

            localStorage.removeItem('token');
            localStorage.removeItem('userData');
            localStorage.removeItem('userRole');
        },
    },
});
