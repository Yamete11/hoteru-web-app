<template>
  <div class="newRoom-component">
  <navbar></navbar>
  <sidebar></sidebar>
  <div class="main">
    <h1>Account Details</h1>
    <form @submit.prevent class="creating-form">
      <div class="input-form">
        <label>Name: </label>
        <input v-model="state.user.name" class="input" type="text" placeholder="Enter room number" :readonly="!state.isEditing">
      </div>
      <div class="input-form">
        <label>Surname: </label>
        <input v-model="state.user.surname" class="input" type="text" placeholder="Enter room capacity" :readonly="!state.isEditing">
      </div>
      <div class="input-form">
        <label>Email: </label>
        <input v-model="state.user.email" class="input" type="text" placeholder="Enter room price" :readonly="!state.isEditing">
      </div>
      <div class="input-form">
        <label>Login: </label>
        <input v-model="state.user.loginName" class="input" type="text" placeholder="Enter room price" :readonly="!state.isEditing">
      </div>
      <div class="input-form">
        <label>Type: </label>
        <input v-if="!state.isEditing" class="input" type="text" :value="state.typeTitle" readonly>
        <select v-else v-model="state.user.idUserType" class="input">
          <option v-for="type in state.userTypes" :value="type.idUserType" :key="type.idUserType">{{ type.title }}</option>
        </select>
      </div>


      <div class="registration-class">
        <router-link class="registration-btn" to="/arrivals">Back</router-link>
        <button type="button" class="registration-btn" @click="toggleEdit">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
      </div>
    </form>

    <h1>Add new user</h1>
    <form @submit.prevent class="creating-form">
      <div class="input-form">
        <label>Name: </label>
        <input v-model="state.newUser.name" class="input" type="text" placeholder="Enter name">
      </div>
      <div class="input-form">
        <label>Surname: </label>
        <input v-model="state.newUser.surname" class="input" type="text" placeholder="Enter surname">
      </div>
      <div class="input-form">
        <label>Email: </label>
        <input v-model="state.newUser.email" class="input" type="text" placeholder="Enter email">
      </div>
      <div class="input-form">
        <label>Login: </label>
        <input v-model="state.newUser.login" class="input" type="text" placeholder="Enter login">
      </div>
      <div class="input-form">
        <label>Password: </label>
        <input v-model="state.newUser.password" class="input" type="text" placeholder="Enter password">
      </div>
      <div class="input-form">
        <label>Type: </label>
        <select v-model="state.newUser.idUserType" class="input">
          <option v-for="type in state.userTypes" :value="type.idType" :key="type.idType">{{ type.title }}</option>
        </select>
      </div>

      <div class="registration-class">
        <button type="button" class="registration-btn" @click="">Clean</button>
        <button type="button" class="registration-btn" @click="addUser">Confirm</button>
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

export default {
  name: "Settings",
  setup(){
    const store = useStore();
    const router = useRouter();
    const state = reactive({
      isEditing: false,
      user:{
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
        login: '',
        password: '',
        idUserType: 0
      },
      newUserTypeTitle: '',
      userTypes: [],
      typeTitle: ''
    });

    async function toggleEdit() {
      this.isEditing = !this.isEditing;
    }

    async function fetchUser(idUser){
      try {
        const response = await axios.get('https://localhost:44384/api/User/fullUser/' + idUser, {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.user = response.data;

        const responseUserTypes = await axios.get('https://localhost:44384/api/UserType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.userTypes = responseUserTypes.data;

        console.log(state.userTypes)
        console.log(state.user)

        const foundType = state.userTypes.find(status => status.idType === state.user.idUserType);
        state.typeTitle = foundType ? foundType.title : 'Status not found';

      } catch (error) {
        console.error(error);
      }
    }

    async function addUser(){
      console.log(state.newUser)
      try {
        const response = await axios.post('https://localhost:44384/api/User', state.newUser,{
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


    return {state, toggleEdit, fetchUser, addUser}
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
</style>