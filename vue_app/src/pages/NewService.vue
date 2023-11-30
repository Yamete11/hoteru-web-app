<template>
  <div class="newService-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Service</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Title: </label>
          <input
              v-model="formData.Title"
              class="input"
              type="text"
              placeholder="Enter service title"
          >
          <span class="error-message" v-if="errors.Title">{{ errors.Title[0] }}</span>
        </div>
        <div class="input-form">
          <label>Price: </label>
          <input
              v-model="formData.Sum"
              class="input"
              type="text"
              placeholder="Enter service price"
          >
          <span class="error-message" v-if="errors.Sum">{{ errors.Sum[0] }}</span>
        </div>
        <div class="input-form">
          <label>Description: </label>
          <input
              v-model="formData.Description"
              class="input"
              type="text"
              placeholder="Enter service description"
          >
          <span class="error-message" v-if="errors.Description">{{ errors.Description[0] }}</span>
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/services">Cancel</router-link>
          <button class="registration-btn" @click="addService">Confirm</button>
        </div>
      </form>
    </div>

  </div>
</template>

<script>
import axios from 'axios';
export default {
  name: "NewService",
  data(){
    return {
      formData: {
        Title: '',
        Sum: 0,
        Description: '',
      },
      errors: {
        Title: '',
        Sum: '',
        Description: '',
      }
    }
  },
  methods:{
    async addService() {
      try {
        const response = await axios.post('https://localhost:44384/api/Service', this.formData);
        console.log('Response:', response.data);
        this.errors = {
          Title: '',
          Price: '',
          Description: '',
        }

        if (response.data && response.data.httpStatusCode === 200) {
          this.$router.push({ name: "Services" });
        }
      } catch (error) {
        if (error.response && error.response.data && error.response.data.errors) {
          this.errors = error.response.data.errors;
        } else {
          console.log('Error', error);
        }
      }
    }
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