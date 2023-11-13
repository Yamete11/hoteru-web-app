<template>
  <div class="registration">
    <h1>HOTERU ホテル</h1>
    <form @submit.prevent class="registration-form">
      <div class="input-form">
        <label>First name: </label>
        <input
            v-model="formData.Name"
            class="input"
            type="text"
            placeholder="Enter your First name"
        >
      </div>
      <div class="input-form">
        <label>Last name: </label>
        <input
            v-model="formData.Surname"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Email: </label>
        <input
            v-model="formData.Email"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Login: </label>
        <input
            v-model="formData.LoginName"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Password: </label>
        <input
            v-model="formData.Password"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Company name: </label>
        <input
            v-model="formData.Title"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Country: </label>
        <input
            v-model="formData.Country"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>City: </label>
        <input
            v-model="formData.City"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Address: </label>
        <input
            v-model="formData.Street"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>
      <div class="input-form">
        <label>Postcode: </label>
        <input
            v-model="formData.Postcode"
            class="input"
            type="text"
            placeholder="Enter your last name"
        >
      </div>

    </form>
    <div class="registration-class">
      <router-link class="registration-btn" to="/">Cancel</router-link>
      <router-link class="registration-btn" @click="create" to="/">Confirm</router-link>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: "Registration",
  components: {},

  data() {
    return {
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
      }
    }
  },
  methods:{
    async create(){
      console.log(this.formData)
      try {
        const response = await axios.post('https://localhost:44384/api/Hotel', this.formData);
        console.log('Success:', response.data);
      } catch (error) {
        console.log('Error:', error);
      }
    }
  }
}
</script>

<style scoped>
.registration {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 95vh;
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
</style>

