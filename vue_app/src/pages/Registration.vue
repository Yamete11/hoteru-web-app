<template>
  <div class="registration">
    <h1>HOTERU ホテル</h1>
    <form @submit.prevent="addCompany" class="registration-form">
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
        <span class="error-message" v-if="state.errors.Name">{{ state.errors.Name[0] }}</span>
      </div>
      <div class="input-form">
        <label>Last name: </label>
        <input
            v-model="state.formData.Surname"
            class="input"
            type="text"
            placeholder="Enter your last name"
            @input="v$.formData.Surname.$touch()"
        >
        <span v-if="v$.formData.Surname.$error" class="error-message">
          <span v-if="!v$.formData.Surname.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Surname.maxLength.$response">Name must be less than 20 characters</span>
          <span v-else-if="!v$.formData.Surname.onlyLetters.$response">Name must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.Surname">{{ state.errors.Surname[0] }}</span>

      </div>
      <div class="input-form">
        <label>Email: </label>
        <input
            v-model="state.formData.Email"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.Email.$error" class="error-message">
          <span v-if="!v$.formData.Email.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Email.email.$response">Email is not properly written</span>
        </span>
        <span class="error-message" v-if="state.errors.Email">{{ state.errors.Email[0] }}</span>
      </div>

      <div class="input-form">
        <label>Login: </label>
        <input
            v-model="state.formData.LoginName"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.LoginName.$error" class="error-message">
          <span v-if="!v$.formData.LoginName.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.LoginName.maxLength.$response">Name must be less than 20 characters</span>
          <span v-else-if="!v$.formData.LoginName.onlyLetters.$response">Name must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.LoginName">{{ state.errors.LoginName[0] }}</span>
      </div>


      <div class="input-form">
        <label>Password: </label>
        <input
            v-model="state.formData.Password"
            class="input"
            type="password"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.Password.$error" class="error-message">
          <span v-if="!v$.formData.Password.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Password.maxLength.$response">Name must be less than 20 characters</span>
          <span v-else-if="!v$.formData.Password.onlyLetters.$response">Name must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.Password">{{ state.errors.Password[0] }}</span>
      </div>


      <div class="input-form">
        <label>Company name: </label>
        <input
            v-model="state.formData.Title"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.Title.$error" class="error-message">
          <span v-if="!v$.formData.Title.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Title.maxLength.$response">Title must be less than 20 characters</span>
          <span v-else-if="!v$.formData.Title.onlyLetters.$response">Title must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.Title">{{ state.errors.Title[0] }}</span>
      </div>


      <div class="input-form">
        <label>Country: </label>
        <input
            v-model="state.formData.Country"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.Country.$error" class="error-message">
          <span v-if="!v$.formData.Country.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Country.maxLength.$response">Title must be less than 20 characters</span>
          <span v-else-if="!v$.formData.Country.onlyLetters.$response">Title must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.Country">{{ state.errors.Country[0] }}</span>
      </div>


      <div class="input-form">
        <label>City: </label>
        <input
            v-model="state.formData.City"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.City.$error" class="error-message">
          <span v-if="!v$.formData.City.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.City.maxLength.$response">Title must be less than 20 characters</span>
          <span v-else-if="!v$.formData.City.onlyLetters.$response">Title must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.City">{{ state.errors.City[0] }}</span>
      </div>


      <div class="input-form">
        <label>Street: </label>
        <input
            v-model="state.formData.Street"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.Street.$error" class="error-message">
          <span v-if="!v$.formData.Street.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Street.maxLength.$response">Title must be less than 20 characters</span>
          <span v-else-if="!v$.formData.Street.onlyLetters.$response">Title must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.Street">{{ state.errors.Street[0] }}</span>
      </div>


      <div class="input-form">
        <label>Postcode: </label>
        <input
            v-model="state.formData.Postcode"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
        <span v-if="v$.formData.Postcode.$error" class="error-message">
          <span v-if="!v$.formData.Postcode.required.$response">This field is required</span>
          <span v-else-if="!v$.formData.Postcode.maxLength.$response">Title must be less than 20 characters</span>
          <span v-else-if="!v$.formData.Postcode.onlyLetters.$response">Title must contain only letters</span>
        </span>
        <span class="error-message" v-if="state.errors.Postcode">{{ state.errors.Postcode[0] }}</span>
      </div>
      <div class="registration-class">
        <router-link class="registration-btn" to="/">Cancel</router-link>
        <button class="registration-btn" type="submit">Confirm</button>
      </div>
    </form>
  </div>
</template>

<script>
import { reactive } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, email, maxLength } from '@vuelidate/validators';
import axios from 'axios';
import {useRouter} from "vue-router/dist/vue-router";
import {useStore} from "vuex";

export default {
  name: "Registration",
  setup() {
    const store = useStore();

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
        Name: { required, maxLength: maxLength(20), onlyLetters },
        Surname: { required, maxLength: maxLength(20), onlyLetters },
        Email: { required, email },
        LoginName: { required, maxLength: maxLength(20) },
        Password: { required, maxLength: maxLength(20) },
        Title: { required, maxLength: maxLength(20) },
        City: { required, maxLength: maxLength(20), onlyLetters },
        Country: { required, maxLength: maxLength(20), onlyLetters },
        Street: { required, maxLength: maxLength(20) },
        Postcode: { required, maxLength: maxLength(15) }
      }
    };

    const v$ = useVuelidate(rules, state);


    async function addCompany(){
      v$.value.$validate();
      console.log(state.formData)
      console.log(v$.value.$error)
      if (!v$.value.$error) {
        try {
          const response = await axios.post('https://localhost:44384/api/Hotel', state.formData, {
            headers: {
              'Authorization': `Bearer ${store.getters.getToken}`
            }
          });
          console.log('Success:', response.data);
          if (response.data.httpStatusCode && response.data.httpStatusCode !== 200) {
            state.errors = response.data.errors || {};
            console.log('Error', response.data.message);
          } else {
            await router.push('/');
          }
        } catch (error) {
          if (error.response && error.response.data && error.response.data.errors) {
            state.errors = error.response.data.errors;
          }
          console.log('Error', error);
        }
      }
    }
    return { state, addCompany, v$ };
  }
}
</script>

<style scoped>
.registration {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #252525;
}


.registration-form {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 300px;
}

.registration-btn{
  display: flex;
  justify-content: space-between;
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
  cursor: pointer;
  margin: 5px;
}

.registration-class{
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  max-width: 300px;
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

.input-form input[type="text"],
.input-form input[type="password"]{
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

