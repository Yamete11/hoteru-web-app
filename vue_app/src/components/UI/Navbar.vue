<template>
  <div class="navbar">
    <div class="navbar-title">
      <h1>HOTERU ホテル</h1>
      <h2>"Company name"</h2>
    </div>
    <div class="navbar-links">
      <router-link class="navbar-btn" to="/new-reservation">New Reservation</router-link>
      <div class="right-links">
        <router-link class="navbar-btn" to="/settings">Settings</router-link>
        <button class="navbar-btn" @click="logout">Log out</button>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  name: "Navbar",
  methods: {
    logout() {
      localStorage.removeItem('token');
      this.$router.push('/');
    }
  }
}
</script>

<style scoped>
.navbar{
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  background-color: #715d47;
  color: white;
  height: 8vh;
  box-sizing: border-box;
}

.navbar-title{
  display: flex;
  justify-content: space-around;
  align-items: center;
  width: 25%
}

.navbar-links{
  display: flex;
  justify-content: space-between;
  width: 40%;
}

.right-links{
  display: flex;
  justify-content: space-around;
  width: 20%;
}

.navbar-btn{
  text-decoration: none;
  font-weight: bold;
  font-size: 20px;
  color: white;
  border: solid 2px white;
  border-radius: 5px;
  padding: 10px;
  background-color: #715d47;
  cursor: pointer;
}

.navbar h1, .navbar h2 {
  margin: 0;
}
</style>