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
                :min="todayString"
                :max="maxInDate"
                @input="v$.formData.In.$touch()"
            >
          </div>
          <div class="input-form">
            <label>Out: </label>
            <input
                v-model="state.formData.Out"
                class="input"
                type="date"
                :min="tomorrowString"
                @input="v$.formData.Out.$touch()"
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
                  @input="v$.formData.Capacity.$touch()"
              >
            </div>
            <div class="input-form">
              <label>Type: </label>
              <select v-model="state.roomType" @change="v$.state.roomType.$touch()">
                <option disabled value="0" selected>Select type</option>
                <option v-for="roomType in state.roomTypes" :key="roomType.idType" :value="String(roomType.idType)">{{ roomType.title }}</option>
              </select>
            </div>
          </div>

          <div class="input-form">
            <label>Room Selection: </label>
            <select v-model="state.formData.IdRoom" @change="v$.formData.IdRoom.$touch()">
              <option disabled value="0" selected>Select a room</option>
              <option v-for="room in filteredRooms" :key="room.idRoom" :value="room.idRoom">{{ room.number }} - Capacity: {{ room.capacity }}</option>
            </select>
            <label>Price: {{state.formData.Price}}</label>
          </div>
        </div>

        <div class="guest">
          <label>Guest personal information</label>
          <div class="input-form">
            <label>Guest Selection: </label>
            <select v-model="state.formData.IdPerson" @change="v$.formData.IdPerson.$touch()">
              <option disabled value="0">Select a guest</option>
              <option v-for="guest in state.guests" :key="guest.IdPerson" :value="guest.IdPerson">
                {{ guest.name }} {{ guest.surname }}, {{ guest.passport }}
              </option>
            </select>
            <router-link class="form-btn" to="/new-guest">Add new guest</router-link>
          </div>
        </div>

        <div class="guest">
          <label>Deposit option</label>
          <div v-if="!state.hasDeposit">
            <label>There is no deposit.</label>
          </div>
          <div v-else class="input-form">
            <label>Deposit sum: </label>
            <input
                v-model="state.formData.Sum"
                class="input"
                type="number"
                placeholder="Enter room capacity"
            >
            <label>Choose type: </label>
            <select v-model="state.formData.IdDepositType">
              <option disabled value="0">Select type</option>
              <option v-for="type in state.depositTypes" :key="type.idType" :value="type.idType">
                {{ type.title }}
              </option>
            </select>
          </div>
          <button @click.prevent="switchDeposit" class="form-btn">{{ state.hasDeposit ? 'Delete deposit' : 'Add deposit'}}</button>
        </div>

        <div class="guest">
          <label>Service option</label>
          <div class="input-form">
            <label>Choose a service</label>
            <select v-model="state.selectedService" class="input">
              <option value="" disabled selected>Select a service</option>
              <option v-for="service in state.services" :key="service.idService" :value="service">
                {{ service.title }}: {{ service.sum }}
              </option>
            </select>
            <button @click.prevent="addService" class="form-btn">Add</button>
            <div class="service-list" v-if="state.formData.services.length > 0">
              <ul class="added-services-list">
                <li class="element" v-for="(service, index) in state.formData.services" :key="index">
                  <span>{{ service.title }}: {{ service.sum }} </span>
                  <button class="btn" @click.prevent="removeService(index)">Remove</button>
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
    const today = new Date();
    const todayString = today.toISOString().split('T')[0];
    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1);
    const tomorrowString = tomorrow.toISOString().split('T')[0];

    const idUser = store.getters.getUserData.idUser;

    const state = reactive({
      formData: {
        In: todayString,
        Out: Date,
        Capacity: 0,
        Price: 0,
        IdRoom: 0,
        Confirmed: false,
        Sum: 0,
        IdDepositType: 0,
        IdPerson: 0,
        services: [],
        idUser: idUser
      },
      selectedService: '',
      statusTitle: '',
      guestStatuses: [],
      roomType: 0,
      room: '',
      rooms: [],
      roomTypes: [],
      depositTypes: [],
      guests: [],
      services: [],
      errors: {},
      guestSearchQuery: '',
      filteredGuests: [],
      isGuestSelected: false,
      hasDeposit: false
    });


    const rules = {
      formData: {
        In: { required, minValue: val => new Date(val) >= new Date(today) },
        Out: { required, minValue: val => new Date(val) > new Date(state.formData.In) },
        Capacity: { required, numeric, minValue: value => value > 0, maxValue: value => value <= 40 },
        IdRoom: { required },
        IdPerson: { required },
        IdDepositType: {
          required: () => !state.hasDeposit
        },
        Sum: {
          required: () => !state.hasDeposit,
          numeric: () => !state.hasDeposit,
        }
      }
    };


    const v$ = useVuelidate(rules, state);

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
          await router.push('/arrivals');
        }
      } catch (error) {
        console.error(error);
      }
    }

    const filteredRooms = computed(() => {
      const selectedRoomType = state.roomTypes.find(rt => rt.idType.toString() === state.roomType)?.title;
      console.log(state.roomType)
      return state.rooms.filter(room => {
        const capacityMatch = room.capacity >= state.formData.Capacity;
        const roomTypeMatch = room.type === selectedRoomType;
        return capacityMatch && roomTypeMatch;
      });
    });


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

    function switchDeposit() {
      state.hasDeposit = !state.hasDeposit;
      state.formData.Sum = 0;
      state.formData.IdDepositType = 0;
    }


    return {
      state,
      v$,
      fetchRooms,
      filteredRooms,
      todayString,
      tomorrowString,
      minOutDate,
      addReservation,
      calculatedPrice,
      maxInDate,
      addService,
      removeService,
      switchDeposit
    }
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

.element {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  margin: 10px 0;
  background-color: #C8B6A6;
  border-radius: 5px;
  font-weight: bold;
  font-size: 15px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.service-list ul {
  list-style-type: none;
  padding: 0;
  margin: 0;
  max-width: 800px;
}

.service-list {
  overflow-y: auto;
  border: 1px solid black;
  border-radius: 5px;
  padding: 10px;
  margin-bottom: 10px;
  margin-top: 10px;
}


.btn {
  padding: 0.3rem 0.8rem;
  font-size: 0.8rem;
  font-weight: bold;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
</style>
