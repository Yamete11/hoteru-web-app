<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>Guest Details</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Name: </label>
          <input v-model="guest.name" class="input" type="text" placeholder="Enter room number" :readonly="!isEditing">
        </div>
        <div class="input-form">
          <label>Surname: </label>
          <input v-model="guest.surname" class="input" type="text" placeholder="Enter room capacity" :readonly="!isEditing">
        </div>
        <div class="input-form">
          <label>Email: </label>
          <input v-model="guest.email" class="input" type="text" placeholder="Enter room price" :readonly="!isEditing">
        </div>
        <div class="input-form">
          <label>Tel. number: </label>
          <input v-model="guest.telNumber" class="input" type="text" placeholder="Enter room price" :readonly="!isEditing">
        </div>
        <div class="input-form">
          <label>Passport: </label>
          <input v-model="guest.passport" class="input" type="text" placeholder="Enter room price" :readonly="!isEditing">
        </div>

        <div class="input-form">
          <label>Status: </label>
          <input v-if="!isEditing" class="input" type="text" :value="statusTitle" readonly>
          <select v-else v-model="guest.idGuestStatus" class="input">
            <option v-for="status in guestStatuses" :value="status.idGuestStatus" :key="status.idGuestStatus">{{ status.title }}</option>
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
  name: "GuestDetails",
  data() {
    return {
      isEditing: false,
      guest: {
        idPerson: '',
        name: '',
        surname: '',
        email: '',
        telNumber: '',
        passport: '',
        idGuestStatus: ''
      },
      statusTitle: '',
      guestStatuses: []
    }
  },
  props: {
    idPerson: {
      type: Number,
      required: true
    }
  },
  methods: {
    async toggleEdit() {
      this.isEditing = !this.isEditing;
      if (!this.isEditing) {

        const foundStatus = this.guestStatuses.find(status => status.idGuestStatus === this.guest.idGuestStatus);
        this.statusTitle = foundStatus ? foundStatus.title : 'Status not found';
        try {
          const response = await axios.put('https://localhost:44384/api/Guest', this.guest);
          console.log('Success:', response.data);
        } catch (error) {
          console.log('Error:', error);
        }
      }
    },
    async fetchSpecificGuest(idPerson) {
      try {
        const response = await axios.get('https://localhost:44384/api/Guest/' + idPerson);
        this.guest = response.data;

        const responseStatus = await axios.get('https://localhost:44384/api/GuestStatus');
        this.guestStatuses = responseStatus.data;

        const foundStatus = this.guestStatuses.find(status => status.idGuestStatus === this.guest.idGuestStatus);
        this.statusTitle = foundStatus ? foundStatus.title : 'Status not found';
      } catch (error) {
        console.error(error);
      }
    },
  },
  mounted() {
    this.fetchSpecificGuest(this.idPerson);
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