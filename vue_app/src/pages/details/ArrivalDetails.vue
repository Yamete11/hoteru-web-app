<template>
  <navbar></navbar>
  <sidebar></sidebar>
  <div class="main">
    <h1>Arrival Details</h1>
    <form>
      <div class="date-inputs">
        <div class="input-form">
          <label>In: </label>
          <input
              v-model="state.formData.in"
              class="input"
              type="date"
              :readonly="!state.isEditing"
          >
        </div>
        <div class="input-form">
          <label>Out: </label>
          <input
              v-model="state.formData.out"
              class="input"
              type="date"
              :readonly="!state.isEditing"
          >
        </div>
      </div>
      <div class="guest">
        <label>Room information</label>
        <div class="date-inputs">
          <div class="input-form">
            <label>Capacity: </label>
            <input
                v-model="state.formData.capacity"
                class="input"
                type="number"
                placeholder="Enter room capacity"
                :readonly="!state.isEditing"
            >
          </div>
          <div class="input-form">
            <label>Type: </label>
            <input v-if="!state.isEditing" class="input" type="text" :value="state.roomType" readonly>
            <select v-else v-model="state.formData.idRoomType" class="input">
              <option disabled value="">Select type</option>
              <option v-for="roomType in state.roomTypes" :key="roomType.idType" :value="roomType.idType">{{ roomType.title }}</option>
            </select>
          </div>
        </div>

        <div class="input-form">
          <label>Room Selection: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.selectedRoom" readonly>
          <select v-else v-model="state.formData.idRoom">
            <option disabled value="">Select a room</option>
            <option v-for="room in state.rooms" :key="room.idRoom" :value="room.idRoom">{{ room.number }} - Capacity: {{ room.capacity }}</option>
          </select>
          <label>Price: {{state.formData.price}}</label>
        </div>
      </div>

      <div class="guest">
        <label>Guest personal information</label>
        <div class="input-form">
          <label>Guest Selection: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.selectedGuest" readonly>
          <select v-else v-model="state.formData.idGuest">
            <option disabled value="">Select a guest</option>
            <option v-for="guest in state.guests" :key="guest.idPerson" :value="guest.idPerson">
              {{ guest.name }} {{ guest.surname }}, {{ guest.passport }}
            </option>
          </select>
          <router-link v-if="state.isEditing" class="form-btn" to="/new-guest">Add new guest</router-link>
        </div>
      </div>

      <div class="guest">
        <label>Deposit option</label>
        <div class="input-form">
          <label>Deposit sum: </label>
          <input
              v-model="state.selectedDeposit.sum"
              class="input"
              type="number"
              placeholder="Enter deposit sum"
              :readonly="!state.isEditing"
          >
          <label>Choose type: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.selectedDeposit.idDepositType" readonly>
          <select v-else v-model="state.selectedDeposit.idDepositType">
            <option disabled value="0">Select type</option>
            <option v-for="type in state.depositTypes" :key="type.idType" :value="type.idType">
              {{ type.title }}
            </option>
          </select>
        </div>
      </div>

      <div class="guest">
        <label>Service option</label>
        <div class="input-form">
          <label v-if="state.isEditing">Choose a service</label>
          <label v-else>Added services</label>
          <select v-if="state.isEditing" v-model="state.selectedService" class="input">
            <option disabled value="0">Select service</option>
            <option v-for="service in state.services" :key="service.idService" :value="service">
              {{ service.title }}: {{ service.sum }}
            </option>
          </select>
          <button v-if="state.isEditing" @click.prevent="addService" class="form-btn">Add</button>
          <div class="service-list" v-if="state.formData.services && state.formData.services.length > 0">
            <ul class="added-services-list">
              <li class="element" v-for="(service, index) in state.formData.services" :key="index">
                {{ service.title }}: {{ service.sum }} <button v-if="state.isEditing" @click.prevent="removeService(index)">Remove</button>
              </li>
            </ul>
          </div>
        </div>
      </div>


      <div class="registration-class">
        <router-link class="registration-btn" to="/arrivals">Cancel</router-link>
        <button class="registration-btn" type="button" @click="toggleEdit">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
      </div>
    </form>
  </div>
</template>

<script>
import {computed, reactive, ref, watch} from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue } from '@vuelidate/validators';
import axios from 'axios';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { differenceInCalendarDays } from 'date-fns';
export default {
  name: "ArrivalDetails",
  props: {
    idReservation: {
      type: Number,
      required: true
    }
  },
  setup() {
    const store = useStore();
    const router = useRouter();
    const today = new Date().toISOString().split('T')[0];
    const state = reactive({
      formData: '',
      selectedRoom: '',
      selectedGuest: '',
      selectedDeposit: '',
      selectedService: '',
      selectedDepositType: '',
      selectedRoomType: '',
      rooms: [],
      roomType: '',
      roomTypes: [],
      guests: [],
      depositTypes: [],
      isEditing: false
    });

    async function toggleEdit(){
      state.isEditing = !state.isEditing;
    }

    async function fetchReservation(idReservation){
      console.log(idReservation)
      try{
        const response = await axios.get('https://localhost:44384/api/Reservation/arrival/' + idReservation,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });

        state.formData = response.data;
        console.log(state.formData)
      } catch(error){
        console.log(error);
      }

      try{
        const response = await axios.get('https://localhost:44384/api/Room/freeRooms?idRoom=' + state.formData.idRoom,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.rooms = response.data;

        const response2 = await axios.get('https://localhost:44384/api/RoomType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.roomTypes = response2.data;

        const foundRoom = state.rooms.find(status => status.idRoom === state.formData.idRoom);
        state.selectedRoom =  foundRoom.number + " - Capacity: " + foundRoom.capacity;
        state.roomType = foundRoom.type;

        const response3 = await axios.get('https://localhost:44384/api/Guest',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.guests = response3.data.list;

        const foundGuest = state.guests.find(guest => guest.idPerson === state.formData.idGuest)
        state.selectedGuest = foundGuest.name + " " + foundGuest.surname + ", " + foundGuest.passport

        const responseDeposit = await axios.get('https://localhost:44384/api/Deposit/' + state.formData.idDeposit,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.selectedDeposit = responseDeposit.data;

        const responseDepositType = await axios.get('https://localhost:44384/api/DepositType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.depositTypes = responseDepositType.data;

        const responseServices = await axios.get('https://localhost:44384/api/Service',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.services = responseServices.data.list;


      } catch (error) {
        console.log(error);
      }
    }

    const removeService = (index) => {
      state.formData.services.splice(index, 1);
    };

    const addService = () => {
      if (state.selectedService && !state.formData.services.includes(state.selectedService)) {
        state.formData.services.push(state.selectedService);
      }
    };

    return {
      state,
      today,
      fetchReservation,
      removeService,
      addService,
      toggleEdit
    }
  }
  ,
  mounted() {
    this.fetchReservation(this.idReservation);
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
  padding-top: 2vh;
  margin: 5%;
}

.creating-form {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 30%;
}

form {
  min-width: 30%;
}

.registration-class {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.date-inputs {
  display: flex;
  justify-content: space-between;
  width: 100%;
}

.input-form {
  display: flex;
  flex-direction: column;
  position: relative;
  flex: 1;
  margin-right: 10px;
}

.input-form:last-child {
  margin-right: 0;
}

.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: black;
}

.input-form input[type="text"],
.input-form input[type="date"],
.input-form input[type="number"],
.input-form select{
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px;
}

.registration-btn {
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
  margin: 10px;
  cursor: pointer;
  max-width: 110px;
}

.error-message {
  color: red;
  margin: 10px 0;
}
h1 {
  display: flex;
  justify-content: center;
  color: black;
}


.guest {
  display: flex;
  flex-direction: column;
  border: 2px solid #989595;
  border-radius: 5px;
  padding: 10px;
  margin-top: 20px;
}


.tab-switcher {
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
}

.tab-switcher span {
  cursor: pointer;
  padding: 10px;
  border-bottom: 2px solid transparent;
}

.tab-switcher span.active {
  border-color: #8D7B68;
  font-weight: bold;
  font-size: 20px;
}


.form-btn {
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
  cursor: pointer;
  max-width: 110px;
}

.element{
  display: flex;
  justify-content: space-around;
  width: 100%;
  background-color:#C8B6A6;
  border-radius: 5px;
  font-weight: bold;
  font-size: 15px;
  padding-top: 10px;
  padding-bottom: 10px;
  margin: 10px 0;
}

.service-list ul {
  padding-left: 0;
  list-style-type: none;
  width: 100%;
}

.service-list {
  max-height: 200px;
  overflow-y: auto;
  width: 96%;
  border: 1px solid black;
  border-radius: 5px;
  padding: 10px;
  margin-bottom: 10px;
  margin-top: 10px;

}
</style>