<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Room</h1>
      <form @submit.prevent="addRoom" class="creating-form">

        <div class="input-form">
          <label>Number: </label>
          <input
              v-model="state.formData.Number"
              class="input"
              type="text"
              placeholder="Enter room number"
              @input="v$.formData.Number.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Number.$error">
            <span v-if="!v$.formData.Number.required.$response">Number is required*</span>
            <span v-if="!v$.formData.Number.maxLength.$response">Number must be less than 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Number">{{ state.errors.Number[0] }}</span>
        </div>


        <div class="input-form">
          <label>Capacity: </label>
          <input
              v-model="state.formData.Capacity"
              class="input"
              type="text"
              placeholder="Enter room capacity"
              @input="v$.formData.Capacity.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Capacity.$error">
            <span v-if="!v$.formData.Capacity.required.$response">Capacity is required*</span>
            <span v-else-if="!v$.formData.Capacity.numeric.$response">Capacity must be a number*</span>
            <span v-else-if="!v$.formData.Capacity.maxValue.$response">Capacity must be less than or equal to 10*</span>
          </span>
          <span class="error-message" v-if="state.errors.Capacity">{{ state.errors.Capacity[0] }}</span>
        </div>


        <div class="input-form">
          <label>Price: </label>
          <input
              v-model="state.formData.Price"
              class="input"
              type="text"
              placeholder="Enter room price"
              @input="v$.formData.Price.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Price.$error">
            <span v-if="!v$.formData.Price.required.$response">Price is required*</span>
            <span v-else-if="!v$.formData.Price.numeric.$response">Price must be a number*</span>
            <span v-else-if="!v$.formData.Price.maxValue.$response">Price must be less than or equal to 1,000,000*</span>
          </span>
          <span class="error-message" v-if="state.errors.Price">{{ state.errors.Price[0] }}</span>
        </div>


        <div class="input-form">
          <label>Type: </label>
          <select v-model="state.formData.Type" @change="v$.formData.Type.$touch()">
            <option disabled value="">Select type</option>
            <option v-for="roomType in state.roomTypes" :key="roomType.idRoomType" :value="String(roomType.idRoomType)">{{ roomType.title }}</option>
          </select>
          <span class="error-message" v-if="v$.formData.Type.$error">
            <span v-if="!v$.formData.Type.required.$response">Type is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.Type">{{ state.errors.Type[0] }}</span>
        </div>


        <div class="input-form">
          <label>Status: </label>
          <select v-model="state.formData.Status" @change="v$.formData.Status.$touch()">
            <option disabled value="">Select status</option>
            <option v-for="roomStatus in state.roomStatuses" :key="roomStatus.idRoomStatus" :value="String(roomStatus.idRoomStatus)">{{ roomStatus.title }}</option>
          </select>
          <span class="error-message" v-if="v$.formData.Status.$error">
            <span v-if="!v$.formData.Status.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.Status">{{ state.errors.Status[0] }}</span>
        </div>


        <div class="registration-class">
          <router-link class="registration-btn" to="/rooms">Cancel</router-link>
          <button class="registration-btn" type="submit">Confirm</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { reactive } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue } from '@vuelidate/validators';
import axios from 'axios';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

export default {
  name: "NewRoom",
  setup() {
    const store = useStore();
    const router = useRouter();
    const state = reactive({
      roomTypes: [],
      roomStatuses: [],
      formData: {
        Number: '',
        Capacity: '',
        Price: '',
        Type: '',
        Status: ''
      },
      errors: {}
    });

    const rules = {
      formData: {
        Number: { required, maxLength: maxLength(20) },
        Capacity: { required, numeric, maxValue: maxValue(10) },
        Price: { required, numeric, maxValue: maxValue(1000000) },
        Type: { required },
        Status: { required }
      }
    };

    const v$ = useVuelidate(rules, state);

    async function fetchRoomTypes() {
      try {
        const response = await axios.get('https://localhost:44384/api/RoomType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.roomTypes = response.data;
        const response2 = await axios.get('https://localhost:44384/api/RoomStatus',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.roomStatuses = response2.data;
      } catch (error) {
        console.error(error);
      }
    }

    async function addRoom() {
      /*v$.value.$touch();
      if (!v$.value.$error) {*/
        try {
          const response = await axios.post('https://localhost:44384/api/Room', state.formData, {
            headers: {
              'Authorization': `Bearer ${store.getters.getToken}`
            }
          });
          console.log('Response:', response.data);
          if (response.data && response.data.httpStatusCode === 200) {
            await router.push('/rooms');
          }
        } catch (error) {
          if (error.response && error.response.data && error.response.data.errors) {
            state.errors = error.response.data.errors;
          }
          console.log('Error', error);
        }
      /*}*/
    }

    return { state, v$, addRoom, fetchRoomTypes };
  },
  mounted() {
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