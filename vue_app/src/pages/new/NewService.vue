<template>
  <div class="newService-component">
    <notifications position="top right" />
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Service</h1>
      <form @submit.prevent="addService" class="creating-form">

        <div class="input-form">
          <label>Title*: </label>
          <input
              data-testid="service-title"
              v-model="state.formData.Title"
              class="input"
              type="text"
              placeholder="Enter service title"
              @input="v$.formData.Title.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Title.$error">
            <span v-if="!v$.formData.Title.required.$response">Title is required*</span>
            <span v-else-if="!v$.formData.Title.maxLength.$response">Title must be less than 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Title">{{ state.errors.Title[0] }}</span>
        </div>


        <div class="input-form">
          <label>Price*: </label>
          <input
              data-testid="service-price"
              v-model.number="state.formData.Sum"
              class="input"
              type="text"
              placeholder="Enter service price"
              @input="v$.formData.Sum.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Sum.$error">
          <span v-if="!v$.formData.Sum.numeric.$response">Price must be a number*</span>
          <span v-else-if="!v$.formData.Sum.required.$response">Price is required*</span>
          <span v-else-if="!v$.formData.Sum.minValue.$response">Price must be at least 1*</span>
          <span v-else-if="!v$.formData.Sum.maxValue.$response">Price must not exceed 1,000,000*</span>
          </span>
          <span class="error-message" v-if="state.errors.Sum">{{ state.errors.Sum[0] }}</span>
        </div>



        <div class="input-form">
          <label>Description: </label>
          <input
              data-testid="service-description"
              v-model="state.formData.Description"
              class="input"
              type="text"
              placeholder="Enter service description"
              @input="v$.formData.Description.$touch()"
          >
          <span class="error-message" v-if="v$.formData.Description.$error">Description can have only 50 symbols</span>
          <span class="error-message" v-if="state.errors.Description">{{ state.errors.Description[0] }}</span>
        </div>


        <div class="registration-class">
          <router-link class="registration-btn" to="/services">Cancel</router-link>
          <button class="registration-btn" type="submit" data-testid="service-confirm-button">Confirm</button>
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
import { notify } from '@kyvg/vue3-notification';


export default {
  name: "NewService",
  setup() {
    const store = useStore();
    const router = useRouter();
    const state = reactive({
      formData: {
        Title: '',
        Sum: 0,
        Description: '',
      },
      errors: {}
    });

    const rules = {
      formData: {
        Title: { required, maxLength: maxLength(20) },
        Sum: { required, numeric, minValue: minValue(1), maxValue: maxValue(1000000) },
        Description: { maxLength: maxLength(50) }
      }
    };

    const v$ = useVuelidate(rules, state);

    async function addService() {
      v$.value.$validate();
      if (!v$.value.$error) {
        try {
          const response = await axios.post('https://localhost:44384/api/Service', state.formData, {
            headers: {
              'Authorization': `Bearer ${store.getters.getToken}`
            }
          });
          console.log("Ответ сервера:", response);

          if (response.data && response.data.httpStatusCode === 200) {
            await router.push({
              path: '/services',
              query: { created: 'true' }
            });

          }

        } catch (error) {
          if (error.response && error.response.data && error.response.data.errors) {
            state.errors = error.response.data.errors;
          }
          console.log('Error', error);
        }
      }
    }


    return { state, v$, addService };
  }
}
</script>

<style scoped>
.newService-component {
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