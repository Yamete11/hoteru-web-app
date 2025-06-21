<template>
  <div class="room-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <select v-model="searchField" class="search-select" data-testid="room-search-select">
            <option value="number">Number</option>
            <option value="capacity">Capacity</option>
            <option value="type">Type</option>
            <option value="status">Status</option>
          </select>
          <input
              type="text"
              class="search-input"
              v-model="searchQuery"
              :placeholder="`Search by ${searchField}...`"
              data-testid="search-input"
          />
          <router-link to="/new-room" class="new-room-button" data-testid="new-room-button">New Room</router-link>
        </div>
        <div class="main-bot">
          <div class="table-headers">
            <span class="header number">Number</span>
            <span class="header capacity">Capacity</span>
            <span class="header type">Type</span>
            <span class="header status">Status</span>
            <span class="header action">Action</span>
          </div>
          <div v-if="!isLoading">
            <room-list :rooms="rooms" @deleteRoom="deleteRoom" />
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
import axios from 'axios';

export default {
  name: "Room",
  data() {
    return {
      isLoading: false,
      rooms: [],
      searchQuery: '',
      searchField: 'number',
      page: 1,
      limit: 15,
      roomTypes: [],
      roomStatuses: [],
      totalRooms: 0,
    };
  },
  mounted() {
    this.fetchRooms();
  },
  watch: {
    searchQuery: 'fetchRooms',
    searchField: 'fetchRooms'
  },
  methods: {
    deleteRoom(idRoom) {
      this.rooms = this.rooms.filter(room => room.idRoom !== idRoom);
    },
    async fetchRooms() {
      try {
        this.isLoading = true;
        const response = await axios.get('https://localhost:44384/api/Room', {
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
        this.rooms = response.data.list;
        this.totalRooms = Math.ceil(response.data.totalCount / this.limit);
      } catch (error) {
        console.error(error);
      } finally {
        this.isLoading = false;
      }
    }
    ,
    async loadMore() {
      try {
        this.page++;
        const response = await axios.get('https://localhost:44384/api/Room', {
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
        this.totalRooms = Math.ceil(response.data.totalCount / this.limit);
        this.rooms = [...this.rooms, ...response.data.list];
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
  height: 100vh;
}

.new-room-button {
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

.header.number,
.header.capacity,
.header.type,
.header.status,
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
