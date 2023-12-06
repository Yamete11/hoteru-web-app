<template>
  <div class="newService-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>Service Details</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Title: </label>
          <input
              v-model="service.title"
              class="input"
              type="text"
              placeholder="Enter room number"
              :readonly="!isEditing"
          >
        </div>
        <div class="input-form">
          <label>Price: </label>
          <input
              v-model="service.sum"
              class="input"
              type="text"
              placeholder="Enter room capacity"
              :readonly="!isEditing"
          >
        </div>
        <div class="input-form">
          <label>Description: </label>
          <input
              v-model="service.description"
              class="input"
              type="text"
              placeholder="Enter room price"
              :readonly="!isEditing"
          >
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/services">Back</router-link>
          <button type="button" class="registration-btn" @click="toggleEdit">{{ isEditing ? 'Save' : 'Edit' }}</button>
        </div>
      </form>
    </div>

  </div>
</template>

<script>
import axios from 'axios';
export default {
  name: "ServiceDetails",
  props: {
    idService: {
      type: Number,
      required: true
    }
  },
  data(){
    return {
      isEditing: false,
      service: {
        idService: '',
        title: '',
        sum: '',
        description: ''
      }
    }
  },
  methods:{
    async toggleEdit() {
      this.isEditing = !this.isEditing;
    },
    async fetchSpecificService(idService) {
      try {
        const response = await axios.get('https://localhost:44384/api/Service/' + idService);
        this.service = response.data;
      } catch (error) {
        console.error(error);
      }
    },
  },
  mounted() {
    this.fetchSpecificService(this.idService);
  }
}
</script>

<style scoped>
.newService-component {
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