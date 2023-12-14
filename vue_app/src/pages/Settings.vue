<template>
  <div class="newRoom-component">
  <navbar></navbar>
  <sidebar></sidebar>
  <div class="main">
    <h1>Account Details</h1>
    <form @submit.prevent class="creating-form">
      <div class="input-form">
        <label>Name: </label>
        <input v-model="user.name" class="input" type="text" placeholder="Enter room number" :readonly="!isEditing">
      </div>
      <div class="input-form">
        <label>Surname: </label>
        <input v-model="user.surname" class="input" type="text" placeholder="Enter room capacity" :readonly="!isEditing">
      </div>
      <div class="input-form">
        <label>Email: </label>
        <input v-model="user.email" class="input" type="text" placeholder="Enter room price" :readonly="!isEditing">
      </div>
      <div class="input-form">
        <label>Login: </label>
        <input v-model="user.loginName" class="input" type="text" placeholder="Enter room price" :readonly="!isEditing">
      </div>
      <div class="input-form">
        <label>Type: </label>
        <input v-if="!isEditing" class="input" type="text" :value="typeTitle" readonly>
        <select v-else v-model="user.idUserType" class="input">
          <option v-for="type in userTypes" :value="type.idUserType" :key="type.idUserType">{{ type.title }}</option>
        </select>
      </div>


      <div class="registration-class">
        <router-link class="registration-btn" to="/guests">Back</router-link>
        <button type="button" class="registration-btn" @click="toggleEdit">{{ isEditing ? 'Save' : 'Edit' }}</button>
      </div>
    </form>
  </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "Settings",
  data(){
    return{
      isEditing: false,
      user:{
        idPerson: 0,
        name: '',
        surname: '',
        email: '',
        loginName: '',
        idUserType: ''
      },
      userTypes: [],
      typeTitle: ''
    }
  },
  methods: {
    async toggleEdit() {
      this.isEditing = !this.isEditing;
    },
    async fetchUser(idUser){
      try {
        const response = await axios.get('https://localhost:44384/api/User/fullUser/' + idUser, {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        this.user = response.data;

        const responseUserTypes = await axios.get('https://localhost:44384/api/UserType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        this.userTypes = responseUserTypes.data;

        console.log(this.userTypes)
        console.log(this.user)

        const foundType = this.userTypes.find(status => status.idUserType === this.user.idUserType);
        this.typeTitle = foundType ? foundType.title : 'Status not found';

      } catch (error) {
        console.error(error);
      }
    }
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