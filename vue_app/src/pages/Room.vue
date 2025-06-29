<template>
  <div class="room-component">
    <notifications position="top right" />
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
            <room-list :rooms="rooms" @deleteRoom="deleteRoom" @occupiedDeleteAttempt="showOccupiedWarning" />
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
import { notify } from "@kyvg/vue3-notification";


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
      isOccupiedWarningVisible: false
    };
  },
  mounted() {
    this.fetchRooms();

    if (this.$route.query.created === 'true') {
      notify({
        title: 'Room Created',
        text: 'Room has been successfully created.',
        type: 'success',
        duration: 3000
      });

      this.$router.replace({ query: {} });
    }
  },
  watch: {
    searchQuery: 'fetchRooms',
    searchField: 'fetchRooms'
  },
  methods: {
    showOccupiedWarning() {
      if (this.isOccupiedWarningVisible) return;

      this.isOccupiedWarningVisible = true;

      notify({
        title: "Warning",
        text: 'Cannot delete a room with status "Occupied".',
        type: 'warn',
        duration: 3000
      });

      setTimeout(() => {
        this.isOccupiedWarningVisible = false;
      }, 3000);
    }
    ,
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

.warning-banner {
  background-color: #ffcccc;
  color: #990000;
  padding: 1rem;
  text-align: center;
  margin: 1rem 0;
  font-weight: bold;
  border: 1px solid #cc0000;
  border-radius: 5px;
}

</style>
