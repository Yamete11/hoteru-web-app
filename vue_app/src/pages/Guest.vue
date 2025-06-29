<template>
  <div class="guest-component">
    <notifications position="top right" />
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <select v-model="searchField" class="search-select" data-testid="guest-search-select">
            <option value="name">Name</option>
            <option value="surname">Surname</option>
            <option value="telNumber">Phone</option>
            <option value="email">Email</option>
          </select>
          <input
              type="text"
              class="search-input"
              v-model="searchQuery"
              :placeholder="`Search by ${searchField}...`"
              data-testid="guest-search-input"
          />
          <router-link to="/new-guest" class="new-guest-button" data-testid="new-guest-button">New Guest</router-link>
        </div>
        <div class="main-bot">
          <div class="table-headers">
            <span class="header name">Name</span>
            <span class="header surname">Surname</span>
            <span class="header telNumber">Tel.number</span>
            <span class="header email">Email</span>
            <span class="header action">Action</span>
          </div>
          <div v-if="!isLoading">
            <guest-list :guests="guests" @deleteGuest="deleteGuest" @notificationDeleteAttempt="showGuestDeletedNotification"/>
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
import { notify } from "@kyvg/vue3-notification";


export default {
  name: "Guest",
  data() {
    return {
      totalGuests: 0,
      isLoading: false,
      guests: [],
      searchQuery: '',
      searchField: 'name',
      page: 1,
      limit: 15,
      isGuestDeletedNotificationVisible: false
    };
  },
  mounted() {
    this.fetchGuests();

    if (this.$route.query.created === 'true') {
      notify({
        title: "Guest Created",
        text: "The new guest has been successfully created.",
        type: "success",
        duration: 3000,
      });

      this.$router.replace({ query: {} });
    }
  },
  watch: {
    searchQuery: 'fetchGuests',
    searchField: 'fetchGuests',
  },
  methods: {
    showGuestDeletedNotification() {
      if (this.isGuestDeletedNotificationVisible) return;

      this.isGuestDeletedNotificationVisible = true;

      notify({
        title: "Guest Deleted",
        text: "Guest has been deleted. All associated reservations were also removed.",
        type: "success",
        duration: 3000,
      });

      setTimeout(() => {
        this.isGuestDeletedNotificationVisible = false;
      }, 3000);
    },
    deleteGuest(idPerson) {
      this.guests = this.guests.filter(guest => guest.idPerson !== idPerson);
    },
    async fetchGuests() {
      try {
        this.isLoading = true;
        this.page = 1;
        const response = await axios.get('https://localhost:44384/api/Guest', {
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
        this.guests = response.data.list;
        this.totalGuests = Math.ceil(response.data.totalCount / this.limit);
      } catch (error) {
        console.error(error);
      } finally {
        this.isLoading = false;
      }
    },
    async loadMore() {
      if (this.page >= this.totalGuests) return;
      try {
        this.page++;
        const response = await axios.get('https://localhost:44384/api/Guest', {
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
        this.guests = [...this.guests, ...response.data.list];
      } catch (error) {
        console.error(error);
      }
    },
  },
};
</script>


<style scoped>
.guest-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
}

.new-guest-button {
  font-weight: bold;
  font-size: 20px;
  padding: 0.5rem 3rem;
  background-color: #A4907C;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  white-space: nowrap;
}
.header.name,
.header.surname,
.header.telNumber,
.header.email,
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
