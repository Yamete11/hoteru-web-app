<template>
  <div class="main-login">
    <div class="registration">
      <router-link class="registration-btn" @click="login" to="/registration">New Company</router-link>
    </div>
    <div class="login">
      <h1>HOTERU ホテル</h1>
      <form @submit.prevent class="login-form">
        <my-input
            class="login-input"
            v-model="login"
            type="text"
            placeholder="login"
        />
        <my-input
            class="login-input"
            v-model="password"
            type="password"
            placeholder="password"
        />
        <router-link class="login-btn" @click="login" to="/arrivals">LOGIN</router-link>

      </form>
    </div>
  </div>
</template>

<script>
export default {
  name: "Login",
  data() {
    return {
      login: '',
      password: ''
    }
  },
  methods: {
    login() {
      // handle login
    }
  }
}
</script>

<style scoped>
.main-login{
  background-color: #252525;
  height: 100vh;
}
.login {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 70vh;
}

.login-form {
  display: flex;
  flex-direction: column;
  width: 100%;
  max-width: 300px;
}

.login-input, .login-btn {
  box-sizing: border-box;
  width: 100%;
  padding: 10px;
  margin-top: 10px;
  border: 1px solid #ccc;
}

.login-input{
  background-color: #D9D9D9;
}

.login-btn {
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  display: flex;
  justify-content: center;
  border-radius: 5px;
  font-weight: bold;
  color: white;
}

.registration{
  display: flex;
  justify-content: flex-end;
  width: 100%;
  height: 4vh;
}

.registration-btn{
  text-decoration: none;
  background-color: #8D7B68;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-weight: bold;
  color: white;
  margin: 10px 10px 0 10px;
  width: 10vw;
  display: flex;
  justify-content: center;
}
</style>

