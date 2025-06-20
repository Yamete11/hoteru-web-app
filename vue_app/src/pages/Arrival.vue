<template>
  <div class="room-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <select v-model="searchField" class="search-select" data-testid="arrival-search-select">
            <option value="name">Name</option>
            <option value="roomNumber">Room</option>
            <option value="bookedBy">Booked By</option>
          </select>
          <input
              type="text"
              class="search-input"
              v-model="searchQuery"
              :placeholder="`Search by ${searchField}...`"
              data-testid="arrival-search-input"
          />
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
          <div v-if="!isLoading">
            <arrival-list :reservations="filteredReservations" @deleteReservation="deleteReservation"/>
            <div v-intersection="loadMore" class="observer"></div>
          </div>
          <div v-else>
            <div>The list is loading...</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "Arrival",
  data() {
    return {
      isLoading: false,
      reservations: [],
      searchQuery: '',
      searchField: 'name',
      page: 1,
      limit: 15,
      totalReservations: 0
    };
  },
  computed: {
    filteredReservations() {
      return this.reservations.filter(res => {
        const rawValue = res[this.searchField];
        const fieldValue = String(rawValue ?? '').toLowerCase();
        return fieldValue.startsWith(this.searchQuery.toLowerCase());
      });
    }
  },
  mounted() {
    this.fetchReservations();
  },
  methods: {
    deleteReservation(idReservation) {
      this.reservations = this.reservations.filter(reservation => reservation.idReservation !== idReservation);
    },
    async fetchReservations() {
      try {
        this.isLoading = true;
        const response = await axios.get('https://localhost:44384/api/Reservation/arrivals', {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
          params: {
            page: this.page,
            limit: this.limit
          }
        });
        this.reservations = response.data.list;
        this.totalReservations = Math.ceil(response.data.totalCount / this.limit);
      } catch (error) {
        console.error(error);
      } finally {
        this.isLoading = false;
      }
    },
    async loadMore() {
      try {
        this.page++;
        const response = await axios.get('https://localhost:44384/api/Reservation/arrivals', {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
          params: {
            page: this.page,
            limit: this.limit
          }
        });
        this.totalReservations = Math.ceil(response.data.totalCount / this.limit);
        this.reservations = [...this.reservations, ...response.data.list];
      } catch (error) {
        console.error(error);
      }
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
  justify-content: flex-start;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
}

.search-select {
  padding: 0.6rem 1rem;
  font-size: 1rem;
  border: none;
  border-radius: 4px;
  background-color: #FFFFFF;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.search-input {
  width: 200px;
  padding: 0.6rem 1rem;
  font-size: 1rem;
  border: none;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  background-color: #FFFFFF;
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
.header.in,
.header.out,
.header.name,
.header.room,
.header.bookedBy,
.header.action {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}

.observer {
  height: 10px;
  margin-bottom: 20px;
}
</style>
