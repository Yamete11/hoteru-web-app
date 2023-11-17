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
          <select v-model="formData.Type">
            <option disabled value="">Select type</option>
            <option v-for="roomType in roomTypes" :key="roomType.idRoomType" :value="String(roomType.idRoomType)">{{ roomType.title }}</option>

          </select>

        </div>
        <div class="input-form">
          <label>Status: </label>
          <select v-model="formData.Status">
            <option disabled value="">Select status</option>
            <option v-for="roomStatus in roomStatuses" :key="roomStatus.idRoomStatus" :value="String(roomStatus.idRoomStatus)">{{ roomStatus.title }}</option>

          </select>

        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/rooms">Cancel</router-link>
          <router-link class="registration-btn" @click="addRoom" to="/rooms">Confirm</router-link>
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
      roomStatuses: [],
      selectedType: '',
      selectedStatus: '',
      formData: {
        Number: '',
        Capacity: '',
        Price: '',
        Type: '',
        Status: ''
      }
    }
  },
  methods:{
    async fetchRoomTypes(){
      console.log(this.formData)
      try{
        const response = await axios.get('https://localhost:44384/api/RoomType');
        this.roomTypes = response.data;
        const response2 = await axios.get('https://localhost:44384/api/RoomStatus');
        this.roomStatuses = response2.data;
      } catch (error) {
        console.error(error);
      }
    },
    async addRoom() {
      try {
        const response = await axios.post('https://localhost:44384/api/Room', this.formData);
        console.log('Success:', response.data);
      } catch (error) {
        console.log('Error:', error);
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