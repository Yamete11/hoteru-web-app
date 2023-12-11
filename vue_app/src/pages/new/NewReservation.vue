<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Reservation</h1>
      <form @submit.prevent="addRoom" class="creating-form">

        <div class="tab-switcher">
          <span :class="{ active: state.activeTab === 'arrival' }" @click="state.activeTab = 'arrival'">Arrival</span>
          <span> - </span>
          <span :class="{ active: state.activeTab === 'reservation' }" @click="state.activeTab = 'reservation'">Reservation</span>
        </div>


        <div class="date-inputs">
          <div class="input-form">
            <label>In: </label>
            <input
                v-model="state.in"
                class="input"
                type="date"
                :min="today"
                placeholder="Enter in"
            >
          </div>
          <div class="input-form">
            <label>Out: </label>
            <input
                v-model="state.out"
                class="input"
                type="date"
                :min="minOutDate"
                :disabled="!state.in"
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
                  v-model="state.capacity"
                  class="input"
                  type="text"
                  placeholder="Enter room number"
              >
            </div>
            <div class="input-form">
              <label>Type: </label>
              <select v-model="state.roomType">
                <option disabled value="">Select type</option>
                <option v-for="roomType in state.roomTypes" :key="roomType.idRoomType" :value="String(roomType.idRoomType)">{{ roomType.title }}</option>
              </select>
            </div>
          </div>

          <div class="input-form">
            <label>Room Selection: </label>
            <select v-model="state.room">
              <option disabled value="">Select a room</option>
              <option v-for="room in filteredRooms" :key="room.idRoom" :value="String(room.idRoom)">{{ room.number }} - Capacity: {{ room.capacity }}</option>
            </select>
          </div>
        </div>

        <div class="guest">
          <label>Guest personal information</label>
          <div class="input-form">
            <label>Guest Name: </label>
            <input
                v-model="state.guestSearchQuery"
                @input="searchGuests"
                class="input"
                type="text"
                placeholder="Start typing name..."
            >
            <ul v-if="state.filteredGuests.length" class="suggestions">
              <li v-for="guest in state.filteredGuests" :key="state.guest.idPerson" @click="selectGuest(guest)">
                {{ guest.name }} {{ guest.surname}}, {{ guest.passport}}
              </li>
            </ul>
          </div>


          <div class="date-inputs">
            <div class="input-form">
              <label>Name: </label>
              <input
                  v-model="state.guest.name"
                  class="input"
                  type="text"
                  placeholder="Enter name"
                  :disabled="state.isGuestSelected"
              >
            </div>

            <div class="input-form">
              <label>Surname: </label>
              <input
                  v-model="state.guest.surname"
                  class="input"
                  type="text"
                  placeholder="Enter name"
                  :disabled="state.isGuestSelected"
              >
            </div>
          </div>

          <div class="date-inputs">
            <div class="input-form">
              <label>Email: </label>
              <input
                  v-model="state.guest.email"
                  class="input"
                  type="text"
                  placeholder="Enter name"
                  :disabled="state.isGuestSelected"
              >
            </div>

            <div class="input-form">
              <label>Passport: </label>
              <input
                  v-model="state.guest.telNumber"
                  class="input"
                  type="text"
                  placeholder="Enter name"
                  :disabled="state.isGuestSelected"
              >
            </div>
          </div>

          <div class="date-inputs">
            <div class="input-form">
              <label>Tel. number: </label>
              <input
                  v-model="state.guest.passport"
                  class="input"
                  type="text"
                  placeholder="Enter name"
                  :disabled="state.isGuestSelected"
              >
            </div>

            <div class="input-form">
              <label>Status: </label>
              <select v-model="state.guest.idGuestStatus" class="input" :disabled="state.isGuestSelected">
                <option disabled value="">Select status</option>
                <option v-for="status in state.guestStatuses" :value="status.idGuestStatus" :key="status.idGuestStatus">{{ status.title }}</option>
              </select>
            </div>
          </div>
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
import {computed, reactive, ref, watch} from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue } from '@vuelidate/validators';
import axios from 'axios';
export default {
  name: "NewReservation",
  setup(){
    const state = reactive({
      formData: {
        In: '',
        Out: '',
        Capacity: '',
        Price: '',
        RoomNumber: '',
      },
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
      guestStatuses: [],
      in: '',
      out: '',
      capacity: 0,
      roomType: '',
      room: '',
      rooms: [],
      roomTypes: [],
      guests: [],
      errors: {},
      guestSearchQuery: '',
      filteredGuests: [],
      isGuestSelected: false,
      activeTab: 'reservation'
    });

    function searchGuests() {
      if (state.guestSearchQuery.length && Array.isArray(state.guests)) {
        state.filteredGuests = state.guests.filter(guest =>
            guest.name.toLowerCase().includes(state.guestSearchQuery.toLowerCase())
        );
      } else {
        state.filteredGuests = [];
      }
    }


    function selectGuest(guest) {
      state.guestSearchQuery = guest.name + " " + guest.surname + ", " + guest.passport;
      state.isGuestSelected = true;
      state.filteredGuests = [];
    }

    const rules = {
      formData: {
        In: { required, maxLength: maxLength(20) },
        Out: { required, numeric, maxValue: maxValue(10) },
        Capacity: { required, numeric, maxValue: maxValue(1000000) },
        Price: { required },
        RoomNumber: { required },
        BookedBy: {required},
        Deposit: {required}
      }
    }

    async function fetchRooms() {
      try {
        const response = await axios.get('https://localhost:44384/api/Room/freeRooms');
        state.rooms = response.data;
        const response2 = await axios.get('https://localhost:44384/api/RoomType');
        state.roomTypes = response2.data;
        const response3 = await axios.get('https://localhost:44384/api/Guest');
        state.guests = response3.data.list;

        const responseStatus = await axios.get('https://localhost:44384/api/GuestStatus');
        state.guestStatuses = responseStatus.data;

        console.log(state.guests)
      } catch (error) {
        console.error(error);
      }
    }

    const filteredRooms = computed(() => {
      const selectedRoomType = state.roomTypes.find(rt => rt.idRoomType.toString() === state.roomType)?.title;
      return state.rooms.filter(room => {
        const capacityMatch = room.capacity >= state.capacity;
        const roomTypeMatch = room.type === selectedRoomType;
        return capacityMatch && roomTypeMatch;
      });
    });


    const v$ = useVuelidate(rules, state);
    const today = new Date().toISOString().split('T')[0];
    const minOutDate = ref('');
    watch(() => state.in, (newValue) => {
      if (newValue) {
        const inDate = new Date(newValue);
        inDate.setDate(inDate.getDate() + 1);
        minOutDate.value = inDate.toISOString().split('T')[0];
      } else {
        minOutDate.value = '';
      }
    });

    watch(() => state.guestSearchQuery, (newValue) => {
      if (!newValue) {
        state.isGuestSelected = false;
      }
    });

    return {state, v$, fetchRooms, filteredRooms, today, minOutDate, searchGuests, selectGuest}
  },
  mounted() {
    this.fetchRooms();
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
.input-form select{
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px;
}

.error-message {
  color: red;
  margin: 10px 0;
}

.suggestions {
  list-style-type: none;
  margin: 0;
  padding: 0;
  position: absolute;
  z-index: 1000;
  width: 100%;
  background-color: white;
  border: 1px solid #ccc;
  border-radius: 5px;
  top: 100%;
}

.suggestions li {
  padding: 5px;
  cursor: pointer;
  border-bottom: 1px solid #eee;
  width: 100%;
}

.suggestions li:last-child {
  border-bottom: none;
}

.suggestions li:hover {
  background-color: #f1f1f1;
}

h1 {
  display: flex;
  justify-content: center;
  color: black;
}


.guest {
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
</style>
