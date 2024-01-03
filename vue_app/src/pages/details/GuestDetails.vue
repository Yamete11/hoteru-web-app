<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>Guest Details</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Name: </label>
          <input v-model="state.formData.name"
                 class="input"
                 type="text"
                 placeholder="Enter name"
                 :readonly="!state.isEditing"
                 @input="v$.formData.name.$touch()"
          >
          <span class="error-message" v-if="v$.formData.name.$error">
            <span v-if="!v$.formData.name.required.$response">Name is required*</span>
            <span v-else-if="!v$.formData.name.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.name.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Name">{{ state.errors.Name[0] }}</span>
        </div>

        <div class="input-form">
          <label>Surname: </label>
          <input v-model="state.formData.surname"
                 class="input"
                 type="text"
                 placeholder="Enter room capacity"
                 :readonly="!state.isEditing"
                 @input="v$.formData.surname.$touch()"
          >
          <span class="error-message" v-if="v$.formData.surname.$error">
            <span v-if="!v$.formData.surname.required.$response">Surname is required*</span>
            <span v-else-if="!v$.formData.surname.maxLength.$response">Surname must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.surname.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Surname">{{ state.errors.Surname[0] }}</span>
        </div>

        <div class="input-form">
          <label>Email: </label>
          <input v-model="state.formData.email"
                 class="input"
                 type="text"
                 placeholder="Enter room price"
                 :readonly="!state.isEditing"
                 @input="v$.formData.email.$touch()"
          >
          <span class="error-message" v-if="v$.formData.email.$error">
            <span v-if="!v$.formData.email.required.$response">Email is required*</span>
            <span v-if="!v$.formData.email.email.$response">Invalid email format*</span>
          </span>
          <span class="error-message" v-if="state.errors.Email">{{ state.errors.Email[0] }}</span>
        </div>

        <div class="input-form">
          <label>Tel. number: </label>
          <input v-model="state.formData.telNumber"
                 class="input"
                 type="text"
                 placeholder="Enter room price"
                 :readonly="!state.isEditing"
                 @input="v$.formData.telNumber.$touch()"
          >
          <span class="error-message" v-if="v$.formData.telNumber.$error">
            <span v-if="!v$.formData.telNumber.required.$response">Telephone number is required*</span>
            <span v-if="!v$.formData.telNumber.numeric.$response">Telephone number must be a number*</span>
            <span v-if="!v$.formData.telNumber.maxLength.$response">Telephone number must be less than 15 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.TelNumber">{{ state.errors.TelNumber[0] }}</span>
        </div>

        <div class="input-form">
          <label>Passport: </label>
          <input v-model="state.formData.passport"
                 class="input"
                 type="text"
                 placeholder="Enter room price"
                 :readonly="!state.isEditing"
                 @input="v$.formData.passport.$touch()"
          >
          <span class="error-message" v-if="v$.formData.passport.$error">
            <span v-if="!v$.formData.passport.required.$response">Passport number is required*</span>
            <span v-if="!v$.formData.passport.maxLength.$response">Passport number must be less than 10 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Passport">{{ state.errors.Passport[0] }}</span>
        </div>

        <div class="input-form">
          <label>Status: </label>
          <input v-if="!state.isEditing" class="input" type="text" :value="state.statusTitle" readonly>
          <select v-else v-model="state.formData.idGuestStatus" class="input" @change="v$.formData.idGuestStatus.$touch()">
            <option v-for="status in state.guestStatuses" :value="status.idStatus" :key="status.idStatus">{{ status.title }}</option>
          </select>
          <span class="error-message" v-if="v$.formData.idGuestStatus.$error">
            <span v-if="!v$.formData.idGuestStatus.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.IdGuestStatus">{{ state.errors.IdGuestStatus[0] }}</span>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" to="/guests">Back</router-link>
          <button type="button" class="registration-btn" @click="toggleEdit">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
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
import {email} from "@vuelidate/validators";

export default {
  name: "GuestDetails",
  props: {
    idPerson: {
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
      errors: {}
    })

    function onlyLetters(value) {
      return /^[A-Za-z]+$/.test(value);
    }

    const rules = {
      formData: {
        name: { required, maxLength: maxLength(20), onlyLetters },
        surname: { required, maxLength: maxLength(20), onlyLetters},
        email: { required, email },
        telNumber: { required, maxLength: maxLength(15) },
        passport: { required, maxLength: maxLength(10) },
        idGuestStatus: { required }
      }
    }

    const v$ = useVuelidate(rules, state);

    async function toggleEdit() {
      if (state.isEditing) {
        v$.value.$touch();
        if (!v$.value.$error) {
          state.formData.idGuestStatus = String(state.formData.idGuestStatus);
          try {
            const response = await axios.put('https://localhost:44384/api/Guest', state.formData, {
              headers: {
                'Authorization': `Bearer ${store.getters.getToken}`
              },
            });
            if (response.data.httpStatusCode && response.data.httpStatusCode !== 200) {
              state.errors = response.data.errors || {};
              console.log('Error', response.data.message);
            } else {
              console.log('Success:', response.data);
              state.isEditing = false;
              const foundStatus = state.guestStatuses.find(status => status.idStatus == state.formData.idGuestStatus);
              state.statusTitle = foundStatus ? foundStatus.title : 'Status not found';
            }
          } catch (error) {
            if (error.response && error.response.data && error.response.data.errors) {
              state.errors = error.response.data.errors;
            }
            console.log('Error:', error);
          }
        }
      } else {
        state.isEditing = true;
      }
    }

    async function fetchSpecificGuest(idPerson) {
      try {
        const response = await axios.get('https://localhost:44384/api/Guest/' + idPerson, {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.formData = response.data;

        const responseStatus = await axios.get('https://localhost:44384/api/GuestStatus',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.guestStatuses = responseStatus.data;

        const foundStatus = state.guestStatuses.find(status => status.idStatus === state.formData.idGuestStatus);
        state.statusTitle = foundStatus ? foundStatus.title : 'Status not found';
      } catch (error) {
        console.error(error);
      }
    }
    return { state, v$, toggleEdit, fetchSpecificGuest};

  },

  mounted() {
    this.fetchSpecificGuest(this.idPerson);
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