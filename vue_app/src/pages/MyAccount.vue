<template>
  <div class="newRoom-component">
  <navbar></navbar>
  <sidebar></sidebar>
  <div class="main">
    <form @submit.prevent="toggleEdit" class="creating-form">
      <h1>Account Details</h1>
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
      }
    }

    const v$ = useVuelidate(rules, state);



    async function toggleEdit() {
      if (state.isEditing) {
        v$.value.$touch();
        console.log(state.formData)
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

    return {state, toggleEdit, fetchUser, v$}
  },
  mounted(){
    this.fetchUser(this.$store.getters.getUserData.idUser);
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