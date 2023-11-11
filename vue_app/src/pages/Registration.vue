<template>
  <div class="registration-form">
    <h1>HOTERU ホテル</h1>
    <form @submit.prevent class="registration-form">
      <registration-input>First name: </registration-input>
      <registration-input>Last name: </registration-input>
      <registration-input>Email: </registration-input>
      <registration-input>Login: </registration-input>
      <registration-input>Password: </registration-input>
      <registration-input>Company name: </registration-input>
      <registration-input>Country: </registration-input>
      <registration-input>City: </registration-input>
      <registration-input>Address: </registration-input>
      <registration-input>Postal code: </registration-input>

    </form>
    <div class="registration-btn">
      <router-link class="form-btn" @click="create" to="/">Cancel</router-link>
      <router-link class="form-btn" @click="create" to="/">Confirm</router-link>
    </div>
  </div>
</template>

<script>
export default {
  name: "Registration",
  components: {},

  data() {
    return {
      firstName: '',
      lastName: '',
      email: '',
      login: '',
      password: '',
      companyName: '',
      country: '',
      city: '',
      address: '',
      postCode: ''
    }
  },
  methods:{
    create(){

    }
  }
}
</script>

<style scoped>
.registration-form .registration-form{
  display: flex;
  text-align: center;
  flex-direction: column;
}

.form-btn{
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
}
</style>