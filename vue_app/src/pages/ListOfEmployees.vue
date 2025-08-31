<template>
  <div class="newRoom-component">
    <navbar />
    <sidebar />

    <div class="main">
      <div class="newUser-section">
        <form @submit.prevent="addUser" class="creating-form">
          <h1>Add new user</h1>

          <div class="input-form">
            <label>Name: </label>
            <input
                v-model="state.newUser.name"
                class="input"
                type="text"
                placeholder="Enter name"
                @input="v$.newUser.name.$touch()"
                data-testid="input-name"
            />
            <span class="error-message" v-if="v$.newUser.name.$error">
              <span v-if="!v$.newUser.name.required.$response">Name is required*</span>
              <span v-else-if="!v$.newUser.name.maxLength.$response">Name must be less than 20 characters*</span>
              <span v-else-if="!v$.newUser.name.onlyLetters.$response">Name must contain only letters*</span>
            </span>
            <span class="error-message" v-if="state.errors.Name">{{ state.errors.Name?.[0] }}</span>
          </div>

          <div class="input-form">
            <label>Surname: </label>
            <input
                v-model="state.newUser.surname"
                class="input"
                type="text"
                placeholder="Enter surname"
                @input="v$.newUser.surname.$touch()"
                data-testid="input-surname"
            />
            <span class="error-message" v-if="v$.newUser.surname.$error">
              <span v-if="!v$.newUser.surname.required.$response">Surname is required*</span>
              <span v-else-if="!v$.newUser.surname.maxLength.$response">Surname must be less than 20 characters*</span>
              <span v-else-if="!v$.newUser.surname.onlyLetters.$response">Surname must contain only letters*</span>
            </span>
            <span class="error-message" v-if="state.errors.Surname">{{ state.errors.Surname?.[0] }}</span>
          </div>

          <div class="input-form">
            <label>Email: </label>
            <input
                v-model="state.newUser.email"
                class="input"
                type="text"
                placeholder="Enter email"
                @input="v$.newUser.email.$touch()"
                data-testid="input-email"
            />
            <span class="error-message" v-if="v$.newUser.email.$error">
              <span v-if="!v$.newUser.email.required.$response">Email is required*</span>
              <span v-else-if="!v$.newUser.email.email.$response">Invalid email format*</span>
            </span>
            <span class="error-message" v-if="state.errors.Email">{{ state.errors.Email?.[0] }}</span>
          </div>

          <div class="input-form">
            <label>Login: </label>
            <input
                v-model="state.newUser.loginName"
                class="input"
                type="text"
                placeholder="Enter login"
                @input="v$.newUser.loginName.$touch()"
                data-testid="input-login"
            />
            <span class="error-message" v-if="v$.newUser.loginName.$error">
              <span v-if="!v$.newUser.loginName.required.$response">Login is required*</span>
              <span v-else-if="!v$.newUser.loginName.minLength.$response">Login must be at least 3 characters*</span>
              <span v-else-if="!v$.newUser.loginName.maxLength.$response">Login must be less than 15 characters*</span>
              <span v-else-if="!v$.newUser.loginName.loginValid.$response">Only letters, digits, dot, dash and underscore*</span>
            </span>
            <span class="error-message" v-if="state.errors.LoginName">{{ state.errors.LoginName?.[0] }}</span>
          </div>

          <div class="input-form">
            <label>Password: </label>
            <input
                v-model="state.newUser.password"
                class="input"
                type="password"
                placeholder="Enter password"
                @input="v$.newUser.password.$touch()"
                data-testid="input-password"
            />
            <span class="error-message" v-if="v$.newUser.password.$error">
              <span v-if="!v$.newUser.password.required.$response">Password is required*</span>
              <span v-else-if="!v$.newUser.password.minLength.$response">Password must be at least 3 characters*</span>
            </span>
            <span class="error-message" v-if="state.errors.Password">{{ state.errors.Password?.[0] }}</span>
          </div>

          <div class="input-form">
            <label>Type: </label>
            <select
                v-model="state.newUser.idUserType"
                class="input"
                @change="v$.newUser.idUserType.$touch()"
                data-testid="select-user-type"
            >
              <option disabled value="">Select type</option>
              <option
                  v-for="type in filteredUserTypes"
                  :key="type.idType"
                  :value="String(type.idType)"
              >
                {{ type.title }}
              </option>
            </select>
            <span class="error-message" v-if="v$.newUser.idUserType.$error">
              <span v-if="!v$.newUser.idUserType.required.$response">Type is required*</span>
            </span>
            <span class="error-message" v-if="state.errors.IdUserType">{{ state.errors.IdUserType?.[0] }}</span>
          </div>

          <div class="registration-class">
            <button type="button" class="registration-btn" @click="clearNewUserForm" data-testid="clean-button">Clean</button>
            <button type="submit" class="registration-btn" data-testid="add-user-button">Add User</button>
          </div>
        </form>

        <div class="user-list">
          <h1>List of employees:</h1>

          <div class="users-card">
            <ul class="users">
              <li
                  class="element"
                  v-for="user in state.users"
                  :key="user.idPerson"
                  :data-testid="'user-item-' + user.loginName"
              >
                <div class="info">
                  <div class="line">
                    <span class="label">Login</span>
                    <span class="value">{{ user.loginName }}</span>
                  </div>
                  <div class="line">
                    <span class="label">Type</span>
                    <span class="badge" :class="'badge--' + (user.userType || '').toLowerCase()">
                      {{ user.userType }}
                    </span>
                  </div>
                </div>

                <div class="actions">
                  <router-link
                      v-if="canViewDetails(user)"
                      class="btn"
                      :to="`/user/${user.idPerson}`"
                      :data-testid="'details-user-' + user.loginName"
                  >
                    Details
                  </router-link>
                  <button
                      v-else
                      class="btn btn--disabled"
                      disabled
                      :title="'Only Superadmin can view Superadmin details'"
                      :data-testid="'details-user-disabled-' + user.loginName"
                  >
                    Details
                  </button>

                  <button
                      class="btn"
                      :class="{ 'btn--disabled': !canDelete(user) }"
                      :disabled="!canDelete(user)"
                      @click.prevent="canDelete(user) && deleteUser(user)"
                      :data-testid="'delete-user-' + user.loginName"
                  >
                    Remove
                  </button>
                </div>
              </li>
            </ul>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { reactive, computed, onMounted } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, maxLength, email as emailV, minLength } from '@vuelidate/validators';
import { useStore } from 'vuex';
import { notify } from '@kyvg/vue3-notification';
import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

export default {
  name: 'ListOfEmployees',
  setup() {
    const store = useStore();

    const state = reactive({
      newUser: {
        name: '',
        surname: '',
        email: '',
        loginName: '',
        password: '',
        idUserType: '',
      },
      userTypes: [],
      errors: {},
      users: [],
    });

    const currentRole = computed(() => String(store.getters.getUserRole || store.getters.getUserData?.userType || ''));
    const isSuperadmin = computed(() => currentRole.value.trim().toLowerCase() === 'superadmin');

    const onlyLetters = (v) => typeof v === 'string' && /^\p{L}+(?:[ '-]\p{L}+)*$/u.test(v);
    const loginValid  = (v) => typeof v === 'string' && /^[A-Za-z0-9._-]{3,15}$/.test(v);

    const rules = {
      newUser: {
        name: { required, maxLength: maxLength(20), onlyLetters },
        surname: { required, maxLength: maxLength(20), onlyLetters },
        email: { required, email: emailV },
        loginName: { required, minLength: minLength(3), maxLength: maxLength(15), loginValid },
        password: { required, minLength: minLength(3) },
        idUserType: { required },
      },
    };
    const v$ = useVuelidate(rules, state);

    const filteredUserTypes = computed(() =>
        (state.userTypes || []).filter(t => (t.title || '').toLowerCase() !== 'superadmin')
    );

    const currentUserId = computed(() => store.getters.getPersonId);

    async function loadUserTypes() {
      const { data } = await http.get(ENDPOINTS.USER_TYPE);
      state.userTypes = data?.data ?? data ?? [];
    }

    async function fetchUsers() {
      const { data } = await http.get(ENDPOINTS.USER.ROOT);
      const rows = data?.data ?? data ?? [];
      state.users = rows.map(u => ({
        idPerson: u.idPerson ?? u.idUser ?? 0,
        loginName: u.loginName ?? u.login ?? '',
        userType:  u.userType ?? '',
      }));
    }

    const canDelete = (user) => {
      const isTargetSuperadmin = (user.userType || '').toLowerCase() === 'superadmin';
      const isSelf = user.idPerson === currentUserId.value;
      return !isTargetSuperadmin && !isSelf;
    };

    const canViewDetails = (user) => {
      const isTargetSuperadmin = (user.userType || '').toLowerCase() === 'superadmin';
      if (!isSuperadmin.value && isTargetSuperadmin) return false;
      return true;
    };

    async function addUser() {
      state.errors = {};
      v$.value.$touch();
      if (v$.value.$error) return;

      const payload = {
        Name: String(state.newUser.name).trim(),
        Surname: String(state.newUser.surname).trim(),
        Email: String(state.newUser.email).trim(),
        LoginName: String(state.newUser.loginName).trim(),
        Password: String(state.newUser.password),
        IdUserType: Number(state.newUser.idUserType),
      };

      try {
        const { data } = await http.post(ENDPOINTS.USER.ROOT, payload);

        if (data?.httpStatusCode && ![200, 201].includes(data.httpStatusCode)) {
          state.errors = data.errors || {};
          notify({ title: 'Create failed', text: data?.message || 'Validation failed', type: 'error' });
          return;
        }

        notify({ title: 'User Created', text: 'User has been successfully created.', type: 'success', duration: 3000 });
        await fetchUsers();
        clearNewUserForm();
      } catch (err) {
        state.errors = err?.response?.data?.errors || {};
        notify({ title: 'Create failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
      }
    }

    async function deleteUser(user) {
      if (!canDelete(user)) {
        notify({ title: 'Not allowed', text: 'You cannot delete yourself or a Superadmin.', type: 'warn' });
        return;
      }
      try {
        const { data } = await http.delete(ENDPOINTS.USER.BY_ID(user.idPerson));
        if (data?.httpStatusCode && data.httpStatusCode !== 200) {
          notify({ title: 'Delete failed', text: data?.message || 'Operation failed', type: 'error' });
          return;
        }
        state.users = state.users.filter(u => u.idPerson !== user.idPerson);
        notify({ title: 'User Deleted', text: 'User has been successfully deleted.', type: 'success', duration: 3000 });
      } catch (err) {
        notify({ title: 'Delete failed', text: err?.response?.data?.message || err?.message || 'Unexpected error', type: 'error' });
      }
    }

    function clearNewUserForm() {
      state.newUser = {
        name: '',
        surname: '',
        email: '',
        loginName: '',
        password: '',
        idUserType: '',
      };
      v$.value.newUser.$reset();
      state.errors = {};
    }

    onMounted(async () => {
      await loadUserTypes();
      await fetchUsers();
    });

    return {
      state, v$, filteredUserTypes,
      addUser, deleteUser, clearNewUserForm,
      currentUserId, canDelete,
      currentRole, isSuperadmin, canViewDetails,
    };
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
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 80px);
  padding: 32px 24px;
  margin: 0;
}

.newUser-section {
  display: grid;
  grid-template-columns: minmax(420px, 1fr) minmax(420px, 1fr);
  column-gap: 48px;
  row-gap: 24px;
  width: min(1100px, 100%);
  margin: 0 auto;
  align-items: start;
}

.creating-form {
  width: 100%;
  background: #fff;
  border-radius: 12px;
  padding: 24px 20px;
  box-shadow: 0 8px 20px rgba(0,0,0,.08);
  display: flex;
  flex-direction: column;
  gap: 14px;
}

h1 {
  display: flex;
  justify-content: center;
  color: #2a2a2a;
  margin: 0 0 8px;
}

.input-form {
  display: flex;
  flex-direction: column;
  margin: 5px 0;
}

.input-form label {
  margin-bottom: 6px;
  font-weight: 700;
  color: #2a2a2a;
}

.input-form input[type="text"],
.input-form input[type="password"],
.input-form select {
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background: #fff;
  color: #111;
}

.input-form select {
  cursor: pointer;
}

.error-message {
  color: #cc1f1a;
  margin: 6px 0;
  font-size: 0.9rem;
}

.registration-class{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.registration-btn{
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px 16px;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-weight: bold;
  color: white;
  cursor: pointer;
}

.user-list {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.user-list > h1 { margin: 0 0 10px; }

.users-card {
  width: 100%;
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 8px 20px rgba(0,0,0,0.08);
  padding: 14px;
  border: 1px solid rgba(0,0,0,0.06);
}

.users {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-height: 520px;
  overflow-y: auto;
}

.element {
  display: grid;
  grid-template-columns: 1fr 240px;
  gap: 12px;
  align-items: center;
  padding: 12px 14px;
  background-color: #f8f5f2;
  border: 1px solid rgba(0,0,0,0.06);
  border-radius: 10px;
  transition: box-shadow .2s ease, transform .2s ease, background-color .2s ease;
}

.element:hover {
  background-color: #f5efe9;
  box-shadow: 0 4px 12px rgba(0,0,0,0.08);
}

.info { display: grid; gap: 6px; }
.line { display: flex; align-items: center; gap: 8px; }
.label { color: #6b6b6b; font-weight: 600; min-width: 48px; }
.value { color: #2c2c2c; font-weight: 700; }

.badge {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 700;
  background: #e9e3dc;
  color: #5a4b3a;
  border: 1px solid rgba(0,0,0,0.06);
}

.badge--admin { background:#ffe9d9; color:#8a3d00; }
.badge--employee { background:#e7f3ff; color:#004a92; }
.badge--superadmin { background:#ffe4e4; color:#b83232; }

.actions {
  display: grid;
  grid-template-columns: 120px 120px;
  gap: 10px;
  justify-items: end;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 36px;
  padding: 0 12px;
  font-size: 0.85rem;
  font-weight: 700;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.2s ease, opacity 0.2s ease, transform 0.05s ease;
  text-decoration: none;
  box-sizing: border-box;
}

.btn:link,
.btn:visited,
.btn:hover,
.btn:active,
.btn:focus {
  text-decoration: none;
  color: #FFFFFF;
}

.btn:hover { background-color: #3a3a3a; }
.btn:active { transform: translateY(1px); }

.btn--disabled,
.btn:disabled {
  background-color: #9a9a9a;
  border-color: #bdbdbd;
  color: #ececec;
  cursor: not-allowed;
  opacity: 0.85;
}

@media (max-width: 980px) {
  .newUser-section {
    grid-template-columns: 1fr;
    column-gap: 0;
    row-gap: 24px;
    width: min(720px, 100%);
  }
  .main {
    align-items: flex-start;
    min-height: auto;
  }
}
</style>
