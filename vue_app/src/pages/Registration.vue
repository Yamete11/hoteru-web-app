<template>
  <div class="registration">
    <h1>HOTERU ホテル</h1>
    <form @submit.prevent class="registration-form">
      <registration-input v-model="formData.firstName" placeholder="Enter your first name">First name: </registration-input>
      <registration-input v-model="formData.lastName" placeholder="Enter your last name">Last name: </registration-input>
      <registration-input v-model="formData.email" placeholder="Enter your email">Email: </registration-input>
      <registration-input v-model="formData.login" placeholder="Enter your login">Login: </registration-input>
      <registration-input v-model="formData.password" placeholder="Enter your password">Password: </registration-input>
      <registration-input v-model="formData.companyName" placeholder="Enter your company name">Company name: </registration-input>
      <registration-input v-model="formData.country" placeholder="Enter your country">Country: </registration-input>
      <registration-input v-model="formData.city" placeholder="Enter tour city">City: </registration-input>
      <registration-input v-model="formData.street" placeholder="Enter your address">Address: </registration-input>
      <registration-input v-model="formData.postCode" placeholder="Enter your postal code">Postal code: </registration-input>

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
        firstName: '',
        lastName: '',
        email: '',
        login: '',
        password: '',
        companyName: '',
        country: '',
        city: '',
        street: '',
        postCode: ''
      }
    }
  },
  methods:{
    async create(){
      try {
        const response = await axios.post('https://localhost:44384/api/Hotel', this.formData);
        console.log('Success:', response.data);
      } catch (error) {
        console.error('Error:', error);
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
</style>