<template>
  <div class="room-component">
    <notifications position="top right" />
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <select v-model="searchField" class="search-select" data-testid="reservation-search-select">
            <option value="name">Name</option>
            <option value="room">Room</option>
            <option value="bookedBy">Booked By</option>
            <option value="date">Date Range</option>
          </select>
          <div v-if="searchField !== 'date'">
            <input
                type="text"
                class="search-input"
                v-model="searchQuery"
                :placeholder="`Search by ${searchField}...`"
                data-testid="reservation-search-input"
            />
          </div>
          <div v-else class="date-range-container">
            <input
                type="date"
                class="search-input"
                v-model="dateFrom"
                data-testid="reservation-date-from"
            />
            <input
                type="date"
                class="search-input"
                v-model="dateTo"
                data-testid="reservation-date-to"
            />
          </div>
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
            <history-list :reservations="filteredReservations" @deleteReservation="deleteReservation"/>
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
import {notify} from "@kyvg/vue3-notification";

export default {
  name: "History",
  data() {
    return {
      isLoading: false,
      reservations: [],
      page: 1,
      limit: 15,
      totalReservations: 0,
      searchQuery: '',
      searchField: 'name',
      dateFrom: '',
      dateTo: ''
    };
  },
  computed: {
    filteredReservations() {
      if (this.searchField === 'date') {
        return this.reservations.filter(res => {
          const dateIn = new Date(res.dateIn);
          const from = this.dateFrom ? new Date(this.dateFrom) : null;
          const to = this.dateTo ? new Date(this.dateTo) : null;
          return (!from || dateIn >= from) && (!to || dateIn <= to);
        });
      }
      return this.reservations.filter(res => {
        const rawValue = res[this.searchField];
        const fieldValue = String(rawValue ?? '').toLowerCase();
        return fieldValue.startsWith(this.searchQuery.toLowerCase());
      });
    }
  },
  watch: {
    searchQuery: 'fetchReservations',
    searchField: 'fetchReservations',
  },
  mounted() {
    this.fetchReservations();
  },
  methods: {
    deleteReservation(idReservation) {
      this.reservations = this.reservations.filter(reservation => reservation.idReservation !== idReservation);

      notify({
        title: 'History Deleted',
        text: `History has been deleted.`,
        type: 'success',
        duration: 3000
      });
    },
    async fetchReservations() {
      try {
        this.isLoading = true;
        const response = await axios.get('https://localhost:44384/api/Reservation/history', {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
          params: {
            page: 1,
            limit: this.limit,
            searchQuery: this.searchQuery,
            searchField: this.searchField
          }
        });
        this.page = 1;
        this.reservations = response.data.list;
        this.totalReservations = Math.ceil(response.data.totalCount / this.limit);
      } catch (error) {
        console.error(error);
      } finally {
        this.isLoading = false;
      }
    },
    async loadMore() {
      if (this.page >= this.totalReservations) return;
      try {
        this.page++;
        const response = await axios.get('https://localhost:44384/api/Reservation/history', {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
          params: {
            page: this.page,
            limit: this.limit,
            searchQuery: this.searchQuery,
            searchField: this.searchField
          }
        });
        this.totalReservations = Math.ceil(response.data.totalCount / this.limit);
        this.reservations = [...this.reservations, ...response.data.list];
      } catch (error) {
        console.error(error);
      }
    }
  }
};
</script>

<style scoped>
.room-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
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

.date-range-container {
  display: flex;
  gap: 1rem;
}
</style>
