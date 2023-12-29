<template>
  <div class="registration">
    <h1>HOTERU ホテル</h1>
    <form @submit.prevent="create" class="registration-form">
      <div class="input-form">
        <label>First name: </label>
        <input
            v-model="state.formData.Name"
            class="input"
            type="text"
            placeholder="Enter your First name"
            @input="v$.formData.Name.$touch()"
        >
        <span v-if="v$.formData.Name.$error" class="error-message">
          <span v-if="!v$.formData.Name.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Name.maxLength.$response">Name must be less than 50 characters</span>
          <span v-else-if="!v$.formData.Name.onlyLetters.$response">Name must contain only letters</span>
        </span>
      </div>
      <div class="input-form">
        <label>Last name: </label>
        <input
            v-model="state.formData.Surname"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Email: </label>
        <input
            v-model="state.formData.Email"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Login: </label>
        <input
            v-model="state.formData.LoginName"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Password: </label>
        <input
            v-model="state.formData.Password"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Company name: </label>
        <input
            v-model="state.formData.Title"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Country: </label>
        <input
            v-model="state.formData.Country"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>City: </label>
        <input
            v-model="state.formData.City"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Address: </label>
        <input
            v-model="state.formData.Street"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Postcode: </label>
        <input
            v-model="state.formData.Postcode"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>

    </form>
    <div class="registration-class">
      <router-link class="registration-btn" to="/">Cancel</router-link>
      <button class="registration-btn" type="submit" >Confirm</button>
    </div>
  </div>
</template>

<script>
import { reactive } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, email, maxLength, helpers } from '@vuelidate/validators';
import axios from 'axios';
import {useRouter} from "vue-router/dist/vue-router";

export default {
  name: "Registration",
  setup() {
    const router = useRouter();
    const state = reactive({
      formData: {
        Name: '',
        Surname: '',
        Email: '',
        LoginName: '',
        Password: '',
        Title: '',
        City: '',
        Country: '',
        Street: '',
        Postcode: ''
      },
      errors: {}
    });

    function onlyLetters(value) {
      return /^[A-Za-z]+$/.test(value);
    }

    const rules = {
      formData: {
        Name: { required, maxLength: maxLength(50), onlyLetters },
        Surname: { required, maxLength: maxLength(50), onlyLetters },
        Email: { required, email },
        LoginName: { required, maxLength: maxLength(30) },
        Password: { required, maxLength: maxLength(30) },
        Title: { required, maxLength: maxLength(100) },
        City: { required, maxLength: maxLength(100), onlyLetters },
        Country: { required, maxLength: maxLength(100), onlyLetters },
        Street: { required, maxLength: maxLength(100) },
        Postcode: { required, maxLength: maxLength(15) }
      }
    };

    const v$ = useVuelidate(rules, state.formData);

    async function create(){
      v$.value.$validate();
      console.log(state.formData)
      if (!v$.value.$error) {
        try {
          const response = await axios.post('https://localhost:44384/api/Hotel', state.formData);
          console.log('Success:', response.data);
          if (response.data && response.data.httpStatusCode === 200) {
            await router.push('/');
          }
        } catch (error) {
          console.log('Error:', error);
        }
      }
    }
    return { state, create, v$ };
  }
}
</script>

<style scoped>
.registration {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #252525;
}

.registration-form {
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
  cursor: pointer;
}

.registration-class{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 10vw;
}

.input-form {
  display: flex;
  flex-direction: column;
  margin: 5px;
}

.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: #FFFFFF;
}

.input-form input[type="text"] {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px;
}

.error-message {
  color: red;
  margin: 10px 0;
}
</style>

