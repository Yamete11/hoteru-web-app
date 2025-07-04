<template>
  <div class="newRoom-component" data-testid="new-room-component">
    <navbar data-testid="navbar"></navbar>
    <sidebar data-testid="sidebar"></sidebar>
    <div class="main" data-testid="main-content">
      <h1 data-testid="form-title">New Room</h1>
      <form @submit.prevent="addRoom" class="creating-form" data-testid="room-form">

        <div class="input-form" data-testid="input-number">
          <label>Number: </label>
          <input
              v-model="state.formData.Number"
              class="input"
              type="text"
              placeholder="Enter room number"
              @input="v$.formData.Number.$touch()"
              data-testid="number-input"
          >
          <span class="error-message" v-if="v$.formData.Number.$error" data-testid="error-number-validation">
            <span v-if="!v$.formData.Number.required.$response">Number is required*</span>
            <span v-if="!v$.formData.Number.maxLength.$response">Number must be less than 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Number" data-testid="error-number-server">{{ state.errors.Number[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-capacity">
          <label>Capacity: </label>
          <input
              v-model="state.formData.Capacity"
              class="input"
              type="number"
              placeholder="Enter room capacity"
              @input="v$.formData.Capacity.$touch()"
              data-testid="capacity-input"
          >
          <span class="error-message" v-if="v$.formData.Capacity.$error" data-testid="error-capacity-validation">
            <span v-if="!v$.formData.Capacity.required.$response">Capacity is required*</span>
            <span v-else-if="!v$.formData.Capacity.numeric.$response">Capacity must be a number*</span>
            <span v-else-if="!v$.formData.Capacity.maxValue.$response">Capacity must be less than or equal to 10*</span>
            <span v-else-if="!v$.formData.Capacity.minValue.$response">Capacity must be more than 0*</span>
          </span>
          <span class="error-message" v-if="state.errors.Capacity" data-testid="error-capacity-server">{{ state.errors.Capacity[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-price">
          <label>Price: </label>
          <input
              v-model="state.formData.Price"
              class="input"
              type="number"
              placeholder="Enter room price"
              @input="v$.formData.Price.$touch()"
              data-testid="price-input"
          >
          <span class="error-message" v-if="v$.formData.Price.$error" data-testid="error-price-validation">
            <span v-if="!v$.formData.Price.required.$response">Price is required*</span>
            <span v-else-if="!v$.formData.Price.numeric.$response">Price must be a number*</span>
            <span v-else-if="!v$.formData.Price.maxValue.$response">Price must be less than or equal to 1,000,000*</span>
            <span v-else-if="!v$.formData.Price.minValue.$response">Price must be more than 0*</span>
          </span>
          <span class="error-message" v-if="state.errors.Price" data-testid="error-price-server">{{ state.errors.Price[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-type">
          <label>Type: </label>
          <select
              v-model="state.formData.Type"
              @change="v$.formData.Type.$touch()"
              data-testid="type-select"
          >
            <option disabled value="">Select type</option>
            <option
                v-for="roomType in state.roomTypes"
                :key="roomType.idType"
                :value="String(roomType.idType)"
                data-testid="type-option"
            >
              {{ roomType.title }}
            </option>
          </select>
          <span class="error-message" v-if="v$.formData.Type.$error" data-testid="error-type-validation">
            <span v-if="!v$.formData.Type.required.$response">Type is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.Type" data-testid="error-type-server">{{ state.errors.Type[0] }}</span>
        </div>

        <div class="input-form" data-testid="input-status">
          <label>Status: </label>
          <select
              v-model="state.formData.Status"
              @change="v$.formData.Status.$touch()"
              data-testid="status-select"
          >
            <option disabled value="">Select status</option>
            <option
                v-for="roomStatus in state.roomStatuses"
                :key="roomStatus.idStatus"
                :value="String(roomStatus.idStatus)"
                data-testid="status-option"
            >
              {{ roomStatus.title }}
            </option>
          </select>
          <span class="error-message" v-if="v$.formData.Status.$error" data-testid="error-status-validation">
            <span v-if="!v$.formData.Status.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.Status" data-testid="error-status-server">{{ state.errors.Status[0] }}</span>
        </div>

        <div class="registration-class" data-testid="form-actions">
          <router-link class="registration-btn" to="/rooms" data-testid="cancel-button">Cancel</router-link>
          <button class="registration-btn" type="submit" data-testid="submit-button">Confirm</button>
        </div>
      </form>
    </div>
  </div>
</template>


<script>
import { reactive } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, numeric, maxLength, maxValue, minValue } from '@vuelidate/validators';
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
        Capacity: 0,
        Price: 0,
        Type: '',
        Status: ''
      },
      errors: {}
    });

    const rules = {
      formData: {
        Number: { required, maxLength: maxLength(20) },
        Capacity: { required, numeric, maxValue: maxValue(10), minValue: minValue(1) },
        Price: { required, numeric, maxValue: maxValue(1000000), minValue: minValue(1) },
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
      v$.value.$touch();
      if (!v$.value.$error) {
        try {
          const response = await axios.post('https://localhost:44384/api/Room', state.formData, {
            headers: {
              'Authorization': `Bearer ${store.getters.getToken}`
            }
          });

          if (response.data.httpStatusCode && response.data.httpStatusCode !== 200) {
            state.errors = response.data.errors || {};
          } else {
            await router.push({
              path: '/rooms',
              query: { created: 'true' }
            });
          }
        } catch (error) {
          if (error.response?.data?.errors) {
            state.errors = error.response.data.errors;
          }
          console.log('Error', error);
        }
      }
    }

    return { state, v$, addRoom, fetchRoomTypes};
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

.input-form input[type="text"],
.input-form input[type="number"] {
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