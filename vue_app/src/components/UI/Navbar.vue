<template>
  <div class="navbar">
    <div class="navbar-title">
      <h1 @click="goToArrivals">HOTERU ホテル</h1>
      <h2>{{ companyName }}</h2>
    </div>
    <div class="navbar-links">
      <router-link class="navbar-btn" to="/new-reservation">New Reservation</router-link>
      <div class="right-links">
        <div class="dropdown-wrapper" @mouseenter="showDropdown = true" @mouseleave="showDropdown = false">
          <button class="navbar-btn">Settings</button>
          <div class="dropdown-content" v-show="showDropdown">
            <router-link to="/my-account">My Account</router-link>
            <router-link to="/employees">List of Employees</router-link>
          </div>
        </div>
        <button class="navbar-btn" @click="logout">Log out</button>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  name: "Navbar",
  data() {
    return {
      showDropdown: false
    };
  },
  methods: {
    logout() {
      localStorage.removeItem('token');
      localStorage.removeItem('userData');
      this.$store.commit('clearToken');
      this.$store.commit('clearUserData');
      this.$router.push('/');
    }
    ,
    goToArrivals() {
      this.$router.push('/arrivals');
    }
  },
  computed: {
    companyName() {
      const user = this.$store.getters.getUserData;
      return user?.companyTitle || 'No Company';
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
  z-index: 10;
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
  width: 25%;
}

.navbar-links{
  display: flex;
  justify-content: space-between;
  width: 40%;
}

.right-links{
  display: flex;
  justify-content: space-around;
  width: 30%;
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
.navbar h1 {
  cursor: pointer;
}

.dropdown-wrapper {
  position: relative;
  display: inline-block;
}

.dropdown-content {
  position: absolute;
  top: 100%;
  left: 0;
  background-color: #715d47;
  border: 1px solid white;
  border-radius: 5px;
  min-width: 160px;
  z-index: 100;
  display: flex;
  flex-direction: column;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
}

.dropdown-content a {
  color: white;
  padding: 10px;
  text-decoration: none;
  font-weight: bold;
  font-size: 16px;
  border-bottom: 1px solid white;
}

.dropdown-content a:last-child {
  border-bottom: none;
}

.dropdown-content a:hover {
  background-color: #8d745b;
}



</style>
