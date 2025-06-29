<template>
  <div class="newService-component">
    <notifications position="top right" />
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>Service Details</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Title: </label>
          <input
              v-model="state.formData.title"
              class="input"
              type="text"
              placeholder="Enter room number"
              :readonly="!state.isEditing"
              @input="v$.formData.title.$touch()"
              data-testid="input-title"
          >
          <span class="error-message" v-if="v$.formData.title.$error">
            <span v-if="!v$.formData.title.required.$response">Title is required*</span>
            <span v-else-if="!v$.formData.title.maxLength.$response">Title must be less than 20 characters*</span>
          </span>
          <span class="error-message" v-if="state.errors.Title">{{ state.errors.Title[0] }}</span>
        </div>

        <div class="input-form">
          <label>Price: </label>
          <input
              v-model="state.formData.sum"
              class="input"
              type="text"
              placeholder="Enter room capacity"
              :readonly="!state.isEditing"
              @input="v$.formData.sum.$touch()"
              data-testid="input-price"
          >
          <span class="error-message" v-if="v$.formData.sum.$error">
          <span v-if="!v$.formData.sum.numeric.$response">Price must be a number*</span>
          <span v-else-if="!v$.formData.sum.required.$response">Price is required*</span>
          <span v-else-if="!v$.formData.sum.minValue.$response">Price must be at least 1*</span>
          <span v-else-if="!v$.formData.sum.maxValue.$response">Price must not exceed 1,000,000*</span>
          </span>
          <span class="error-message" v-if="state.errors.Sum">{{ state.errors.Sum[0] }}</span>
        </div>

        <div class="input-form">
          <label>Description: </label>
          <input
              v-model="state.formData.description"
              class="input"
              type="text"
              placeholder="Enter room price"
              :readonly="!state.isEditing"
              @input="v$.formData.description.$touch()"
              data-testid="input-description"
          >
          <span class="error-message" v-if="v$.formData.description.$error">Description can have only 50 symbols*</span>
          <span class="error-message" v-if="state.errors.Description">{{ state.errors.Description[0] }}</span>
        </div>

        <div class="registration-class">
          <router-link class="registration-btn" to="/services" data-testid="button-back">Back</router-link>
          <button type="button" class="registration-btn" @click="toggleEdit" data-testid="button-edit-save">{{ state.isEditing ? 'Save' : 'Edit' }}</button>
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
  name: "ServiceDetails",
  props: {
    idService: {
      type: Number,
      required: true
    }
  },
  setup(){
    const store = useStore();
    const router = useRouter();
    const state = reactive( {
      isEditing: false,
      formData: {
        idService: '',
        title: '',
        sum: 0,
        description: ''
      },
      errors: {}
    })

    const rules = {
      formData: {
        title: { required, maxLength: maxLength(20) },
        sum: { required, numeric, minValue: minValue(1), maxValue: maxValue(1000000) },
        description: { maxLength: maxLength(50) }
      }
    };

    const v$ = useVuelidate(rules, state);


    async function toggleEdit() {
      if (state.isEditing) {
        v$.value.$touch();
        if (!v$.value.$error) {
          try {
            const response = await axios.put('https://localhost:44384/api/Service', state.formData, {
              headers: {
                'Authorization': `Bearer ${store.getters.getToken}`
              },
            });

            if (response.data.httpStatusCode && response.data.httpStatusCode !== 200) {
              state.errors = response.data.errors || {};
              console.log('Error', response.data.message);
            } else {
              console.log('Success:', response.data);
              notify({
                title: 'Service Updated',
                text: 'The service has been successfully updated.',
                type: 'success',
                duration: 4000
              });
              state.isEditing = false;
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

    async function fetchSpecificService(idService) {
      try {
        const response = await axios.get('https://localhost:44384/api/Service/' + idService,{
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
        });
        state.formData = response.data;
      } catch (error) {
        console.error(error);
      }
    }

    return { state, v$, toggleEdit, fetchSpecificService}
  },
  mounted() {
    this.fetchSpecificService(this.idService);
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