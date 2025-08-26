<template>
  <div class="newRoom-component">
    <navbar />
    <sidebar />

    <div class="main">
      <form @submit.prevent="toggleEdit" class="creating-form">
        <h1>Hotel Settings</h1>

        <div class="input-form">
          <label>Title:</label>
          <input
              v-model="state.form.title"
              class="input"
              type="text"
              placeholder="Enter title"
              :readonly="!state.isEditing"
              @input="v$.form.title.$touch()"
              data-testid="hotel-title"
          />
          <span class="error-message" v-if="v$.form.title.$error">
            <span v-if="!v$.form.title.required.$response">Title is required*</span>
            <span v-else-if="!v$.form.title.onlyAsciiLetters.$response">Letters A–Z only, no spaces*</span>
            <span v-else-if="!v$.form.title.maxLength.$response">Max 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Title">{{ state.errors.Title?.[0] }}</span>
        </div>

        <div class="input-form">
          <label>City:</label>
          <input
              v-model="state.form.city"
              class="input"
              type="text"
              placeholder="Enter city"
              :readonly="!state.isEditing"
              @input="v$.form.city.$touch()"
              data-testid="hotel-city"
          />
          <span class="error-message" v-if="v$.form.city.$error">
            <span v-if="!v$.form.city.required.$response">City is required*</span>
            <span v-else-if="!v$.form.city.onlyAsciiLetters.$response">Letters A–Z only, no spaces*</span>
            <span v-else-if="!v$.form.city.maxLength.$response">Max 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.City">{{ state.errors.City?.[0] }}</span>
        </div>

        <div class="input-form">
          <label>Country:</label>
          <input
              v-model="state.form.country"
              class="input"
              type="text"
              placeholder="Enter country"
              :readonly="!state.isEditing"
              @input="v$.form.country.$touch()"
              data-testid="hotel-country"
          />
          <span class="error-message" v-if="v$.form.country.$error">
            <span v-if="!v$.form.country.required.$response">Country is required*</span>
            <span v-else-if="!v$.form.country.onlyAsciiLetters.$response">Letters A–Z only, no spaces*</span>
            <span v-else-if="!v$.form.country.maxLength.$response">Max 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Country">{{ state.errors.Country?.[0] }}</span>
        </div>

        <div class="input-form">
          <label>Street:</label>
          <input
              v-model="state.form.street"
              class="input"
              type="text"
              placeholder="Enter street"
              :readonly="!state.isEditing"
              @input="v$.form.street.$touch()"
              data-testid="hotel-street"
          />
          <span class="error-message" v-if="v$.form.street.$error">
            <span v-if="!v$.form.street.required.$response">Street is required*</span>
            <span v-else-if="!v$.form.street.maxLength.$response">Max 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Street">{{ state.errors.Street?.[0] }}</span>
        </div>

        <div class="input-form">
          <label>Postcode:</label>
          <input
              v-model="state.form.postcode"
              class="input"
              type="text"
              placeholder="Enter postcode"
              :readonly="!state.isEditing"
              @input="v$.form.postcode.$touch()"
              data-testid="hotel-postcode"
          />
          <span class="error-message" v-if="v$.form.postcode.$error">
            <span v-if="!v$.form.postcode.required.$response">Postcode is required*</span>
            <span v-else-if="!v$.form.postcode.maxLength.$response">Max 15 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Postcode">{{ state.errors.Postcode?.[0] }}</span>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" to="/arrivals" data-testid="button-back">Back</router-link>
          <button
              class="registration-btn"
              type="submit"
              :disabled="state.isSubmitting || (state.isEditing && v$.$invalid)"
              data-testid="button-submit"
          >
            {{ state.isEditing ? (state.isSubmitting ? 'Saving...' : 'Save') : 'Edit' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { reactive, onMounted } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, maxLength } from '@vuelidate/validators'
import { notify } from '@kyvg/vue3-notification'
import { useStore } from 'vuex'
import * as hotel from '@/api/hotel'

export default {
  name: 'HotelSettings',
  setup() {
    const store = useStore()

    const state = reactive({
      isEditing: false,
      isSubmitting: false,
      form: {
        title: '',
        city: '',
        country: '',
        street: '',
        postcode: '',
      },
      original: null,
      errors: {},
    })

    const onlyAsciiLetters = (v) => typeof v === 'string' && /^[A-Za-z]+$/.test(v)

    const rules = {
      form: {
        title: { required, onlyAsciiLetters, maxLength: maxLength(20) },
        city: { required, onlyAsciiLetters, maxLength: maxLength(20) },
        country: { required, onlyAsciiLetters, maxLength: maxLength(20) },
        street: { required, maxLength: maxLength(20) },
        postcode: { required, maxLength: maxLength(15) },
      },
    }
    const v$ = useVuelidate(rules, state)

    const mapFromApi = (api) => {
      const d = api?.data ?? api ?? {}
      const h = d?.data ?? d ?? {}
      return {
        title: h.Title ?? h.title ?? '',
        city: h.City ?? h.city ?? '',
        country: h.Country ?? h.country ?? '',
        street: h.Street ?? h.street ?? '',
        postcode: h.Postcode ?? h.postcode ?? '',
      }
    }

    const mapToApi = (ui) => ({
      Title: String(ui.title).trim(),
      City: String(ui.city).trim(),
      Country: String(ui.country).trim(),
      Street: String(ui.street).trim(),
      Postcode: String(ui.postcode).trim(),
    })

    async function load() {
      try {
        const res = await hotel.get()
        state.form = mapFromApi(res)
        state.original = { ...state.form }
        v$.value.$reset()
      } catch (e) {
        console.error('hotel.load error', e)
        notify({ title: 'Load failed', text: 'Could not load hotel settings', type: 'error' })
      }
    }

    async function toggleEdit() {
      if (!state.isEditing) {
        state.isEditing = true
        return
      }

      state.errors = {}
      v$.value.$touch()
      if (v$.value.$error) return

      state.isSubmitting = true
      try {
        const payload = mapToApi(state.form)
        const res = await hotel.update(payload)

        const code = res?.httpStatusCode ?? res?.data?.httpStatusCode
        const msg  = res?.message ?? res?.data?.message
        const err  = res?.errors ?? res?.data?.errors

        if (code && code !== 200) {
          state.errors = err || {}
          notify({ title: 'Save failed', text: msg || 'Validation failed', type: 'error' })
          return
        }

        store.dispatch?.('setCompanyName', payload.Title)
        const current = store.getters.getUserData || {}
        store.commit('setUserData', { ...current, companyTitle: payload.Title })

        state.original = { ...state.form }
        state.isEditing = false
        notify({ title: 'Saved', text: 'Hotel settings updated', type: 'success' })
      } catch (e) {
        const errors = e?.response?.data?.errors
        if (errors) state.errors = errors
        notify({
          title: 'Save failed',
          text: e?.response?.data?.message || e?.message || 'Unexpected error',
          type: 'error',
        })
      } finally {
        state.isSubmitting = false
      }
    }

    onMounted(load)

    return { state, v$, toggleEdit }
  },
}
</script>

<style scoped>
.newRoom-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
  height: 100vh;
}
.main {
  display: flex;
  align-items: center;
  flex-direction: column;
  flex-grow: 1;
  padding-top: 8vh;
  margin: 5%;
}

.creating-form {
  width: 20%;
  max-width: 720px;
  background: #fff;
  border-radius: 8px;
  padding: 24px 20px;
  box-shadow: 0 6px 16px rgba(0,0,0,.08);
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.input-form {
  display: flex;
  flex-direction: column;
  margin: 5px;
}
.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: black;
}
.input-form input[type="text"] {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 6px;
}
.input[readonly] {
  background: #f3f3f3;
  color: #666;
}

h1 { display: flex;
  justify-content: center;
  color: black;
}

.registration-class {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.registration-btn {
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
  margin: 10px;
  cursor: pointer;
}
.registration-btn[disabled] {
  opacity: .6;
  cursor: not-allowed;
}

.error-message {
  color: red;
  margin: 2px 0 8px;
  font-size: 0.9rem;
}
</style>
