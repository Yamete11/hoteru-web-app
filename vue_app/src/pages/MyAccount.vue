<template>
  <div class="newRoom-component">
    <navbar />
    <sidebar />

    <div class="main">
      <form @submit.prevent="toggleEdit" class="creating-form">
        <h1>Account Details</h1>

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

        <div class="input-form">
          <label>Type: </label>

          <select
              v-if="canChangeType"
              v-model="state.formData.idUserType"
              class="input"
              @change="v$.formData.idUserType.$touch(); refreshTypeTitle()"
              data-testid="select-user-type"
          >
            <option disabled value="">Select type</option>
            <option
                v-for="type in filteredUserTypes"
                :key="type.idType"
                :value="String(type.idType)"
                :data-testid="'option-type-' + type.idType"
            >
              {{ type.title }}
            </option>
          </select>

          <input
              v-else
              class="input"
              type="text"
              :value="state.typeTitle"
              data-testid="input-type-readonly"
              readonly
          />

          <span class="error-message" v-if="v$.formData.idUserType.$error">
            <span v-if="!v$.formData.idUserType.required.$response">Type is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.IdUserType">{{ state.errors.IdUserType?.[0] }}</span>
          <span class="error-message" v-if="state.errors.Permission">{{ state.errors.Permission }}</span>
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
import { reactive, onMounted, computed, watch } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, maxLength, email as emailV } from '@vuelidate/validators';
import { useStore } from 'vuex';
import { useRoute } from 'vue-router';
import { notify } from '@kyvg/vue3-notification';
import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

export default {
  name: 'Settings',
  setup() {
    const store = useStore();
    const route = useRoute();

    const state = reactive({
      isEditing: false,
      isSubmitting: false,
      formData: {
        idPerson: 0,
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

    const currentRole = computed(() => String(store.getters.getUserRole || store.getters.getUserData?.userType || ''));
    const isSuperadmin = computed(() => currentRole.value.trim().toLowerCase() === 'superadmin');

    const targetId = computed(() => {
      const fromRoute = route.params.id ? Number(route.params.id) : null;
      return fromRoute ?? store.getters.getPersonId ?? null;
    });

    const myPersonId = computed(() => Number(store.getters.getPersonId || store.getters.getUserData?.idPerson || 0));
    const isEditingOwn = computed(() => Number(targetId.value) === Number(myPersonId.value));

    const onlyLetters = (v) => typeof v === 'string' && /^[\p{L}]+$/u.test(v);
    const loginValid  = (v) => typeof v === 'string' && /^[A-Za-z0-9._-]+$/.test(v);

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
      const d = api.data ?? api ?? {};
      return {
        idPerson: Number(d?.idPerson ?? d?.idUser ?? 0),
        idUser: Number(d?.idUser ?? d?.idPerson ?? 0),
        name: String(d?.name ?? '').trim(),
        surname: String(d?.surname ?? '').trim(),
        email: String(d?.email ?? '').trim(),
        loginName: String(d?.loginName ?? '').trim(),
        idUserType: String(d?.idUserType ?? ''),
      };
    };

    const mapToApi = (ui) => ({
      IdPerson: Number(ui.idPerson || targetId.value),
      Name: String(ui.name ?? '').trim(),
      Surname: String(ui.surname ?? '').trim(),
      Email: String(ui.email ?? '').trim(),
      LoginName: String(ui.loginName ?? '').trim(),
      IdUserType: Number(ui.idUserType),
    });

    async function loadUserTypes() {
      const { data } = await http.get(ENDPOINTS.USER_TYPE);
      state.userTypes = data?.data ?? data ?? [];
    }

    const findTypeById = (id) => state.userTypes.find(t => String(t.idType) === String(id));
    const isTypeSuperadmin = (typeId) => (findTypeById(typeId)?.title || '').trim().toLowerCase() === 'superadmin';

    const isTargetSuperadmin = computed(() => isTypeSuperadmin(state.formData.idUserType));
    const canChangeType = computed(() =>
        state.isEditing && isSuperadmin.value && !isEditingOwn.value && !isTargetSuperadmin.value
    );

    const filteredUserTypes = computed(() =>
        state.userTypes.filter(t => String(t.title).trim().toLowerCase() !== 'superadmin')
    );

    function refreshTypeTitle() {
      state.typeTitle = findTypeById(state.formData.idUserType)?.title || '';
    }

    async function fetchUserFull() {
      const id = targetId.value;
      if (!id) {
        notify({ title: 'No user', text: 'User id is missing', type: 'warn' });
        return;
      }
      const { data } = await http.get(ENDPOINTS.USER.FULL(id));
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

      if (isSuperadmin.value && !isEditingOwn.value && isTypeSuperadmin(state.formData.idUserType)) {
        state.errors.Permission = 'You are not allowed to assign Superadmin role.';
        notify({ title: 'Permission denied', text: 'Cannot assign Superadmin.', type: 'error' });
        return;
      }


      try {
        state.isSubmitting = true;
        const payload = mapToApi(state.formData);
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
        state.errors = err?.response?.data?.errors || err?.details || {};
        notify({ title: 'Update failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
      } finally {
        state.isSubmitting = false;
      }
    }

    watch(() => state.formData.idUserType, refreshTypeTitle);
    onMounted(async () => {
      await loadUserTypes();
      await fetchUserFull();
    });

    return {
      state,
      v$,
      toggleEdit,
      refreshTypeTitle,
      isSuperadmin,
      canChangeType,
      filteredUserTypes,
    };
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

.input[readonly] {
  background: #f3f3f3;
  color: #666;
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
.input-form select{
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

.error-message {
  color: red;
  margin: 10px 0;
}
</style>
