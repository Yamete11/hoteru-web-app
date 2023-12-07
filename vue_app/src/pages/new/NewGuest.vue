<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Guest</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Name: </label>
          <input
              v-model="formData.name"
              class="input"
              type="text"
              placeholder="Enter name"
          >
        </div>
        <div class="input-form">
          <label>Surname: </label>
          <input
              v-model="formData.surname"
              class="input"
              type="text"
              placeholder="Enter surname"
          >
        </div>
        <div class="input-form">
          <label>Email: </label>
          <input
              v-model="formData.email"
              class="input"
              type="text"
              placeholder="Enter email"
          >
        </div>

        <div class="input-form">
          <label>Tel. number: </label>
          <input
              v-model="formData.telNumber"
              class="input"
              type="text"
              placeholder="Enter tel. number"
          >
        </div>

        <div class="input-form">
          <label>Passport: </label>
          <input
              v-model="formData.passport"
              class="input"
              type="text"
              placeholder="Enter room price"
          >
        </div>

        <div class="input-form">
          <label>Status: </label>
          <select v-model="formData.status">
            <option disabled value="">Select status</option>
            <option v-for="guestStatus in guestStatuses" :key="guestStatus.idGuestStatus" :value="String(guestStatus.idGuestStatus)">{{ guestStatus.title }}</option>
          </select>
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/guests">Cancel</router-link>
          <button class="registration-btn" @click="addGuest">Confirm</button>
        </div>
      </form>
    </div>

  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "NewGuest",
  data(){
    return {
      formData: {
        idPerson: '',
        name: '',
        surname: '',
        email: '',
        telNumber: '',
        passport: '',
        status: ''
      },
      guestStatuses: [],
    }
  },
  methods:{
    async fetchGuestStatuses(){
      console.log(this.formData)
      try{
        const response = await axios.get('https://localhost:44384/api/GuestStatus');
        this.guestStatuses = response.data;
      } catch (error) {
        console.error(error);
      }
    },
    async addGuest() {
      console.log(this.formData)
      try {
        const response = await axios.post('https://localhost:44384/api/Guest', this.formData);
        console.log('Response:', response.data);

        if (response.data && response.data.httpStatusCode === 200) {
          this.$router.push({ name: "Rooms" });
        }
      } catch (error) {
        if (error.response && error.response.data && error.response.data.errors) {
          this.errors = error.response.data.errors;
        } else {
          console.log('Error', error);
        }
      }
    }
  },
  mounted(){
    this.fetchGuestStatuses();
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