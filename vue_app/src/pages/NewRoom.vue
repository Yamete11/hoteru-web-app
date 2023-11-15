<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Room</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Number: </label>
          <input
              v-model="formData.Number"
              class="input"
              type="text"
              placeholder="Enter room number"
          >
        </div>
        <div class="input-form">
          <label>Capacity: </label>
          <input
              v-model="formData.Capacity"
              class="input"
              type="text"
              placeholder="Enter room capacity"
          >
        </div>
        <div class="input-form">
          <label>Price: </label>
          <input
              v-model="formData.Price"
              class="input"
              type="text"
              placeholder="Enter room price"
          >
        </div>
        <div class="input-form">
          <label>Type: </label>
          <input
              v-model="formData.Type"
              class="input"
              type="text"
              placeholder="Enter room type"
          >
        </div>
        <div class="input-form">
          <label>Status: </label>
          <input
              v-model="formData.Status"
              class="input"
              type="text"
              placeholder="Enter room status"
          >
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/rooms">Cancel</router-link>
          <router-link class="registration-btn" to="/rooms">Confirm</router-link>
        </div>
      </form>
    </div>

  </div>
</template>

<script>
import axios from 'axios';
export default {
  name: "NewRoom",
  data() {
    return {
      roomTypes: [],
      formData: {
        Number: '',
        Capacity: '',
        Price: '',
        Type: '',
        Status: ''
      }
    }
  },
  method:{
    async fetchRoomTypes(){
      try{
        const response = await axios.get('https://localhost:44384/api/RoomType');
        this.roomTypes = response.data;
      } catch (error) {
        console.error(error);
      }
    }
  },
  mounted(){
    this.fetchRoomTypes();
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
  flex-direction: column;
  flex-grow: 1;
  padding-top: 8vh;
  padding-left: 8%;
}
</style>