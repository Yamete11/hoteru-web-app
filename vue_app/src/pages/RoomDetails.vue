<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>Room Details</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Number: </label>
          <input v-model="room.number" class="input" type="text" placeholder="Enter room number" :readonly="!isEditing">
        </div>
        <div class="input-form">
          <label>Capacity: </label>
          <input v-model="room.capacity" class="input" type="text" placeholder="Enter room capacity" :readonly="!isEditing">
        </div>
        <div class="input-form">
          <label>Price: </label>
          <input v-model="room.price" class="input" type="text" placeholder="Enter room price" :readonly="!isEditing">
        </div>
        <div class="input-form">
          <label>Type: </label>
          <input v-if="!isEditing" class="input" type="text" :value="typeTitle" readonly>
          <select v-else v-model="room.type" class="input">
            <option v-for="type in roomTypes" :value="type.idRoomType" :key="type.idRoomType">{{ type.title }}</option>
          </select>
        </div>
        <div class="input-form">
          <label>Status: </label>
          <input v-if="!isEditing" class="input" type="text" :value="statusTitle" readonly>
          <select v-else v-model="room.status" class="input">
            <option v-for="status in roomStatuses" :value="status.idRoomStatus" :key="status.idRoomStatus">{{ status.title }}</option>
          </select>
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/rooms">Back</router-link>
          <button type="button" class="registration-btn" @click="toggleEdit">{{ isEditing ? 'Save' : 'Edit' }}</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "RoomDetails",
  data() {
    return {
      isEditing: false,
      room: {
        idRoom: '',
        number: '',
        capacity: '',
        price: '',
        type: '',
        status: ''
      },
      typeTitle: '',
      statusTitle: '',
      roomTypes: [],
      roomStatuses: []
    }
  },
  props: {
    idRoom: {
      type: Number,
      required: true
    }
  },
  methods: {
    async toggleEdit() {
      this.isEditing = !this.isEditing;
      if (!this.isEditing) {
        const foundType = this.roomTypes.find(type => type.idRoomType === this.room.type);
        this.typeTitle = foundType ? foundType.title : 'Type not found';

        const foundStatus = this.roomStatuses.find(status => status.idRoomStatus === this.room.status);
        this.statusTitle = foundStatus ? foundStatus.title : 'Status not found';
        try {
          const response = await axios.put('https://localhost:44384/api/Room', this.room);
          console.log('Success:', response.data);
        } catch (error) {
          console.log('Error:', error);
        }
      }
    },
    async fetchSpecificRoom(idRoom) {
      try {
        const response = await axios.get('https://localhost:44384/api/Room/' + idRoom);
        this.room = response.data;

        const responseType = await axios.get('https://localhost:44384/api/RoomType');
        this.roomTypes = responseType.data;

        const responseStatus = await axios.get('https://localhost:44384/api/RoomStatus');
        this.roomStatuses = responseStatus.data;

        const foundType = this.roomTypes.find(type => type.idRoomType === this.room.type);
        this.typeTitle = foundType ? foundType.title : 'Type not found';

        const foundStatus = this.roomStatuses.find(status => status.idRoomStatus === this.room.status);
        this.statusTitle = foundStatus ? foundStatus.title : 'Status not found';
      } catch (error) {
        console.error(error);
      }
    },
  },
  mounted() {
    this.fetchSpecificRoom(this.idRoom);
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