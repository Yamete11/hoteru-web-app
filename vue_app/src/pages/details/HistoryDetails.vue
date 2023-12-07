<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>History Details</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Name:  </label>
          <label>{{ reservation.name }} {{ reservation.surname}}</label>
        </div>
        <div class="input-form">
          <label>Room number: </label>
          <label>{{ reservation.roomNumber }}</label>
        </div>
        <div class="input-form">
          <label>Room type: </label>
          <label>{{ reservation.roomType }}</label>
        </div>

        <div class="input-form">
          <label>In: </label>
          <label>{{ reservation.in }}</label>
        </div>
        <div class="input-form">
          <label>Out: </label>
          <label>{{ reservation.out }}</label>
        </div>
        <div class="input-form">
          <label>Booked by: </label>
          <label>{{ reservation.bookedBy }}</label>
        </div>
        <div class="input-form">
          <label>Deposit: </label>
          <label>{{ reservation.depositSum }} {{ reservation.depositType}}</label>
        </div>
        <div class="input-form">
          <label>Bill: </label>
          <label>{{ reservation.billSum }}</label>
        </div>
        <div class="input-form">
          <label>Bill created at: </label>
          <label>{{ reservation.created }}</label>
        </div>

        <div>
          <label class="service-label">Services: </label>
          <div class="service-list">
            <ul>
              <li class="element" v-for="service in reservation.services" :key="service.idService">
                Title: {{ service.title }},  Price: {{ service.sum }},  Date: {{ service.date }}
              </li>
            </ul>
          </div>
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/history">Back</router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "HistoryDetails",
  data() {
    return {
      reservation: {}
    }
  },
  props: {
    idReservation: {
      type: Number,
      required: true
    }
  },
  methods:{
    async fetchSpecificHistory(idReservation){
      try{
        const response = await axios.get('https://localhost:44384/api/Reservation/history/' + idReservation);

        this.reservation = response.data[0];
        console.log(this.reservation)
      } catch(error){
        console.log(error);
      }
    }
  },
  mounted() {
    this.fetchSpecificHistory(this.idReservation);
  }

}
</script>

<style scoped>
.newRoom-component {
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
  margin: 0 auto;
  width: 80%;
}

.creating-form {
  display: flex;
  align-items: center;
  flex-direction: column;
  width: 100%;
  max-width: 600px;
}

.input-form {
  display: flex;
  margin: 5px;
}

.input-form label {
  margin: 5px;
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
  justify-content: center;
  align-items: center;
  width: 100%;
}

.element{
  display: flex;
  justify-content: center;
  width: 100%;
  background-color:#C8B6A6;
  border-radius: 5px;
  font-weight: bold;
  font-size: 15px;
  padding-top: 10px;
  padding-bottom: 10px;
  margin: 10px 0;
}

.service-list ul {
  padding-left: 0;
  list-style-type: none;
  width: 100%;
}

.service-list {
  max-height: 200px;
  overflow-y: auto;
  width: 100%;
  border: 1px solid black;
  border-radius: 5px;
  padding: 10px;
  margin-bottom: 10px;
}

.service-label {
  font-weight: bold;
  color: black;
  align-self: flex-start;
  margin-bottom: 5px;
}
</style>