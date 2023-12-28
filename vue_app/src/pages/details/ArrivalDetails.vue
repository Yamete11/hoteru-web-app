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
              v-model="state.reservation.In"
              class="input"
              type="date"
              :min="today"
              :max="maxInDate"
              placeholder="Enter in"
          >
        </div>
        <div class="input-form">
          <label>Out: </label>
          <input
              v-model="state.reservation.Out"
              class="input"
              type="date"
              :min="today"
              placeholder="Enter out"
          >
        </div>
      </div>
      <div class="guest">
        <label>Room information</label>
        <div class="date-inputs">
          <div class="input-form">
            <label>Capacity: </label>
            <input
                v-model="state.reservation.Capacity"
                class="input"
                type="number"
                placeholder="Enter room capacity"
            >
          </div>
          <div class="input-form">
            <label>Type: </label>
            <select v-model="state.roomType">
              <option disabled value="">Select type</option>
              <option v-for="roomType in state.roomTypes" :key="roomType.idType" :value="String(roomType.idType)">{{ roomType.title }}</option>
            </select>
          </div>
        </div>

        <div class="input-form">
          <label>Room Selection: </label>
          <select v-model="state.reservation.IdRoom">
            <option disabled value="">Select a room</option>
            <option v-for="room in filteredRooms" :key="room.idRoom" :value="room.idRoom">{{ room.number }} - Capacity: {{ room.capacity }}</option>
          </select>
          <label>Price: {{state.reservation.Price}}</label>
        </div>
      </div>

      <div class="guest">
        <label>Guest personal information</label>
        <div class="input-form">
          <label>Guest Selection: </label>
          <select v-model="state.reservation.idUser">
            <option disabled value="">Select a guest</option>
            <option v-for="guest in state.guests" :key="guest.idPerson" :value="guest.idPerson">
              {{ guest.name }} {{ guest.surname }}, {{ guest.passport }}
            </option>
          </select>
          <router-link class="form-btn" to="/new-guest">Add new guest</router-link>
        </div>
      </div>

      <div class="guest">
        <label>Deposit option</label>
        <div class="input-form">
          <label>Deposit sum: </label>
          <input
              v-model="state.formData.Sum"
              class="input"
              type="number"
              placeholder="Enter room capacity"
          >
          <label>Choose type: </label>
          <select v-model="state.formData.IdDepositType">
            <option disabled value="">Select type</option>
            <option v-for="type in state.depositTypes" :key="type.idDepositType" :value="type.idDepositType">
              {{ type.title }}
            </option>
          </select>
        </div>
      </div>

      <div class="guest">
        <label>Service option</label>
        <div class="input-form">
          <label>Choose a service</label>
          <select v-model="state.selectedService" class="input">
            <option disabled value="">Select service</option>
            <option v-for="service in state.services" :key="service.idService" :value="service">
              {{ service.title }}: {{ service.sum }}
            </option>
          </select>
          <button @click.prevent="addService" class="form-btn">Add</button>
          <div class="service-list" v-if="state.formData.services.length > 0">
            <ul class="added-services-list">
              <li class="element" v-for="(service, index) in state.formData.services" :key="index">
                {{ service.title }}: {{ service.sum }} <button @click.prevent="removeService(index)">Remove</button>
              </li>
            </ul>
          </div>
        </div>
      </div>




      <div class="registration-class">
        <router-link class="registration-btn" to="/rooms">Cancel</router-link>
        <button class="registration-btn" type="submit">Confirm</button>
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
      reservation: '',
      rooms: '',
      roomTypes: '',
      guests: '',
      depositTypes: '',
    });

    async function fetchReservation(idReservation){
      console.log(idReservation)
      try{
        const response = await axios.get('https://localhost:44384/api/Reservation/arrival/' + idReservation,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });

        state.reservation = response.data;
        console.log(state.reservation)
      } catch(error){
        console.log(error);
      }

      try{
        const response = await axios.get('https://localhost:44384/api/Room/freeRooms',{
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

        const response3 = await axios.get('https://localhost:44384/api/Guest',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.guests = response3.data.list;

        const responseDepositType = await axios.get('https://localhost:44384/api/DepositType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.depositTypes = responseDepositType.data;

      } catch (error) {
        console.error(error);
      }
    }

    return {
      state,
      today,
      fetchReservation
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