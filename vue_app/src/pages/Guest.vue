<template>
  <div class="guest-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <input type="text" class="search-input" v-model="searchQuery" placeholder="Search by name ..." />
          <router-link to="/new-guest" class="new-guest-button">New Guest</router-link>
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
            <guest-list :guests="sortedAndSearchedPosts" @deleteGuest="deleteGuest"/>
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
  name: "Guest",
  data() {
    return {
      totalGuests: 0,
      isLoading: false,
      guests: [],
      searchQuery: '',
      page: 1,
      limit: 15,
    };
  },
  computed: {
    sortedAndSearchedPosts() {
      return this.guests.filter(guest => guest.name.toLowerCase().startsWith(this.searchQuery.toLowerCase()));
    }
  },
  mounted() {
    this.fetchGuests();
  },
  methods: {
    deleteGuest(idPerson) {
      this.guests = this.guests.filter(guest => guest.idPerson !== idPerson);
    },
    async fetchGuests() {
      try {
        const token = this.$store.getters.getToken;
        console.log("Token " + token)
        this.isLoading = true;
        const response = await axios.get('https://localhost:44384/api/Guest', {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
          params: {
            page: this.page,
            limit: this.limit
          }
        });
        this.guests = response.data.list;
        this.totalGuests = Math.ceil(response.data.totalCount / this.limit);
        console.log(this.guests)
      } catch (error) {
        console.error(error);
      } finally {
        this.isLoading = false;
      }
    },
    async loadMore() {
      try {
        this.page++;
        console.log(this.page)
        const response = await axios.get('https://localhost:44384/api/Guest', {
          headers: {
            'Authorization': `Bearer ${this.$store.getters.getToken}`
          },
          params: {
            page: this.page,
            limit: this.limit
          }
        });
        console.log(response)
        this.totalGuests = Math.ceil(response.data.totalCount / this.limit);
        this.guests = [...this.guests, ...response.data.list];
      } catch (error) {
        console.error(error);
      }
    },
  },
}
</script>

<style scoped>
.guest-component {
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
.header.name {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.header.surname {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.telNumber {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.email {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
.header.action {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }

.observer{
  height: 10px;
  margin-bottom: 20px;
}
</style>