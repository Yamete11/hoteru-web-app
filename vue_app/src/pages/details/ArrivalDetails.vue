<template>
  <navbar></navbar>
  <sidebar></sidebar>
  <div class="main">
    <h1>{{ detailsType }} Details</h1>
    <form>
      <div class="date-inputs">
        <div class="input-form">
          <label>In: </label>
          <input
              v-model="state.formData.in"
              class="input"
              type="date"
              :readonly="!state.isEditing"
              :min="state.minInDate"
              @input="v$.formData.in.$touch()"
          >
          <span class="error-message" v-if="v$.formData.in.$error">
            <span v-if="!v$.formData.in.required.$response">The field is required*</span>
            <span v-else-if="!v$.formData.in.minValue.$response">You can't use date before today's date*</span>
          </span>
          <span class="error-message" v-if="state.errors.In">{{ state.errors.In[0] }}</span>
        </div>


        <div class="input-form">
          <label>Out: </label>
          <input
              v-model="state.formData.out"
              class="input"
              type="date"
              :readonly="!state.isEditing"
              :min="minOutDate"
              @input="v$.formData.out.$touch()"
          >
          <span class="error-message" v-if="v$.formData.out.$error">
            <span v-if="!v$.formData.out.required.$response">The field is required*</span>
            <span v-else-if="!v$.formData.out.minValue.$response">You can't use date before today's date*</span>
          </span>
          <span class="error-message" v-if="state.errors.Out">{{ state.errors.Out[0] }}</span>
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
                @input="v$.formData.capacity.$touch()"
            >
            <span class="error-message" v-if="v$.formData.capacity.$error">
              <span v-if="!v$.formData.capacity.required.$response">The field is required*</span>
              <span v-else-if="!v$.formData.capacity.numeric.$response">The filed must contain only numeric*</span>
              <span v-else-if="!v$.formData.capacity.minValue.$response">The capacity must be more than 0*</span>
              <span v-else-if="!v$.formData.capacity.maxValue.$response">The capacity must be equal or less than 40*</span>
            </span>
            <span class="error-message" v-if="state.errors.Capacity">{{ state.errors.Capacity[0] }}</span>
          </div>
          <div class="input-form">
            <label>Type: </label>
            <input v-if="!state.isEditing" class="input" type="text" :value="state.roomType" readonly>
            <select v-else v-model="state.formData.idRoomType" class="input" @change="v$.formData.idRoomType.$touch()">
              <option disabled value="">Select type</option>
              <option v-for="roomType in state.roomTypes" :key="roomType.idType" :value="roomType.idType">{{ roomType.title }}</option>
            </select>
            <span class="error-message" v-if="v$.formData.idRoomType.$error">
              <span v-if="!v$.formData.idRoomType.required.$response">The field is required*</span>
            </span>
            <span class="error-message" v-if="state.errors.IdRoomType">{{ state.errors.IdRoomType[0] }}</span>
          </div>
        </div>

        <div class="input-form">
          <label>Room Selection: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.selectedRoom" readonly>
          <select v-else v-model="state.formData.idRoom" @change="v$.formData.idRoom.$touch()">
            <option disabled value="">Select a room</option>
            <option v-for="room in sortedFilteredRooms" :key="room.idRoom" :value="room.idRoom">{{ room.number }} - Capacity: {{ room.capacity }}</option>
          </select>
          <span class="error-message" v-if="v$.formData.idRoom.$error">
              <span v-if="!v$.formData.idRoom.required.$response">The field is required*</span>
            </span>
          <span class="error-message" v-if="state.errors.IdRoom">{{ state.errors.IdRoom[0] }}</span>
          <label>Price: {{state.formData.price}}</label>
        </div>
      </div>

      <div class="guest">
        <div class="input-form">
          <label>Guest Selection: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.selectedGuest" readonly>
          <select v-else v-model="state.formData.idGuest" @change="v$.formData.idGuest.$touch()">
            <option disabled value="">Select a guest</option>
            <option v-for="guest in state.guests" :key="guest.idPerson" :value="guest.idPerson">
              {{ guest.name }} {{ guest.surname }}, {{ guest.passport }}
            </option>
          </select>
          <span class="error-message" v-if="v$.formData.idGuest.$error">
              <span v-if="!v$.formData.idGuest.required.$response">The field is required*</span>
            </span>
          <span class="error-message" v-if="state.errors.IdGuest">{{ state.errors.IdGuest[0] }}</span>
          <router-link v-if="state.isEditing" class="form-btn" to="/new-guest">Add new guest</router-link>
        </div>
      </div>

      <div class="guest">
        <div class="input-form">
          <label>Deposit</label>
          <template v-if="!state.isEditing && !state.hasDeposit">
            <label>There is no deposit.</label>
          </template>
          <template v-else-if="state.hasDeposit">
            <label>Deposit sum: </label>
            <input
                v-model="state.formData.depositSum"
                class="input"
                type="number"
                placeholder="Enter deposit sum"
                :readonly="!state.isEditing"
                @change="v$.formData.depositSum.$touch()"
            >
            <span class="error-message" v-if="v$.formData.depositSum.$error">
              <span v-if="!v$.formData.depositSum.required.$response">The field is required*</span>
              <span v-else-if="!v$.formData.depositSum.numeric.$response">The field can contain only digits*</span>
            </span>
            <span class="error-message" v-if="state.errors.DepositSum">{{ state.errors.DepositSum[0] }}</span>

            <label>Choose type: </label>
            <input v-if="!state.isEditing" class="input" type="text" :value="state.selectedDepositType" readonly>
            <select v-else v-model="state.formData.idDepositType" @change="v$.formData.idDepositType.$touch()">
              <option disabled value="0">Select type</option>
              <option v-for="type in state.depositTypes" :key="type.idType" :value="type.idType">
                {{ type.title }}
              </option>
            </select>
            <span class="error-message" v-if="v$.formData.idDepositType.$error">
              <span v-if="!v$.formData.idDepositType.required.$response">The field is required*</span>
            </span>
            <span class="error-message" v-if="state.errors.IdDepositType">{{ state.errors.IdDepositType[0] }}</span>
          </template>
          <template v-else>
            <label>There is no deposit.</label>
          </template>
          <button v-if="state.isEditing" @click.prevent="switchDeposit" class="form-btn">{{ state.hasDeposit ? 'Delete deposit' : 'Add deposit'}}</button>
        </div>
      </div>

      <div class="guest">
        <label>Service option</label>
        <div class="input-form">
          <label v-if="state.isEditing">Choose a service</label>
          <label v-if="!state.isEditing && state.formData.services && state.formData.services.length > 0">Added services</label>
          <label v-if="!state.isEditing && (!state.formData.services || state.formData.services.length === 0)">There are no services</label>
          <select v-if="state.isEditing" v-model="state.selectedService" class="input">
            <option disabled value="" selected>Select a service</option>
            <option v-for="service in state.services" :key="service.idService" :value="service">
              {{ service.title }}: {{ service.sum }}
            </option>
          </select>
          <button v-if="state.isEditing" @click.prevent="addService" class="form-btn">Add</button>
          <div class="service-list" v-if="state.formData.services && state.formData.services.length > 0">
            <ul class="added-services-list">
              <li class="element" v-for="(service, index) in state.formData.services" :key="index">
                <span>{{ service.title }}: {{ service.sum }} $</span>
                <button class="btn" v-if="state.isEditing" @click.prevent="removeService(index)">Remove</button>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <div class="registration-class">
        <router-link class="registration-btn" to="/arrivals">Cancel</router-link>
        <button v-if="!state.isEditing" class="registration-btn" type="button" @click="confirmReservation">Confirm</button>
        <button class="registration-btn" type="button" @click="toggleEdit">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
      </div>
    </form>
  </div>
</template>

<script>
import {computed, reactive, watch} from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue, minValue } from '@vuelidate/validators';
import axios from 'axios';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import {differenceInCalendarDays} from "date-fns";

export default {
  name: "ArrivalDetails",
  props: {
    idReservation: {
      type: Number,
      required: true
    },
    detailsType: {
      type: String,
      default: 'Arrival'
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
      selectedService: '',
      selectedRoomType: '',
      selectedDepositType: '',
      rooms: [],
      roomType: '',
      roomTypes: [],
      guests: [],
      depositTypes: [],
      isEditing: false,
      roomPrice: 0,
      minInDate: new Date().toISOString().split('T')[0],
      minOutDate: '',
      errors: {},
      hasDeposit: false
    });

    const rules = {
      formData: {
        in: { required, minValue: val => new Date(val) >= new Date(today) },
        out: { required, minValue: val => new Date(val) > new Date(state.formData.in) },
        capacity: { required, numeric, minValue: value => value > 0, maxValue: value => value <= 40 },
        idRoom: { required },
        idRoomType: { required },
        idGuest: { required },
        idDepositType: {
          required: () => !state.hasDeposit
        },
        depositSum: {
          required: () => !state.hasDeposit,
          numeric: () => !state.hasDeposit,
        },
      },
    };

    const v$ = useVuelidate(rules, state);

    async function toggleEdit() {
      if (state.isEditing) {
        v$.value.$touch();
        console.log(v$)
        if (!v$.value.$error) {
          console.log(state.formData)
          try {
            const response = await axios.put('https://localhost:44384/api/Reservation', state.formData, {
              headers: {
                'Authorization': `Bearer ${store.getters.getToken}`
              },
            });
            console.log('Success:', response.data);
            state.isEditing = false;

            const foundGuest = state.guests.find(guest => guest.idPerson === state.formData.idGuest)
            state.selectedGuest = foundGuest.name + " " + foundGuest.surname + ", " + foundGuest.passport

            const foundDeposit = state.depositTypes.find(guest => guest.idType === state.formData.idDepositType)
            if(foundDeposit !== undefined){
              state.selectedDepositType = foundDeposit.title
            }

            const foundRoom = state.rooms.find(status => status.idRoom === state.formData.idRoom);
            state.selectedRoom =  foundRoom.number + " - Capacity: " + foundRoom.capacity;
            state.roomType = foundRoom.type;
          } catch (error) {
            console.log('Error:', error);
          }
        }
      } else {
        state.isEditing = true;
      }
    }

    async function fetchReservation(idReservation){
      console.log(idReservation)
      try{
        const responseReservation = await axios.get('https://localhost:44384/api/Reservation/arrival/' + idReservation,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });

        state.formData = responseReservation.data;
        state.formData.services = state.formData.services || [];
        state.formData.in = isoToLocalDate(state.formData.in);
        state.formData.out = isoToLocalDate(state.formData.out);


        const responseRooms = await axios.get('https://localhost:44384/api/Room/freeRooms?idRoom=' + state.formData.idRoom,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.rooms = responseRooms.data;

        const responseRoomTypes = await axios.get('https://localhost:44384/api/RoomType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.roomTypes = responseRoomTypes.data;

        const foundRoom = state.rooms.find(status => status.idRoom === state.formData.idRoom);
        state.selectedRoom =  foundRoom.number + " - Capacity: " + foundRoom.capacity;
        state.roomType = foundRoom.type;

        const responseGuests = await axios.get('https://localhost:44384/api/Guest',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.guests = responseGuests.data.list;

        const foundGuest = state.guests.find(guest => guest.idPerson === state.formData.idGuest)
        state.selectedGuest = foundGuest.name + " " + foundGuest.surname + ", " + foundGuest.passport

        const responseDepositType = await axios.get('https://localhost:44384/api/DepositType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.depositTypes = responseDepositType.data;

        const foundDeposit = state.depositTypes.find(guest => guest.idType === state.formData.idDepositType)
        if(foundDeposit !== undefined){
          state.selectedDepositType = foundDeposit.title
        }

        const responseServices = await axios.get('https://localhost:44384/api/Service',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.services = responseServices.data.list;


        const nights = differenceInCalendarDays(new Date(state.formData.out), new Date(state.formData.in));
        state.roomPrice = foundRoom.price * nights;
        state.hasDeposit = state.formData.idDepositType !== 0;
        console.log(state.formData)
      } catch (error) {
        console.log(error);
      }
    }

    async function confirmReservation() {
      try {
        const confirmResponse = await axios.put('https://localhost:44384/confirm/' + state.formData.idReservation,
            {
              headers: {
                'Authorization': `Bearer ${store.getters.getToken}`
              },
            }
        );

        console.log('Confirmation Success:', confirmResponse.data);
        await router.push('/arrivals');
      } catch (error) {
        console.error('Confirmation Error:', error);
      }
    }


    const removeService = (index) => {
      state.formData.services.splice(index, 1);
    };

    const addService = () => {
      console.log(state.formData.services);
      if (state.selectedService && !state.formData.services.some(service => service.idService === state.selectedService.idService)) {
        state.formData.services.push(state.selectedService);
      }
    };


    watch(() => state.formData.capacity, (newCapacity) => {
      const selectedRoom = state.rooms.find(room => room.idRoom === state.formData.idRoom);
      if (selectedRoom && selectedRoom.capacity < newCapacity) {
        state.formData.idRoom = '';
      }
    });

    watch(() => state.formData.idRoomType, (newRoomType) => {
      const selectedRoom = state.rooms.find(room => room.idRoom === state.formData.idRoom);
      const selectedRoomType = state.roomTypes.find(type => type.idType === newRoomType);
      if (selectedRoom && selectedRoomType && selectedRoom.type !== selectedRoomType.title) {
        state.formData.idRoom = '';
      }
    });

    const sortedFilteredRooms = computed(() => {
      let filteredRooms = state.rooms;
      if (state.formData.idRoomType) {
        const selectedRoomTypeTitle = state.roomTypes.find(type => type.idType === parseInt(state.formData.idRoomType))?.title;
        filteredRooms = filteredRooms.filter(room => room.type === selectedRoomTypeTitle);
      }
      if (state.formData.capacity) {
        filteredRooms = filteredRooms.filter(room => room.capacity >= parseInt(state.formData.capacity));
      }
      return filteredRooms.sort((a, b) => a.capacity - b.capacity);
    });

    function switchDeposit() {
      state.hasDeposit = !state.hasDeposit;
      state.formData.depositSum = 0;
      state.formData.idDepositType = 0;

      if (state.errors.depositSum) {
        delete state.errors.depositSum;
      }
      if (state.errors.idDepositType) {
        delete state.errors.idDepositType;
      }
    }

    function isoToLocalDate(isoStr) {
      let date = new Date(isoStr);
      let localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
      return localDate.toISOString().split('T')[0];
    }

    watch([() => state.formData.out, () => state.formData.in, () => state.formData.idRoom], ([newOut, newIn, newIdRoom]) => {
      if (newOut && newIn && newIdRoom) {
        const outDate = new Date(newOut);
        const inDate = new Date(newIn);
        const nights = differenceInCalendarDays(outDate, inDate);

        const selectedRoom = state.rooms.find(room => room.idRoom === newIdRoom);
        if (selectedRoom && nights >= 0) {
          state.formData.price = selectedRoom.price * nights;
        }
      }
    });

    const minOutDate = computed(() => {
      if (state.formData.in) {
        const inDate = new Date(state.formData.in);
        inDate.setDate(inDate.getDate() + 1);
        return inDate.toISOString().split('T')[0];
      }
      return '';
    });


    watch(() => state.formData.in, (newIn) => {
      const newInDate = new Date(newIn);
      const outDate = new Date(state.formData.out);

      if (newInDate >= outDate) {
        const newOutDate = new Date(newInDate);
        newOutDate.setDate(newOutDate.getDate() + 1);
        state.formData.out = newOutDate.toISOString().split('T')[0];
      }
      state.minOutDate = minOutDate.value;
    });


    return {
      state,
      today,
      fetchReservation,
      removeService,
      addService,
      toggleEdit,
      sortedFilteredRooms,
      switchDeposit,
      minOutDate,
      confirmReservation,
      v$
    }
  }
  ,
  mounted() {
    this.fetchReservation(this.idReservation);
  },
  computed(){

  }
}
</script>

<style scoped>

.main {
  display: flex;
  align-items: center;
  flex-direction: column;
  flex-grow: 1;
  padding-top: 2vh;
  margin: 5%;
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