<template>
  <div class="newRoom-component" data-testid="new-guest-page">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1 data-testid="new-guest-title">New Guest</h1>
      <form @submit.prevent="addGuest" class="creating-form" data-testid="new-guest-form">

        <div class="input-form" data-testid="input-name">
          <label for="name-input">Name: </label>
          <input
              id="name-input"
              v-model="state.formData.Name"
              class="input"
              type="text"
              placeholder="Enter name"
              @input="v$.formData.Name.$touch()"
              data-testid="name-input"
          />
          <span class="error-message" v-if="v$.formData.Name.$error" data-testid="name-error">
            <span v-if="!v$.formData.Name.required.$response">Name is required*</span>
            <span v-else-if="!v$.formData.Name.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.Name.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Name" data-testid="name-error-backend">{{ state.errors.Name[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-surname">
          <label for="surname-input">Surname: </label>
          <input
              id="surname-input"
              v-model="state.formData.Surname"
              class="input"
              type="text"
              placeholder="Enter surname"
              @input="v$.formData.Surname.$touch()"
              data-testid="surname-input"
          />
          <span class="error-message" v-if="v$.formData.Surname.$error" data-testid="surname-error">
            <span v-if="!v$.formData.Surname.required.$response">Surname is required*</span>
            <span v-else-if="!v$.formData.Surname.maxLength.$response">Surname must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.Surname.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Surname" data-testid="surname-error-backend">{{ state.errors.Surname[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-email">
          <label for="email-input">Email: </label>
          <input
              id="email-input"
              v-model="state.formData.Email"
              class="input"
              type="text"
              placeholder="Enter email"
              @input="v$.formData.Email.$touch()"
              data-testid="email-input"
          />
          <span class="error-message" v-if="v$.formData.Email.$error" data-testid="email-error">
            <span v-if="!v$.formData.Email.required.$response">Email is required*</span>
            <span v-if="!v$.formData.Email.email.$response">Invalid email format*</span>
          </span>
          <span class="error-message" v-if="state.errors.Email" data-testid="email-error-backend">{{ state.errors.Email[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-tel">
          <label for="tel-input">Tel. number: </label>
          <input
              id="tel-input"
              v-model="state.formData.TelNumber"
              class="input"
              type="text"
              placeholder="Enter tel. number"
              @input="v$.formData.TelNumber.$touch()"
              data-testid="tel-input"
          />
          <span class="error-message" v-if="v$.formData.TelNumber.$error" data-testid="tel-error">
            <span v-if="!v$.formData.TelNumber.required.$response">Telephone number is required*</span>
            <span v-if="!v$.formData.TelNumber.numeric.$response">Telephone number must be a number*</span>
            <span v-if="!v$.formData.TelNumber.maxLength.$response">Telephone number must be less than 15 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.TelNumber" data-testid="tel-error-backend">{{ state.errors.TelNumber[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-passport">
          <label for="passport-input">Passport: </label>
          <input
              id="passport-input"
              v-model="state.formData.Passport"
              class="input"
              type="text"
              placeholder="Enter passport number"
              @input="v$.formData.Passport.$touch()"
              data-testid="passport-input"
          />
          <span class="error-message" v-if="v$.formData.Passport.$error" data-testid="passport-error">
            <span v-if="!v$.formData.Passport.required.$response">Passport number is required*</span>
            <span v-if="!v$.formData.Passport.maxLength.$response">Passport number must be less than 10 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Passport" data-testid="passport-error-backend">{{ state.errors.Passport[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-status">
          <label for="status-select">Status: </label>
          <select
              id="status-select"
              v-model="state.formData.IdGuestStatus"
              @change="v$.formData.IdGuestStatus.$touch()"
              data-testid="status-select"
          >
            <option disabled value="">Select status</option>
            <option
                v-for="guestStatus in state.guestStatuses"
                :key="guestStatus.idStatus"
                :value="String(guestStatus.idStatus)"
                :data-testid="`status-option-${guestStatus.idStatus}`"
            >
              {{ guestStatus.title }}
            </option>
          </select>
          <span class="error-message" v-if="v$.formData.IdGuestStatus.$error" data-testid="status-error">
            <span v-if="!v$.formData.IdGuestStatus.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.IdGuestStatus" data-testid="status-error-backend">{{ state.errors.IdGuestStatus[0] }}</span>
        </div>

        <div class="registration-class" data-testid="form-buttons">
          <router-link class="registration-btn" to="/guests" data-testid="cancel-button">Cancel</router-link>
          <button class="registration-btn" type="submit" data-testid="confirm-button">Confirm</button>
        </div>
      </form>
    </div>
  </div>
</template>


<script>
import axios from "axios";
import {onMounted, reactive} from "vue";
import {useVuelidate} from "@vuelidate/core";
import {email, numeric, maxLength, required} from "@vuelidate/validators";
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import {notify} from "@kyvg/vue3-notification";
import { guests } from '@/api';



export default {
  name: "NewGuest",
  setup() {
    const store = useStore();
    const router = useRouter();
    const state = reactive({
      formData: {
        Name: '',
        Surname: '',
        Email: '',
        TelNumber: '',
        Passport: '',
        IdGuestStatus: ''
      },
      guestStatuses: [],
      errors: {}
    });

    function onlyLetters(value) {
      return /^[A-Za-z]+$/.test(value);
    }

    const rules = {
      formData: {
        Name: { required, maxLength: maxLength(20), onlyLetters },
        Surname: { required, maxLength: maxLength(20), onlyLetters},
        Email: { required, email },
        TelNumber: { required, maxLength: maxLength(15), numeric },
        Passport: { required, maxLength: maxLength(10) },
        IdGuestStatus: { required }
      }
    }

    const v$ = useVuelidate(rules, state);

    async function fetchGuestStatuses() {
      try {
        const dto = await guests.status();
        state.guestStatuses = dto.data;
      } catch (error) {
        notify({ title: 'Load failed', text: error?.message || 'Failed to load guest statuses', type: 'error' });
        console.error(error);
      }
    }


    async function addGuest() {
      state.errors = {};
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        state.isSubmitting = true;

        const payload = {
          Name: String(state.formData.Name ?? '').trim(),
          Surname: String(state.formData.Surname ?? '').trim(),
          Email: String(state.formData.Email ?? '').trim(),
          TelNumber: String(state.formData.TelNumber ?? '').trim(),
          Passport: String(state.formData.Passport ?? '').trim(),
          IdGuestStatus: String(state.formData.IdGuestStatus)
        };

        const res = await guests.create(payload);

        if (res?.httpStatusCode === 201 || res?.httpStatusCode === 200) {
          notify({ title: 'Guest Created', text: 'Guest has been created successfully.', type: 'success', duration: 3000 });
          await router.push({ path: '/guests', query: { created: 'true' } });
          return;
        }

        const errors = res?.errors || res?.data?.errors;
        if (errors) state.errors = errors;

        notify({ title: 'Create failed', text: res?.message || res?.data?.message || 'Validation failed', type: 'error' });
      } catch (error) {
        const backendErrors = error?.response?.data?.errors || error?.details;
        if (backendErrors) state.errors = backendErrors;

        notify({ title: 'Create failed', text: error?.response?.data?.message || error?.message || 'Unexpected error', type: 'error' });
      } finally {
        state.isSubmitting = false;
      }
    }

    onMounted(fetchGuestStatuses);

    return { state, v$, addGuest };
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
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 300px;
}

.registration-btn{
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

.registration-class{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
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
  margin-bottom: 10px;
}

h1 {
  display: flex;
  justify-content: center;
  color: black;
}

.input-form select {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px;
  background-color: white;
  color: black;
}
.error-message {
  color: red;
  margin: 10px 0;
}
</style>