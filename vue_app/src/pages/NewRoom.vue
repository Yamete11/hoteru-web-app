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
          <span class="error-message" v-if="errors.Number">{{ errors.Number[0] }}</span>
        </div>
        <div class="input-form">
          <label>Capacity: </label>
          <input
              v-model="formData.Capacity"
              class="input"
              type="text"
              placeholder="Enter room capacity"
          >
          <span class="error-message" v-if="errors.Capacity">{{ errors.Capacity[0] }}</span>
        </div>
        <div class="input-form">
          <label>Price: </label>
          <input
              v-model="formData.Price"
              class="input"
              type="text"
              placeholder="Enter room price"
          >
          <span class="error-message" v-if="errors.Price">{{ errors.Price[0] }}</span>
        </div>
        <div class="input-form">
          <label>Type: </label>
          <select v-model="formData.Type">
            <option disabled value="">Select type</option>
            <option v-for="roomType in roomTypes" :key="roomType.idRoomType" :value="String(roomType.idRoomType)">{{ roomType.title }}</option>
          </select>
          <span class="error-message" v-if="errors.Type">{{ errors.Type[0] }}</span>
        </div>
        <div class="input-form">
          <label>Status: </label>
          <select v-model="formData.Status">
            <option disabled value="">Select status</option>
            <option v-for="roomStatus in roomStatuses" :key="roomStatus.idRoomStatus" :value="String(roomStatus.idRoomStatus)">{{ roomStatus.title }}</option>
          </select>
          <span class="error-message" v-if="errors.Status">{{ errors.Status[0] }}</span>
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/rooms">Cancel</router-link>
          <button class="registration-btn" @click="addRoom">Confirm</button>
        </div>
      </form>
    </div>

  </div>
</template>

<script>
import axios from 'axios';
import { useVuelidate } from '@vuelidate/core'
import { required, email } from '@vuelidate/validators'
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
        Capacity: 0,
        Price: 0,
        Type: '',
        Status: ''
      },
      errors: {
        Number: '',
        Capacity: '',
        Price: '',
        Type: '',
        Status: ''
      }
    }
  },
  setup: () => ({ v$: useVuelidate() }),
  validations (){
    return {
      formData: {
        Number: {required},
        Capacity: {required},
        Price: {required},
        Type: {required},
        Status: {required}
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
      console.log(this.formData)
      try {
        const response = await axios.post('https://localhost:44384/api/Room', this.formData);
        console.log('Response:', response.data);
        this.errors= {
              Number: '',
              Capacity: '',
              Price: '',
              Type: '',
              Status: ''
        }

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
  computed(){
    const v$ = useVuelidate(this.rules, this.formData)
    const submitForm = async () => {
      const result = await v$.value.$validate();
      if(result){
        alert("success")
      } else {
        alert("error")
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