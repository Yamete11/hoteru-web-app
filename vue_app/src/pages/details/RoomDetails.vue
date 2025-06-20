<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>Room Details</h1>
      <form @submit.prevent="toggleEdit" class="creating-form">

        <div class="input-form">
          <label>Number: </label>
          <input v-model="state.formData.number"
                 class="input"
                 type="text"
                 placeholder="Enter room number"
                 :readonly="!state.isEditing"
                 @input="v$.formData.number.$touch()"
                 data-testid="input-number"
          >
          <span class="error-message" v-if="v$.formData.number.$error">
            <span v-if="!v$.formData.number.required.$response">Number is required*</span>
            <span v-if="!v$.formData.number.maxLength.$response">Number must be less than 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Number">{{ state.errors.Number[0] }}</span>
        </div>

        <div class="input-form">
          <label>Capacity: </label>
          <input v-model="state.formData.capacity"
                 class="input"
                 type="number"
                 placeholder="Enter room capacity"
                 :readonly="!state.isEditing"
                 @input="v$.formData.capacity.$touch()"
                 data-testid="input-capacity"
          >
          <span class="error-message" v-if="v$.formData.capacity.$error">
            <span v-if="!v$.formData.capacity.required.$response">Capacity is required*</span>
            <span v-else-if="!v$.formData.capacity.numeric.$response">Capacity must be a number*</span>
            <span v-else-if="!v$.formData.capacity.maxValue.$response">Capacity must be less than or equal to 10*</span>
            <span v-else-if="!v$.formData.capacity.minValue.$response">Capacity must be more than 0*</span>
          </span>
          <span class="error-message" v-if="state.errors.Capacity">{{ state.errors.Capacity[0] }}</span>
        </div>

        <div class="input-form">
          <label>Price: </label>
          <input v-model="state.formData.price"
                 class="input"
                 type="number"
                 placeholder="Enter room price"
                 :readonly="!state.isEditing"
                 @input="v$.formData.price.$touch()"
                 data-testid="input-price"
          >
          <span class="error-message" v-if="v$.formData.price.$error">
            <span v-if="!v$.formData.price.required.$response">Price is required*</span>
            <span v-else-if="!v$.formData.price.numeric.$response">Price must be a number*</span>
            <span v-else-if="!v$.formData.price.maxValue.$response">Price must be less than or equal to 1,000,000*</span>
            <span v-else-if="!v$.formData.price.minValue.$response">Price must be more than 0*</span>
          </span>
          <span class="error-message" v-if="state.errors.Price">{{ state.errors.Price[0] }}</span>
        </div>

        <div class="input-form">
          <label>Type: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.typeTitle" readonly>
          <select v-else v-model="state.formData.type" class="input" @change="v$.formData.type.$touch()" data-testid="room-type-select">
            <option v-for="type in state.roomTypes" :value="type.idType" :key="type.idType" data-testid="room-type-option">{{ type.title }}</option>
          </select>
          <span class="error-message" v-if="v$.formData.type.$error">
            <span v-if="!v$.formData.type.required.$response">Type is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.Type">{{ state.errors.Type[0] }}</span>
        </div>

        <div class="input-form">
          <label>Status: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.statusTitle" readonly>
          <select v-else v-model="state.formData.status" class="input" @change="v$.formData.status.$touch()" data-testid="room-status-select">
            <option v-for="status in state.roomStatuses" :value="status.idStatus" :key="status.idStatus" data-testid="room-status-option">{{ status.title }}</option>
          </select>
          <span class="error-message" v-if="v$.formData.status.$error">
            <span v-if="!v$.formData.status.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.Status">{{ state.errors.Status[0] }}</span>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" to="/rooms" data-testid="btn-back">Back</router-link>
          <button type="submit" class="registration-btn" data-testid="btn-submit">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
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
  name: "RoomDetails",
  props: {
    idRoom: {
      type: Number,
      required: true
    }
  },
  setup() {
    const store = useStore();
    const router = useRouter();
    const state = reactive( {
      isEditing: false,
      formData: {
        idRoom: '',
        number: '',
        capacity: 0,
        price: 0,
        type: '',
        status: ''
      },
      typeTitle: '',
      statusTitle: '',
      roomTypes: [],
      roomStatuses: [],
      errors: {}
    });

    const rules = {
      formData: {
        number: { required, maxLength: maxLength(20) },
        capacity: { required, numeric, maxValue: maxValue(10), minValue: minValue(1) },
        price: { required, numeric, maxValue: maxValue(1000000), minValue: minValue(1) },
        type: { required },
        status: { required }
      }
    };

    const v$ = useVuelidate(rules, state);

    async function toggleEdit() {
      if (state.isEditing) {
        v$.value.$touch();
        if (!v$.value.$error) {
          state.formData.type = String(state.formData.type);
          state.formData.status = String(state.formData.status);
          try {
            const response = await axios.put('https://localhost:44384/api/Room', state.formData, {
              headers: {
                'Authorization': `Bearer ${store.getters.getToken}`
              },
            });
            console.log('Success:', response.data);
            state.isEditing = false;

            const foundType = state.roomTypes.find(type => type.idType == state.formData.type);
            console.log(foundType)
            state.typeTitle = foundType ? foundType.title : 'Type not found';

            const foundStatus = state.roomStatuses.find(status => status.idStatus == state.formData.status);
            state.statusTitle = foundStatus ? foundStatus.title : 'Status not found';

          } catch (error) {
            console.log('Error:', error);

          }
        }
      } else {
        state.isEditing = true;
      }
    }

    async function fetchSpecificRoom(idRoom) {
      try {
        const response = await axios.get('https://localhost:44384/api/Room/' + idRoom,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.formData = response.data;

        console.log(state.formData)

        const responseType = await axios.get('https://localhost:44384/api/RoomType',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.roomTypes = responseType.data;

        const responseStatus = await axios.get('https://localhost:44384/api/RoomStatus',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.roomStatuses = responseStatus.data;

        const foundType = state.roomTypes.find(type => type.idType === state.formData.type);
        state.typeTitle = foundType ? foundType.title : 'Type not found';

        const foundStatus = state.roomStatuses.find(status => status.idStatus === state.formData.status);
        state.statusTitle = foundStatus ? foundStatus.title : 'Status not found';

        console.log(state.roomTypes)
      } catch (error) {
        console.error(error);
      }
    }

    return { state, v$, toggleEdit, fetchSpecificRoom };

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

.input-form input[type="text"],
.input-form input[type="number"]{
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