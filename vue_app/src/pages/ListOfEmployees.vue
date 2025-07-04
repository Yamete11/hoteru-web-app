<template>
  <div class="newRoom-component">
    <notifications position="top right" />
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">

      <div class="newUser-section">
        <form @submit.prevent="addUser" class="creating-form">
          <h1>Add new user</h1>
          <div class="input-form">
            <label>Name: </label>
            <input v-model="state.newUser.name"
                   class="input"
                   type="text"
                   placeholder="Enter name"
                   @input="v$.newUser.name.$touch()"
                   data-testid="input-name"
            >
            <span class="error-message" v-if="v$.newUser.name.$error">
            <span v-if="!v$.newUser.name.required.$response">Name is required*</span>
            <span v-else-if="!v$.newUser.name.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.newUser.name.onlyLetters.$response">Name must contain only letters*</span>
          </span>
            <span class="error-message" v-if="state.errors.Name">{{ state.errors.Name[0] }}</span>
          </div>

          <div class="input-form">
            <label>Surname: </label>
            <input v-model="state.newUser.surname"
                   class="input"
                   type="text"
                   placeholder="Enter surname"
                   @input="v$.newUser.surname.$touch()"
                   data-testid="input-surname"
            >
            <span class="error-message" v-if="v$.newUser.surname.$error">
            <span v-if="!v$.newUser.surname.required.$response">Surname is required*</span>
            <span v-else-if="!v$.newUser.surname.maxLength.$response">Surname must be less than 20 characters*</span>
            <span v-else-if="!v$.newUser.surname.onlyLetters.$response">Name must contain only letters*</span>
          </span>
            <span class="error-message" v-if="state.errors.Surname">{{ state.errors.Surname[0] }}</span>
          </div>

          <div class="input-form">
            <label>Email: </label>
            <input v-model="state.newUser.email"
                   class="input"
                   type="text"
                   placeholder="Enter email"
                   @input="v$.newUser.email.$touch()"
                   data-testid="input-email"
            >
            <span class="error-message" v-if="v$.newUser.email.$error">
            <span v-if="!v$.newUser.email.required.$response">Email is required*</span>
            <span v-if="!v$.newUser.email.email.$response">Invalid email format*</span>
          </span>
            <span class="error-message" v-if="state.errors.Email">{{ state.errors.Email[0] }}</span>
          </div>

          <div class="input-form">
            <label>Login: </label>
            <input v-model="state.newUser.loginName"
                   class="input"
                   type="text"
                   placeholder="Enter login"
                   @input="v$.newUser.loginName.$touch()"
                   data-testid="input-login"
            >
            <span class="error-message" v-if="v$.newUser.loginName.$error">
            <span v-if="!v$.newUser.loginName.required.$response">Login is required*</span>
            <span v-else-if="!v$.newUser.loginName.maxLength.$response">Login must be less than 20 characters*</span>
            <span v-else-if="!v$.newUser.loginName.onlyLetters.$response">Login must contain only letters*</span>
          </span>
            <span class="error-message" v-if="state.errors.Login">{{ state.errors.Login[0] }}</span>
          </div>

          <div class="input-form">
            <label>Password: </label>
            <input v-model="state.newUser.password"
                   class="input"
                   type="password"
                   placeholder="Enter password"
                   @input="v$.newUser.password.$touch()"
                   data-testid="input-password"
            >
            <span class="error-message" v-if="v$.newUser.password.$error">
          <span v-if="!v$.newUser.password.required.$response">Password is required*</span>
        </span>
            <span class="error-message" v-if="state.errors.Password">{{ state.errors.Password[0] }}</span>
          </div>

          <div class="input-form">
            <label>Type: </label>
            <select v-model="state.newUser.idUserType" class="input" @change="v$.newUser.idUserType.$touch()" data-testid="select-user-type">
            <option disabled value="">Select type</option>
              <option v-for="type in filteredUserTypes" :value="type.idType" :key="type.idType">{{ type.title }}</option>
            </select>
            <span class="error-message" v-if="v$.newUser.idUserType.$error">
            <span v-if="!v$.newUser.idUserType.required.$response">Type is required*</span>
          </span>
            <span class="error-message" v-if="state.errors.idUserType">{{ state.errors.idUserType[0] }}</span>
          </div>

          <div class="registration-class">
            <button type="button" class="registration-btn" @click="clearNewUserForm" data-testid="clean-button">Clean</button>
            <button type="submit" class="registration-btn" data-testid="add-user-button">Add User</button>
          </div>
        </form>

        <div class="user-list">
          <h1>List of employees: </h1>
          <div class="service-list">
            <ul class="added-services-list">
              <li class="element" v-for="user in state.users" :key="user.idPerson" :data-testid="'user-item-' + user.loginName">
              <span>Login: {{ user.loginName }} - Type: {{ user.userType }}</span>
                <button v-if="user.userType !== 'Superadmin'" class="btn" @click.prevent="deleteUser(user.idPerson)" :data-testid="'delete-user-' + user.loginName">
                Remove
                </button>
              </li>

            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue } from '@vuelidate/validators';
import axios from 'axios';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import {computed, reactive} from "vue";
import {email} from "@vuelidate/validators";
import { notify } from '@kyvg/vue3-notification';


export default {
  name: "ListOfEmployees",
  setup(){
    const store = useStore();
    const router = useRouter();
    const state = reactive({
      newUser:{
        name: '',
        surname: '',
        email: '',
        loginName: '',
        password: '',
        idUserType: ''
      },
      newUserTypeTitle: '',
      userTypes: [],
      typeTitle: '',
      errors: {},
      users: []
    });

    function onlyLetters(value) {
      return /^[A-Za-z]+$/.test(value);
    }

    const rules = {
      newUser: {
        name: { required, maxLength: maxLength(20), onlyLetters },
        surname: { required, maxLength: maxLength(20), onlyLetters },
        email: { required, email },
        loginName: { required, maxLength: maxLength(15), onlyLetters },
        password: { required },
        idUserType: { required }
      }
    }

    const filteredUserTypes = computed(() => {
      return state.userTypes.filter(type => type.title !== 'Superadmin');
    });

    const v$ = useVuelidate(rules, state);


    async function fetchUser(idUser){
      try {
        const response = await axios.get('https://localhost:44384/api/User/fullUser/' + idUser, {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.formData = response.data;

        const responseUserTypes = await axios.get('https://localhost:44384/api/UserType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.userTypes = responseUserTypes.data;


        const foundType = state.userTypes.find(status => status.idType === state.formData.idUserType);
        state.typeTitle = foundType ? foundType.title : 'Status not found';

        state.formData.idPerson = this.$store.getters.getUserData.idUser;
        console.log(state.formData)

      } catch (error) {
        console.error(error);
      }
    }

    async function fetchUsers() {
      try {
        const response = await axios.get('https://localhost:44384/api/User', {
          headers: {
            'Authorization': `Bearer ${store.getters.getToken}`
          }
        });
        state.users = response.data;
      } catch (error) {
        console.error('Error fetching users:', error);
      }
    }



    async function addUser(){
      console.log(state.newUser)
      v$.value.$touch();
      if (!v$.value.$error) {
        try {
          const response = await axios.post('https://localhost:44384/api/User', state.newUser, {
            headers: {
              'Authorization': `Bearer ${store.getters.getToken}`
            }
          });
          console.log('Response:', response.data);
          if (response.data && response.data.httpStatusCode === 200) {
            notify({
              title: 'User Created',
              text: 'User has been successfully created.',
              type: 'success',
              duration: 4000
            });
            await fetchUsers();

            clearNewUserForm();
          }

        } catch (error) {
          if (error.response && error.response.data && error.response.data.errors) {
            state.errors = error.response.data.errors;
          }
          console.log('Error', error);
        }
      }
    }

    async function deleteUser(idPerson) {
      try {
        await axios.delete(`https://localhost:44384/api/User/${idPerson}`, {
          headers: {
            'Authorization': `Bearer ${store.getters.getToken}`
          }
        });
        state.users = state.users.filter(user => user.idPerson !== idPerson);

        notify({
          title: 'User Deleted',
          text: 'User has been successfully deleted.',
          type: 'success',
          duration: 4000
        });

      } catch (error) {
        console.error('Error deleting user:', error);
      }
    }

    function clearNewUserForm() {
      state.newUser = {
        name: '',
        surname: '',
        email: '',
        loginName: '',
        password: '',
        idUserType: ''
      };
      v$.value.newUser.$reset();
    }


    return {state, fetchUser, addUser, v$, clearNewUserForm, fetchUsers, deleteUser, filteredUserTypes}
  },
  mounted(){
    this.fetchUser(this.$store.getters.getUserData.idUser);
    this.fetchUsers();
  }
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

.user-list {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 40%;
  margin-left: 10px;
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
  height: 200%;
  width: 100%;
  border: 1px solid black;
  border-radius: 5px;
  padding: 10px;
}

.service-label {
  font-weight: bold;
  color: black;
  align-self: center;
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