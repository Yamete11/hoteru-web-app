<template>
  <div class="newRoom-component">
    <notifications position="top right" />
    <navbar />
    <sidebar />

    <div class="main">
      <form @submit.prevent="toggleEdit" class="creating-form">
        <h1>Account Details</h1>

        <!-- Name -->
        <div class="input-form">
          <label>Name: </label>
          <input
              v-model="state.formData.name"
              class="input"
              type="text"
              placeholder="Enter name"
              :readonly="!state.isEditing"
              @input="v$.formData.name.$touch()"
              data-testid="input-name"
          />
          <span class="error-message" v-if="v$.formData.name.$error">
            <span v-if="!v$.formData.name.required.$response">Name is required*</span>
            <span v-else-if="!v$.formData.name.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.name.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Name">{{ state.errors.Name?.[0] }}</span>
        </div>

        <!-- Surname -->
        <div class="input-form">
          <label>Surname: </label>
          <input
              v-model="state.formData.surname"
              class="input"
              type="text"
              placeholder="Enter surname"
              :readonly="!state.isEditing"
              @input="v$.formData.surname.$touch()"
              data-testid="input-surname"
          />
          <span class="error-message" v-if="v$.formData.surname.$error">
            <span v-if="!v$.formData.surname.required.$response">Surname is required*</span>
            <span v-else-if="!v$.formData.surname.maxLength.$response">Surname must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.surname.onlyLetters.$response">Surname must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Surname">{{ state.errors.Surname?.[0] }}</span>
        </div>

        <!-- Email -->
        <div class="input-form">
          <label>Email: </label>
          <input
              v-model="state.formData.email"
              class="input"
              type="text"
              placeholder="Enter email"
              :readonly="!state.isEditing"
              @input="v$.formData.email.$touch()"
              data-testid="input-email"
          />
          <span class="error-message" v-if="v$.formData.email.$error">
            <span v-if="!v$.formData.email.required.$response">Email is required*</span>
            <span v-else-if="!v$.formData.email.email.$response">Invalid email format*</span>
          </span>
          <span class="error-message" v-if="state.errors.Email">{{ state.errors.Email?.[0] }}</span>
        </div>

        <!-- Login -->
        <div class="input-form">
          <label>Login: </label>
          <input
              v-model="state.formData.loginName"
              class="input"
              type="text"
              placeholder="Enter login"
              :readonly="!state.isEditing"
              @input="v$.formData.loginName.$touch()"
              data-testid="input-login"
          />
          <span class="error-message" v-if="v$.formData.loginName.$error">
            <span v-if="!v$.formData.loginName.required.$response">Login is required*</span>
            <span v-else-if="!v$.formData.loginName.maxLength.$response">Login must be less than 15 characters*</span>
            <span v-else-if="!v$.formData.loginName.loginValid.$response">Only letters, numbers, dot, dash and underscore*</span>
          </span>
          <span class="error-message" v-if="state.errors.LoginName">{{ state.errors.LoginName?.[0] }}</span>
        </div>

        <!-- Type -->
        <div class="input-form">
          <label>Type: </label>
          <input
              v-if="!state.isEditing"
              class="input"
              type="text"
              :value="state.typeTitle"
              data-testid="input-type-readonly"
              readonly
          />
          <select
              v-else
              v-model="state.formData.idUserType"
              class="input"
              @change="v$.formData.idUserType.$touch()"
              data-testid="select-user-type"
          >
            <option disabled value="">Select type</option>
            <option
                v-for="type in state.userTypes"
                :key="type.idType"
                :value="String(type.idType)"
                :data-testid="'option-type-' + type.idType"
            >
              {{ type.title }}
            </option>
          </select>
          <span class="error-message" v-if="v$.formData.idUserType.$error">
            <span v-if="!v$.formData.idUserType.required.$response">Type is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.IdUserType">{{ state.errors.IdUserType?.[0] }}</span>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" to="/arrivals" data-testid="button-back">Back</router-link>
          <button class="registration-btn" type="submit" :disabled="state.isSubmitting" data-testid="button-submit">
            {{ state.isEditing ? (state.isSubmitting ? 'Saving...' : 'Save') : 'Edit' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { reactive, onMounted } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, maxLength, email as emailV } from '@vuelidate/validators';
import { useStore } from 'vuex';
import { notify } from '@kyvg/vue3-notification';
import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

export default {
  name: 'Settings',
  setup() {
    const store = useStore();

    const state = reactive({
      isEditing: false,
      isSubmitting: false,
      formData: {
        idUser: 0,
        name: '',
        surname: '',
        email: '',
        loginName: '',
        idUserType: '',
      },
      userTypes: [],
      typeTitle: '',
      errors: {},
    });

    const onlyLetters = (v) => typeof v === 'string' && /^[\p{L}]+$/u.test(v);
    const loginValid = (v) => typeof v === 'string' && /^[A-Za-z0-9._-]+$/.test(v);

    const rules = {
      formData: {
        name: { required, maxLength: maxLength(20), onlyLetters },
        surname: { required, maxLength: maxLength(20), onlyLetters },
        email: { required, email: emailV },
        loginName: { required, maxLength: maxLength(15), loginValid },
        idUserType: { required },
      },
    };
    const v$ = useVuelidate(rules, state);

    const mapFromApi = (api) => {
      const d = api.data ?? api;
      return {
        idUser: d?.idUser ?? 0,
        name: String(d?.name ?? '').trim(),
        surname: String(d?.surname ?? '').trim(),
        email: String(d?.email ?? '').trim(),
        loginName: String(d?.loginName ?? '').trim(),
        idUserType: String(d?.idUserType ?? ''),
      };
    };

    const mapToApi = (ui) => ({
      IdPerson: Number(ui.idPerson ?? store.getters.getPersonId),
      Name: String(ui.name ?? '').trim(),
      Surname: String(ui.surname ?? '').trim(),
      Email: String(ui.email ?? '').trim(),
      LoginName: String(ui.loginName ?? '').trim(),
      IdUserType: Number(ui.idUserType),
    });

    async function loadUserTypes() {
      const { data } = await http.get(ENDPOINTS.USER_TYPE);
      state.userTypes = data.data ?? [];
    }

    function refreshTypeTitle() {
      const found = state.userTypes.find(t => String(t.idType) === String(state.formData.idUserType));
      state.typeTitle = found ? found.title : '';
    }

    async function fetchUserFull() {
      const idUser = store.getters.getUserData?.idUser;
      if (!idUser) return;
      const { data } = await http.get(ENDPOINTS.USER.FULL(idUser));
      state.formData = mapFromApi(data);
      refreshTypeTitle();
    }

    async function toggleEdit() {
      if (!state.isEditing) {
        state.isEditing = true;
        return;
      }
      state.errors = {};
      v$.value.$touch();
      if (v$.value.$error) return;

      try {
        state.isSubmitting = true;
        const payload = mapToApi(state.formData);
        console.log(payload);
        const { data } = await http.put(ENDPOINTS.USER.ROOT, payload);

        if (data?.httpStatusCode && data.httpStatusCode !== 200) {
          state.errors = data.errors || {};
          notify({ title: 'Update failed', text: data?.message || 'Validation failed', type: 'error' });
          return;
        }

        notify({ title: 'User Updated', text: 'User data was successfully updated.', type: 'success', duration: 3000 });
        state.isEditing = false;
        refreshTypeTitle();

        const current = store.getters.getUserData || {};
        store.commit('setUserData', { ...current, loginName: state.formData.loginName });
      } catch (err) {
        const errors = err?.response?.data?.errors || err?.details;
        if (errors) state.errors = errors;
        notify({ title: 'Update failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
      } finally {
        state.isSubmitting = false;
      }
    }

    onMounted(async () => {
      await loadUserTypes();
      await fetchUserFull();
    });

    return { state, v$, toggleEdit };
  },
};
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
  align-items: flex-start;
  flex-direction: row;
  justify-content: space-around;
  flex-wrap: wrap;
  flex-grow: 1;
  padding-top: 8vh;
  margin: 5%;
}

.newUser-section{
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  justify-content: space-around;
  min-width: 40%;
}

.creating-form {
  width: 100%;
  max-width: 300px;
}

.user-list{
  min-width: 50%;
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

.input-form input[type="text"],
.input-form input[type="password"]{
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

.element {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  margin: 10px 0;
  background-color: #C8B6A6;
  border-radius: 5px;
  font-weight: bold;
  font-size: 15px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.service-list ul {
  list-style-type: none;
  padding: 0;
  margin: 0;
  max-width: 800px;
}

.service-list {
  max-height: 200px;
  overflow-y: auto;
  width: 100%;
  border: 1px solid black;
  border-radius: 5px;
  padding: 10px;
  margin-bottom: 10px;
}

.service-label {
  font-weight: bold;
  color: black;
  align-self: flex-start;
  margin-bottom: 5px;
}

.btn {
  padding: 0.3rem 0.8rem;
  font-size: 0.8rem;
  font-weight: bold;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

</style>