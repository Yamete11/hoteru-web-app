<template>
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
            <span v-if="!v$.formData.Capacity.numeric.$response">Capacity must be a number*</span>
            <span v-if="!v$.formData.Capacity.maxValue.$response">Capacity must be less than or equal to 10*</span>
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
            <span v-if="!v$.formData.Price.numeric.$response">Price must be a number*</span>
            <span v-if="!v$.formData.Price.maxValue.$response">Price must be less than or equal to 1,000,000*</span>
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
</template>

<script>
import { reactive } from 'vue';
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
        BookedBy: '',
        Deposit: ''
      },
      errors: {}
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

    const v$ = useVuelidate(rules, state);

    return {state, v$}
  }


}
</script>

<style scoped>

</style>