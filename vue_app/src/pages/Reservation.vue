<template>
  <div class="room-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <input type="text" class="search-input" placeholder="Search ..." />
          <router-link to="/new-reservation" class="new-reservation-button">New Reservation</router-link>
        </div>
        <div class="main-bot">
          <div class="table-headers">
            <span class="header date">Date</span>
            <span class="header time">Time</span>
            <span class="header name">Name</span>
            <span class="header room">Room</span>
            <span class="header bookedBy">Booked By</span>
            <span class="header note">Note</span>
            <span class="header action">Action</span>
          </div>
          <div>
            <reservation-list :reservations="reservations" @deleteReservation="deleteReservation">
              The list of reservations is empty
            </reservation-list>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "Reservation",
  components: {},
  data() {
    return {
      reservations: []
    };
  },
  mounted() {
    console.log(this.$refs.observer);
    this.fetchReservations();
  }
  ,
  methods: {
    async fetchReservations() {
      try {
        const response = await axios.get('https://localhost:44384/api/Reservation');
        this.reservations = response.data;
      } catch (error) {
        console.error(error);
      }
    },
    deleteReservation(idReservation) {
      this.reservations = this.reservations.filter(reservation => reservation.idReservation !== idReservation);
    }
  }

}
</script>

<style scoped>
.room-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
}

.content {
  display: flex;
  flex-grow: 1;
}

.main {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
  padding-top: 8vh;
  padding-left: 8%;
}

.main-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
}

.search-input {
  width: 10%;
  padding: 0.6rem 1rem;
  font-size: 1rem;
  border: none;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  background-color: #FFFFFF;
  margin-right: 1rem;
}

.search-input:focus {
  outline: none;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}


.new-reservation-button {
  font-weight: bold;
  font-size: 20px;
  padding: 0.5rem 3rem;
  background-color: #A4907C;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  white-space: nowrap;
}

.table-headers {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0.5rem 0.5rem 3rem;
  background-color: #A4907C;
  margin: 1rem;
  border-radius: 4px;
}

.header {
  font-weight: bold;
  font-size: 20px;
}
.header.date {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.header.time {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.name {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.room {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.bookedBy {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.note {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.action {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
</style>