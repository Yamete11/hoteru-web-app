<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Guest</h1>
      <form @submit.prevent="addGuest" class="creating-form">

        <div class="input-form">
          <label>Name: </label>
          <input
              v-model="state.formData.Name"
              class="input"
              type="text"
              placeholder="Enter name"
              @input="v$.formData.Name.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Name.$error">
            <span v-if="!v$.formData.Name.required.$response">Name is required*</span>
            <span v-else-if="!v$.formData.Name.maxLength.$response">Name must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.Name.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Name">{{ state.errors.Name[0] }}</span>
        </div>



        <div class="input-form">
          <label>Surname: </label>
          <input
              v-model="state.formData.Surname"
              class="input"
              type="text"
              placeholder="Enter surname"
              @input="v$.formData.Surname.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Surname.$error">
            <span v-if="!v$.formData.Surname.required.$response">Surname is required*</span>
            <span v-else-if="!v$.formData.Surname.maxLength.$response">Surname must be less than 20 characters*</span>
            <span v-else-if="!v$.formData.Surname.onlyLetters.$response">Name must contain only letters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Surname">{{ state.errors.Surname[0] }}</span>
        </div>


        <div class="input-form">
          <label>Email: </label>
          <input
              v-model="state.formData.Email"
              class="input"
              type="text"
              placeholder="Enter email"
              @input="v$.formData.Email.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Email.$error">
            <span v-if="!v$.formData.Email.required.$response">Email is required*</span>
            <span v-if="!v$.formData.Email.email.$response">Invalid email format*</span>
          </span>
          <span class="error-message" v-if="state.errors.Email">{{ state.errors.Email[0] }}</span>
        </div>


        <div class="input-form">
          <label>Tel. number: </label>
          <input
              v-model="state.formData.TelNumber"
              class="input"
              type="text"
              placeholder="Enter tel. number"
              @input="v$.formData.TelNumber.$touch()"
          >
          <span class="error-message" v-if="v$.formData.TelNumber.$error">
            <span v-if="!v$.formData.TelNumber.required.$response">Telephone number is required*</span>
            <span v-if="!v$.formData.TelNumber.numeric.$response">Telephone number must be a number*</span>
            <span v-if="!v$.formData.TelNumber.maxLength.$response">Telephone number must be less than 15 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.TelNumber">{{ state.errors.TelNumber[0] }}</span>
        </div>


        <div class="input-form">
          <label>Passport: </label>
          <input
              v-model="state.formData.Passport"
              class="input"
              type="text"
              placeholder="Enter passport number"
              @input="v$.formData.Passport.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Passport.$error">
            <span v-if="!v$.formData.Passport.required.$response">Passport number is required*</span>
            <span v-if="!v$.formData.Passport.maxLength.$response">Passport number must be less than 10 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Passport">{{ state.errors.Passport[0] }}</span>
        </div>


        <div class="input-form">
          <label>Status: </label>
          <select v-model="state.formData.IdGuestStatus" @change="v$.formData.IdGuestStatus.$touch()">
            <option disabled value="">Select status</option>
            <option v-for="guestStatus in state.guestStatuses" :key="guestStatus.idStatus" :value="String(guestStatus.idStatus)">{{ guestStatus.title }}</option>
          </select>
          <span class="error-message" v-if="v$.formData.IdGuestStatus.$error">
            <span v-if="!v$.formData.IdGuestStatus.required.$response">Status is required*</span>
          </span>
          <span class="error-message" v-if="state.errors.IdGuestStatus">{{ state.errors.IdGuestStatus[0] }}</span>
        </div>


        <div class="registration-class">
          <router-link class="registration-btn" to="/guests">Cancel</router-link>
          <button class="registration-btn" type="submit">Confirm</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import {reactive} from "vue";
import {useVuelidate} from "@vuelidate/core";
import {email, numeric, maxLength, required} from "@vuelidate/validators";
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';


export default {
  name: "NewGuest",
  setup() {
    const store = useStore();
    const router = useRouter();
    const state = reactive({
      formData: {
        Name: '',
        Surname: '',
        Email: '',
        TelNumber: '',
        Passport: '',
        IdGuestStatus: ''
      },
      guestStatuses: [],
      errors: {}
    });

    function onlyLetters(value) {
      return /^[A-Za-z]+$/.test(value);
    }

    const rules = {
      formData: {
        Name: { required, maxLength: maxLength(20), onlyLetters },
        Surname: { required, maxLength: maxLength(20), onlyLetters},
        Email: { required, email },
        TelNumber: { required, maxLength: maxLength(15), numeric },
        Passport: { required, maxLength: maxLength(10) },
        IdGuestStatus: { required }
      }
    }

    const v$ = useVuelidate(rules, state);

    async function fetchGuestStatuses() {
      console.log(this.$store.getters.getToken)
      try {
        const response = await axios.get('https://localhost:44384/api/GuestStatus',{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          }
        });
        state.guestStatuses = response.data;
      } catch (error) {
        console.error(error);
      }
    }

    async function addGuest() {
      v$.value.$validate();
      if (!v$.value.$error) {
        console.log(state.formData)
        try {
          const response = await axios.post('https://localhost:44384/api/Guest', state.formData,{
            headers: {
              'Authorization': `Bearer ${store.getters.getToken}`
            }
          });
          console.log('Response:', response.data);
          if (response.data.httpStatusCode && response.data.httpStatusCode !== 200) {
            state.errors = response.data.errors || {};
            console.log('Error', response.data.message);
          } else {
            await router.push('/guests');
          }
        } catch (error) {
          if (error.response && error.response.data && error.response.data.errors) {
            state.errors = error.response.data.errors;
          }
          console.log('Error', error);
        }
      }
    }

    return { state, v$, addGuest, fetchGuestStatuses};
  },
  mounted(){
    this.fetchGuestStatuses();
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