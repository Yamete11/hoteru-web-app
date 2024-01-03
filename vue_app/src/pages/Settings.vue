<template>
  <div class="newRoom-component">
  <navbar></navbar>
  <sidebar></sidebar>
  <div class="main">
    <h1>Account Details</h1>
    <form @submit.prevent="toggleEdit" class="creating-form">
      <div class="input-form">
        <label>Name: </label>
        <input v-model="state.formData.name"
               class="input"
               type="text"
               placeholder="Enter room number"
               :readonly="!state.isEditing"
               @input="v$.formData.name.$touch()"
        >
        <span class="error-message" v-if="v$.formData.name.$error">
            <span v-if="!v$.formData.name.required.$response">Name is required*</span>
            <span v-else-if="!v$.formData.name.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.name.onlyLetters.$response">Name must contain only letters*</span>
          </span>
        <span class="error-message" v-if="state.errors.Name">{{ state.errors.Name[0] }}</span>
      </div>

      <div class="input-form">
        <label>Surname: </label>
        <input v-model="state.formData.surname"
               class="input"
               type="text"
               placeholder="Enter room capacity"
               :readonly="!state.isEditing"
               @input="v$.formData.surname.$touch()"
        >
        <span class="error-message" v-if="v$.formData.surname.$error">
            <span v-if="!v$.formData.surname.required.$response">Surname is required*</span>
            <span v-else-if="!v$.formData.surname.maxLength.$response">Surname must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.surname.onlyLetters.$response">Name must contain only letters*</span>
          </span>
        <span class="error-message" v-if="state.errors.Surname">{{ state.errors.Surname[0] }}</span>
      </div>

      <div class="input-form">
        <label>Email: </label>
        <input v-model="state.formData.email"
               class="input"
               type="text"
               placeholder="Enter room price"
               :readonly="!state.isEditing"
               @input="v$.formData.email.$touch()"
        >
        <span class="error-message" v-if="v$.formData.email.$error">
            <span v-if="!v$.formData.email.required.$response">Email is required*</span>
            <span v-if="!v$.formData.email.email.$response">Invalid email format*</span>
          </span>
        <span class="error-message" v-if="state.errors.Email">{{ state.errors.Email[0] }}</span>
      </div>

      <div class="input-form">
        <label>Login: </label>
        <input v-model="state.formData.loginName"
               class="input"
               type="text"
               placeholder="Enter room price"
               :readonly="!state.isEditing"
               @input="v$.formData.loginName.$touch()"
        >
        <span class="error-message" v-if="v$.formData.loginName.$error">
            <span v-if="!v$.formData.loginName.required.$response">Name is required*</span>
            <span v-else-if="!v$.formData.loginName.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.loginName.onlyLetters.$response">Name must contain only letters*</span>
          </span>
        <span class="error-message" v-if="state.errors.LoginName">{{ state.errors.LoginName[0] }}</span>
      </div>

      <div class="input-form">
        <label>Type: </label>
        <input v-if="!state.isEditing" class="input" type="text" :value="state.typeTitle" readonly>
        <select v-else v-model="state.formData.idUserType" class="input" @change="v$.formData.idUserType.$touch()">
          <option v-for="type in state.userTypes" :value="type.idType" :key="type.idType">{{ type.title }}</option>
        </select>
        <span class="error-message" v-if="v$.formData.idUserType.$error">
            <span v-if="!v$.formData.idUserType.required.$response">Type is required*</span>
          </span>
        <span class="error-message" v-if="state.errors.idUserType">{{ state.errors.idUserType[0] }}</span>
      </div>


      <div class="registration-class">
        <router-link class="registration-btn" to="/arrivals">Back</router-link>
        <button type="submit" class="registration-btn">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
      </div>
    </form>

    <h1>Add new user</h1>
    <form @submit.prevent="addUser" class="creating-form">
      <div class="input-form">
        <label>Name: </label>
        <input v-model="state.newUser.name"
               class="input"
               type="text"
               placeholder="Enter name"
               @input="v$.newUser.name.$touch()"
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
        >
        <span class="error-message" v-if="v$.newUser.password.$error">
          <span v-if="!v$.newUser.password.required.$response">Password is required*</span>
        </span>
        <span class="error-message" v-if="state.errors.Password">{{ state.errors.Password[0] }}</span>
      </div>

      <div class="input-form">
        <label>Type: </label>
        <select v-model="state.newUser.idUserType" class="input"  @change="v$.newUser.idUserType.$touch()">
          <option disabled value="">Select type</option>
          <option v-for="type in state.userTypes" :value="type.idType" :key="type.idType">{{ type.title }}</option>
        </select>
        <span class="error-message" v-if="v$.newUser.idUserType.$error">
            <span v-if="!v$.newUser.idUserType.required.$response">Type is required*</span>
          </span>
        <span class="error-message" v-if="state.errors.idUserType">{{ state.errors.idUserType[0] }}</span>
      </div>

      <div class="registration-class">
        <button type="button" class="registration-btn" @click="clearNewUserForm">Clean</button>
        <button type="submit" class="registration-btn">Add User</button>
      </div>
    </form>

    <div>
      <label class="service-label">Services: </label>
      <div class="service-list">
        <ul>
          <li class="element" v-for="user in state.users" :key="user.idPerson">
            Login: {{ user.loginName }} - Type: {{ user.userType }} <button @click.prevent="removeUser(idPerson)">Remove</button>
          </li>
        </ul>
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
import {reactive} from "vue";
import {email} from "@vuelidate/validators";

export default {
  name: "Settings",
  setup(){
    const store = useStore();
    const router = useRouter();
    const state = reactive({
      isEditing: false,
      formData:{
        idPerson: 0,
        name: '',
        surname: '',
        email: '',
        loginName: '',
        idUserType: ''
      },
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
      formData: {
        name: { required, maxLength: maxLength(20), onlyLetters },
        surname: { required, maxLength: maxLength(20), onlyLetters},
        email: { required, email },
        loginName: { required, maxLength: maxLength(15), onlyLetters },
        idUserType: { required }
      },
      newUser: {
        name: { required, maxLength: maxLength(20), onlyLetters },
        surname: { required, maxLength: maxLength(20), onlyLetters },
        email: { required, email },
        loginName: { required, maxLength: maxLength(15), onlyLetters },
        password: { required },
        idUserType: { required }
      }
    }

    const v$ = useVuelidate(rules, state);



    async function toggleEdit() {
      if (state.isEditing) {
        v$.value.name.$touch();
        if (!v$.value.$error) {
          try {
            const response = await axios.put('https://localhost:44384/api/User', state.formData, {
              headers: {
                'Authorization': `Bearer ${store.getters.getToken}`
              },
            });
            if (response.data.httpStatusCode && response.data.httpStatusCode !== 200) {
              state.errors = response.data.errors || {};
              console.log('Error', response.data.message);
            } else {
              console.log('Success:', response.data);
              state.isEditing = false;
              state.errors = '';
              const foundStatus = state.userTypes.find(status => status.idType == state.formData.idUserType);
              state.typeTitle = foundStatus ? foundStatus.title : 'Status not found';
            }
          } catch (error) {
            if (error.response && error.response.data && error.response.data.errors) {
              state.errors = error.response.data.errors;
            }
            console.log('Error:', error);
          }
        }
      } else {
        state.isEditing = true;
      }
    }

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
        const response = await axios.get('https://localhost:44384/api/User');
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
            await router.push('/arrivals');
          }
        } catch (error) {
          if (error.response && error.response.data && error.response.data.errors) {
            state.errors = error.response.data.errors;
          }
          console.log('Error', error);
        }
      }
    }

    const removeUser = (index) => {
      state.users.splice(index, 1);
    };

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


    return {state, toggleEdit, fetchUser, addUser, v$, clearNewUserForm, fetchUsers, removeUser}
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

.element{
  display: flex;
  justify-content: center;
  width: 100%;
  background-color:#C8B6A6;
  border-radius: 5px;
  font-weight: bold;
  font-size: 15px;
  padding-top: 10px;
  padding-bottom: 10px;
  margin: 10px 0;
}

.service-list ul {
  padding-left: 0;
  list-style-type: none;
  width: 100%;
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

</style>