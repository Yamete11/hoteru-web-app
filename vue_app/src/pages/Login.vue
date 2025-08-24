<template>
  <div class="main-login" data-testid="main-login">
    <div class="registration">
      <router-link class="registration-btn" to="/registration" data-testid="new-company-link">New Company</router-link>
    </div>
    <div class="login">
      <h1 data-testid="login-title">HOTERU ホテル</h1>
      <form @submit.prevent="loginIn" class="login-form" data-testid="login-form">
        <my-input
            class="login-input"
            v-model="login"
            type="text"
            placeholder="login"
            data-testid="login-input"
        />
        <my-input
            class="login-input"
            v-model="password"
            type="password"
            placeholder="password"
            data-testid="password-input"
        />
        <p class="error-message" v-if="errors.validation" data-testid="error-message">
          {{ errors.validation }}
        </p>
        <button class="login-btn" type="submit" data-testid="login-button">LOG IN</button>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  name: "Login",
  data() {
    return {
      login: "",
      password: "",
      errors: {}
    };
  },
  methods: {
    async loginIn() {
      this.errors = {};
      try {
        await this.$store.dispatch("login", {
          login: this.login,
          password: this.password
        });
        this.$router.push("/arrivals");
      } catch (err) {
        this.errors.validation =
            err?.status === 401
                ? "Validation failed, login or password is wrong"
                : err?.message || "Unexpected error. Please try again.";
      }
    }
  }
};
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
  cursor: pointer;
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

.error-message {
  color: red;
  margin: 10px 0;
}
</style>
