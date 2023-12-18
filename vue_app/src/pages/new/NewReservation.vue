<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Reservation</h1>
      <form @submit.prevent="addReservation" class="creating-form">

        <div class="tab-switcher">
          <span :class="{ active: state.formData.Confirmed === false }" @click="state.formData.Confirmed = false">Arrival</span>
          <span> - </span>
          <span :class="{ active: state.formData.Confirmed === true }" @click="state.formData.Confirmed = true">Reservation</span>
        </div>


        <div class="date-inputs">
          <div class="input-form">
            <label>In: </label>
            <input
                v-model="state.formData.In"
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
                v-model="state.formData.Out"
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
                  v-model="state.formData.Capacity"
                  class="input"
                  type="number"
                  placeholder="Enter room capacity"
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
            <select v-model="state.formData.IdRoom">
              <option disabled value="">Select a room</option>
              <option v-for="room in filteredRooms" :key="room.idRoom" :value="room.idRoom">{{ room.number }} - Capacity: {{ room.capacity }}</option>
            </select>
            <label>Price: {{state.formData.Price}}</label>
          </div>
        </div>

        <div class="guest">
          <label>Guest personal information</label>
          <div class="input-form">
            <label>Guest Selection: </label>
            <select v-model="state.formData.idPerson">
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
  name: "NewReservation",
  setup(){
    const store = useStore();
    const router = useRouter();
    const today = new Date().toISOString().split('T')[0];
    const state = reactive({
      formData: {
        In: today,
        Out: Date,
        Capacity: 0,
        Price: 0,
        IdRoom: 0,
        Confirmed: false,
        Sum: 0,
        IdDepositType: 0,
        idPerson: 0,
        services: []
      },
      selectedService: null,
      statusTitle: '',
      guestStatuses: [],
      roomType: '',
      room: '',
      rooms: [],
      roomTypes: [],
      depositTypes: [],
      guests: [],
      services: [],
      errors: {},
      guestSearchQuery: '',
      filteredGuests: [],
      isGuestSelected: false
    });


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

        const responseStatus = await axios.get('https://localhost:44384/api/GuestStatus',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.guestStatuses = responseStatus.data;

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

        console.log(state.guests)
      } catch (error) {
        console.error(error);
      }
    }

    async function addReservation(){
      console.log(this.state.formData)
      try {
        const response = await axios.post('https://localhost:44384/api/Reservation', state.formData, {
          headers: {
            'Authorization': `Bearer ${store.getters.getToken}`
          }
        });
        console.log('Response:', response.data);
        if (response.data && response.data.httpStatusCode === 200) {
          await router.push('/reservations');
        }
      } catch (error) {
        console.error(error);
      }
    }

    async function addGuest(){
      try {
        const response = await axios.post('https://localhost:44384/api/Guest', state.formData.guest,{
          headers: {
            'Authorization': `Bearer ${store.getters.getToken}`
          }
        });
        console.log('Response:', response.data);
        if (response.data && response.data.httpStatusCode === 200) {
          await router.push('/guests');
        }
      } catch (error) {
        if (error.response && error.response.data && error.response.data.errors) {
          state.errors = error.response.data.errors;
        }
        console.log('Error', error);
      }
    }

    const filteredRooms = computed(() => {
      const selectedRoomType = state.roomTypes.find(rt => rt.idRoomType.toString() === state.roomType)?.title;
      return state.rooms.filter(room => {
        const capacityMatch = room.capacity >= state.formData.Capacity;
        const roomTypeMatch = room.type === selectedRoomType;
        return capacityMatch && roomTypeMatch;
      });
    });


    const v$ = useVuelidate(rules, state);

    const minOutDate = ref('');
    watch(() => state.formData.In, (newValue) => {
      if (newValue) {
        const inDate = new Date(newValue);
        inDate.setDate(inDate.getDate() + 1);
        minOutDate.value = inDate.toISOString().split('T')[0];
      } else {
        minOutDate.value = null;
      }
    });


    watch(() => state.guestSearchQuery, (newValue) => {
      if (!newValue) {
        state.isGuestSelected = false;
      }
    });

    const calculatedPrice = computed(() => {
      const inDate = new Date(state.formData.In);
      const outDate = new Date(state.formData.Out);
      const daysDifference = differenceInCalendarDays(outDate, inDate);
      const selectedRoom = state.rooms.find(x => String(x.idRoom) === String(state.formData.IdRoom))
      if(selectedRoom !== undefined){
        return daysDifference * selectedRoom.price;
      } else {
        return 0
      }
    });

    watch(() => [state.formData.In, state.formData.Out, state.formData.IdRoom], () => {
      if(isNaN(calculatedPrice.value)){
        state.formData.Price = 0
      } else {
        state.formData.Price = parseFloat(calculatedPrice.value);
      }
    });

    const maxInDate = computed(() => {
      if (state.formData.Out && state.formData.Out !== Date) {
        const outDate = new Date(state.formData.Out);
        outDate.setDate(outDate.getDate() - 1);
        return outDate.toISOString().split('T')[0];
      }
      return null;
    });

    const addService = () => {
      if (state.selectedService && !state.formData.services.includes(state.selectedService)) {
        state.formData.services.push(state.selectedService);
      }
    };

    const removeService = (index) => {
      state.formData.services.splice(index, 1);
    };


    return {state, v$, fetchRooms, filteredRooms, today, minOutDate, addReservation, calculatedPrice, maxInDate, addGuest, addService, removeService }
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
