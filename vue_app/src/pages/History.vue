<template>
  <div class="room-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <input type="text" class="search-input" placeholder="Search ..." />
        </div>
        <div class="main-bot">
          <div class="table-headers">
            <span class="header in">Date In</span>
            <span class="header out">Date Out</span>
            <span class="header name">Name</span>
            <span class="header room">Room</span>
            <span class="header bookedBy">Booked By</span>
            <span class="header action">Action</span>
          </div>
          <div>
            <history-list :reservations="reservations" @deleteReservation="deleteReservation">
              The history list is empty
            </history-list>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "History",
  data() {
    return {
      reservations: []
    };
  },
  mounted() {
    console.log(this.reservations)
    this.fetchReservations();
  },
  methods: {
    deleteReservation(idReservation) {
      this.reservations = this.reservations.filter(reservation => reservation.idReservation !== idReservation);
    },
    async fetchReservations() {
      try {
        const response = await axios.get('https://localhost:44384/api/Reservation/history');
        this.reservations = response.data;
      } catch (error) {
        console.error(error);
      }
    },
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
.header.in {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.header.out {
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
.header.action {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
</style>