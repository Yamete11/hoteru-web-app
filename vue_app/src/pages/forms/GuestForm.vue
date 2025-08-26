<template>
  <div class="newRoom-component" :data-testid="state.isCreate ? 'new-guest-page' : 'guest-details-page'">
    <navbar />
    <sidebar />

    <div class="main">
      <h1 :data-testid="state.isCreate ? 'new-guest-title' : 'guest-details-title'">
        {{ state.isCreate ? 'New Guest' : 'Guest Details' }}
      </h1>

      <form @submit.prevent="onSubmit" class="creating-form" :data-testid="state.isCreate ? 'new-guest-form' : 'guest-details-form'">
        <div class="input-form" data-testid="input-name">
          <label for="name-input">Name: </label>
          <input
              id="name-input"
              :data-testid="state.isCreate ? 'name-input' : 'input-name'"
              v-model="state.formData.name"
              class="input"
              type="text"
              placeholder="Enter name"
              :readonly="!state.isEditing"
              @input="v$.formData.name.$touch()"
          />
          <span class="error-message" v-if="v$.formData.name.$error" data-testid="name-error">
            <span v-if="!v$.formData.name.required.$response">Name is required*</span>
            <span v-else-if="!v$.formData.name.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.name.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="fieldError('Name','name')" data-testid="name-error-backend">{{ fieldError('Name','name') }}</span>
        </div>

        <div class="input-form" data-testid="input-surname">
          <label for="surname-input">Surname: </label>
          <input
              id="surname-input"
              :data-testid="state.isCreate ? 'surname-input' : 'input-surname'"
              v-model="state.formData.surname"
              class="input"
              type="text"
              placeholder="Enter surname"
              :readonly="!state.isEditing"
              @input="v$.formData.surname.$touch()"
          />
          <span class="error-message" v-if="v$.formData.surname.$error" data-testid="surname-error">
            <span v-if="!v$.formData.surname.required.$response">Surname is required*</span>
            <span v-else-if="!v$.formData.surname.maxLength.$response">Surname must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.surname.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="fieldError('Surname','surname')" data-testid="surname-error-backend">{{ fieldError('Surname','surname') }}</span>
        </div>

        <div class="input-form" data-testid="input-email">
          <label for="email-input">Email: </label>
          <input
              id="email-input"
              :data-testid="state.isCreate ? 'email-input' : 'input-email'"
              v-model="state.formData.email"
              class="input"
              type="text"
              placeholder="Enter email"
              :readonly="!state.isEditing"
              @input="v$.formData.email.$touch()"
          />
          <span class="error-message" v-if="v$.formData.email.$error" data-testid="email-error">
            <span v-if="!v$.formData.email.required.$response">Email is required*</span>
            <span v-else-if="!v$.formData.email.email.$response">Invalid email format*</span>
          </span>
          <span class="error-message" v-if="fieldError('Email','email')" data-testid="email-error-backend">{{ fieldError('Email','email') }}</span>
        </div>

        <div class="input-form" data-testid="input-tel">
          <label for="tel-input">Tel. number: </label>
          <input
              id="tel-input"
              :data-testid="state.isCreate ? 'tel-input' : 'input-tel'"
              v-model="state.formData.telNumber"
              class="input"
              type="text"
              placeholder="Enter tel. number"
              :readonly="!state.isEditing"
              @input="v$.formData.telNumber.$touch()"
          />
          <span class="error-message" v-if="v$.formData.telNumber.$error" data-testid="tel-error">
            <span v-if="!v$.formData.telNumber.required.$response">Telephone number is required*</span>
            <span v-else-if="!v$.formData.telNumber.numeric.$response">Telephone number must be a number*</span>
            <span v-else-if="!v$.formData.telNumber.maxLength.$response">Telephone number must be less than 15 characters*</span>
          </span>
          <span class="error-message" v-if="fieldError('TelNumber','telNumber')" data-testid="tel-error-backend">{{ fieldError('TelNumber','telNumber') }}</span>
        </div>

        <div class="input-form" data-testid="input-passport">
          <label for="passport-input">Passport: </label>
          <input
              id="passport-input"
              :data-testid="state.isCreate ? 'passport-input' : 'input-passport'"
              v-model="state.formData.passport"
              class="input"
              type="text"
              placeholder="Enter passport number"
              :readonly="!state.isEditing"
              @input="v$.formData.passport.$touch()"
          />
          <span class="error-message" v-if="v$.formData.passport.$error" data-testid="passport-error">
            <span v-if="!v$.formData.passport.required.$response">Passport number is required*</span>
            <span v-else-if="!v$.formData.passport.maxLength.$response">Passport number must be less than 10 characters*</span>
          </span>
          <span class="error-message" v-if="fieldError('Passport','passport')" data-testid="passport-error-backend">{{ fieldError('Passport','passport') }}</span>
        </div>

        <div class="input-form" data-testid="input-status">
          <label for="status-select">Status: </label>

          <template v-if="!state.isEditing && !state.isCreate">
            <input class="input" type="text" :value="state.statusTitle" readonly />
          </template>
          <template v-else>
            <select
                id="status-select"
                :data-testid="state.isCreate ? 'status-select' : 'input-status-select'"
                v-model="state.formData.idGuestStatus"
                @change="v$.formData.idGuestStatus.$touch()"
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
          </template>

          <span class="error-message" v-if="v$.formData.idGuestStatus.$error" data-testid="status-error">
            <span v-if="!v$.formData.idGuestStatus.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="fieldError('IdGuestStatus','idGuestStatus')" data-testid="status-error-backend">{{ fieldError('IdGuestStatus','idGuestStatus') }}</span>
        </div>

        <div class="registration-class" data-testid="form-buttons">
          <router-link class="registration-btn" to="/guests" :data-testid="state.isCreate ? 'cancel-button' : 'back-button'">
            {{ state.isCreate ? 'Cancel' : 'Back' }}
          </router-link>

          <button
              v-if="state.isCreate"
              class="registration-btn"
              type="submit"
              :disabled="state.isSubmitting"
              data-testid="confirm-button"
          >
            Confirm
          </button>

          <button
              v-else
              type="submit"
              class="registration-btn"
              :disabled="state.isSubmitting"
              data-testid="edit-save-button"
          >
            {{ state.isEditing ? 'Save' : 'Edit' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { reactive, onMounted, watch } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { email as emailV, numeric, maxLength, required } from '@vuelidate/validators';
import { useRouter, useRoute } from 'vue-router';
import { notify } from '@kyvg/vue3-notification';
import { guests } from '@/api';

export default {
  name: 'GuestForm',
  props: {
    idPerson: { type: [Number, String], required: false, default: null },
  },
  setup(props) {
    const router = useRouter();
    const route = useRoute();

    const state = reactive({
      isCreate: true,
      isEditing: true,
      isSubmitting: false,
      formData: {
        idPerson: '',
        name: '',
        surname: '',
        email: '',
        telNumber: '',
        passport: '',
        idGuestStatus: '',
      },
      statusTitle: '',
      guestStatuses: [],
      errors: {},
    });

    const onlyLetters = (value) => typeof value === 'string' && /^[\p{L}]+$/u.test(value);

    const rules = {
      formData: {
        name: { required, maxLength: maxLength(20), onlyLetters },
        surname: { required, maxLength: maxLength(20), onlyLetters },
        email: { required, email: emailV },
        telNumber: { required, maxLength: maxLength(15), numeric },
        passport: { required, maxLength: maxLength(10) },
        idGuestStatus: { required },
      },
    };

    const v$ = useVuelidate(rules, state);

    const toId = (val) => {
      const n = Number(val);
      return Number.isFinite(n) && n > 0 ? n : 0;
    };

    const fieldError = (...keys) => {
      for (const k of keys) {
        const arr = state.errors?.[k];
        if (Array.isArray(arr) && arr.length) return arr[0];
      }
      return '';
    };

    function mapFromApi(api) {
      return {
        idPerson: api?.idPerson ?? api?.IdPerson ?? '',
        name: String(api?.name ?? api?.Name ?? '').trim(),
        surname: String(api?.surname ?? api?.Surname ?? '').trim(),
        email: String(api?.email ?? api?.Email ?? '').trim(),
        telNumber: String(api?.telNumber ?? api?.TelNumber ?? '').trim(),
        passport: String(api?.passport ?? api?.Passport ?? '').trim(),
        idGuestStatus: String(api?.idGuestStatus ?? api?.IdGuestStatus ?? ''),
      };
    }

    function mapToApi(ui) {
      return {
        idPerson: ui.idPerson,
        Name: String(ui.name ?? '').trim(),
        Surname: String(ui.surname ?? '').trim(),
        Email: String(ui.email ?? '').trim(),
        TelNumber: String(ui.telNumber ?? '').trim(),
        Passport: String(ui.passport ?? '').trim(),
        IdGuestStatus: String(ui.idGuestStatus),
      };
    }

    async function loadStatuses() {
      try {
        const dto = await guests.status();
        state.guestStatuses = dto?.data ?? dto ?? [];
        updateStatusTitle();
      } catch (error) {
        notify({ title: 'Load failed', text: error?.message || 'Failed to load guest statuses', type: 'error' });
      }
    }

    function updateStatusTitle() {
      const found = state.guestStatuses.find((s) => String(s.idStatus) === String(state.formData.idGuestStatus));
      state.statusTitle = found?.title || '';
    }

    async function fetchGuest(id) {
      if (!id) return;
      try {
        let dto;
        if (typeof guests.getPerson === 'function') dto = await guests.getPerson(id);
        else if (typeof guests.get === 'function') dto = await guests.get(id);
        else throw new Error('Guest get method not found');

        const data = dto?.data ?? dto;
        state.formData = mapFromApi(data);
        updateStatusTitle();
      } catch (error) {
        notify({ title: 'Load failed', text: error?.message || 'Failed to load guest', type: 'error' });
        window.history.length > 1 ? history.back() : (location.href = '/guests');
      }
    }

    async function createGuest() {
      state.errors = {};
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        state.isSubmitting = true;
        const payload = mapToApi(state.formData);
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

    async function saveUpdate() {
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        state.isSubmitting = true;
        state.errors = {};
        const payload = mapToApi(state.formData);
        const res = await guests.update(payload);

        if (res?.httpStatusCode && res.httpStatusCode !== 200) {
          state.errors = res?.errors || {};
          notify({ title: 'Update failed', text: res?.message || 'Validation failed', type: 'error' });
          return;
        }

        notify({ title: 'Guest Updated', text: 'The guest has been successfully updated.', type: 'success', duration: 4000 });
        state.isEditing = false;
        updateStatusTitle();
      } catch (err) {
        const backendErrors = err?.response?.data?.errors || err?.details;
        if (backendErrors) state.errors = backendErrors;
        notify({ title: 'Update failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
      } finally {
        state.isSubmitting = false;
      }
    }

    function onSubmit() {
      if (state.isCreate) {
        createGuest();
      } else if (!state.isEditing) {
        state.isEditing = true;
      } else {
        saveUpdate();
      }
    }

    const init = () => {
      const idFromProps = toId(props.idPerson);
      const idFromRoute = toId(route.params?.idPerson);
      const resolvedId = idFromProps || idFromRoute;
      state.isCreate = !resolvedId;
      state.isEditing = state.isCreate;
      state.formData.idPerson = resolvedId;

      loadStatuses().then(() => {
        if (!state.isCreate) fetchGuest(resolvedId);
      });
    };

    onMounted(init);
    watch(() => [props.idPerson, route.params?.idPerson], init);
    watch(() => [state.formData.idGuestStatus, state.guestStatuses], updateStatusTitle, { deep: true });

    return { state, v$, onSubmit, fieldError };
  },
};
</script>

<style scoped>
.newRoom-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
  min-height: 100vh;
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

.input[readonly] {
  background: #f3f3f3;
  color: #666;
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
