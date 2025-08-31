import { createStore } from 'vuex'
import { login as apiLogin, refresh as apiRefresh, logout as apiLogout, me as apiMe } from '@/api/auth'
import { users } from '@/api'

function extractCompanyName(userOrWrapper) {
    const u = userOrWrapper?.data ?? userOrWrapper
    return typeof u?.companyTitle === 'string' && u.companyTitle.trim()
        ? u.companyTitle.trim()
        : null
}

export default createStore({
    state: {
        userData: null,
        userRole: null,
        refreshTimerId: null,
        companyName: '',
    },
    getters: {
        isAuthenticated: s => !!s.userData,
        getUserData: s => s.userData,
        getUserRole: s => s.userRole,
        getPersonId: s => s.userData?.id ? Number(s.userData.id) : null,
        getCompanyName: (s) =>
            (s.companyName && s.companyName.trim()) ||
            extractCompanyName(s.userData) ||
            'No Company',
    },
    mutations: {
        setUserData(state, val) {
            state.userData = val
            if (val) {
                localStorage.setItem('userData', JSON.stringify(val))
                const name = extractCompanyName(val)
                state.companyName = name || ''
            } else {
                localStorage.removeItem('userData')
                state.companyName = ''
            }
        },
        setUserRole(state, role) {
            state.userRole = role || null
        },
        setRefreshTimerId(state, id) {
            if (state.refreshTimerId) clearTimeout(state.refreshTimerId)
            state.refreshTimerId = id || null
        },
        setCompanyName(state, name) {
            state.companyName = String(name || '')
        },
    },
    actions: {
        async initializeStore({ commit }) {
            try {
                const me = await apiMe()
                commit('setUserData', me)
                commit('setUserRole', me?.role || null)
            } catch {
                commit('setUserData', null)
                commit('setUserRole', null)
                commit('setRefreshTimerId', null)
            }
        }
        ,

        setCompanyName({ commit }, name) {
            commit('setCompanyName', name)
        },

        async login({ commit, dispatch }, { login, password }) {
            const { expiresAtUtc } = await apiLogin({ login, password })
            const me = await apiMe()
            commit('setUserData', me)
            commit('setUserRole', me?.role || null)
            try {
                if (me?.login) {
                    const raw = await users.getByName(me.login)
                    const payload = raw?.data ?? raw
                    const name = extractCompanyName(raw)
                    const enriched = name ? { ...me, ...payload, companyTitle: name } : { ...me, ...payload }
                    commit('setUserData', enriched)
                }
            } catch {}
            dispatch('scheduleRefresh', { expiresAtUtc })
        },

        async fetchUserData({ commit }) {
            const me = await apiMe()
            commit('setUserData', me)
            commit('setUserRole', me?.role || null)
            return me
        },

        async refresh({ dispatch }) {
            const { expiresAtUtc } = await apiRefresh()
            dispatch('scheduleRefresh', { expiresAtUtc })
            return expiresAtUtc
        },

        scheduleRefresh({ commit, dispatch }, { expiresAtUtc }) {
            let tExpMs = expiresAtUtc ? Date.parse(expiresAtUtc) : NaN
            const skewMs = 60 * 1000
            let delay = Number.isFinite(tExpMs) ? (tExpMs - Date.now() - skewMs) : 10 * 60 * 1000
            delay = Math.max(delay, 5 * 1000)
            const id = setTimeout(() => { dispatch('refresh').catch(() => {}) }, delay)
            commit('setRefreshTimerId', id)
        },

        async logout({ commit }) {
            try { await apiLogout() } catch {}
            commit('setRefreshTimerId', null)
            commit('setUserData', null)
            commit('setUserRole', null)
            localStorage.removeItem('userData')
            window.location.assign('/')
        },
    },
})
