<template>
  <div class="room-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">

        <div class="main-top">
          <div class="left">
            <input type="text" class="search-input" v-model="searchQuery" placeholder="Search room by number ..."/>
            <button @click="loadMoreRooms">Load</button>
          </div>
          <div class="right">
            <router-link to="/new-room" class="new-room-button">New Room</router-link>
          </div>
        </div>
        <div class="main-bot">
          <div class="table-headers">
            <span class="header number">Number</span>
            <span class="header capacity">Capacity</span>
            <span class="header type">Type</span>
            <span class="header status">Status</span>
            <span class="header action">Action</span>
          </div>
          <div>
            <room-list :rooms="sortedAndSearchedPosts" @deleteRoom="deleteRoom"/>
            <div v-intersection=""  class="observer"></div>
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
  components: {},
  data() {
    return {
      rooms: [],
      searchQuery: '',
      page: 1,
      limit: 15,
      roomTypes: [],
      roomStatuses: [],
    };
  },
  mounted() {
    this.fetchRooms();
  },
  computed: {
    sortedAndSearchedPosts() {
      return this.rooms.filter(room => room.number.toLowerCase().startsWith(this.searchQuery.toLowerCase()));
    }
  },
  methods: {
    deleteRoom(idRoom) {
      this.rooms = this.rooms.filter(room => room.idRoom !== idRoom);
    },
    async fetchRooms() {
      try {
        const response = await axios.get('https://localhost:44384/api/Room', {
          params: {
            page: this.page,
            limit: this.limit
          }
        });
        this.rooms = response.data;
        console.log(this.rooms)
      } catch (error) {
        console.error(error);
      }
    },
    async loadMoreRooms() {
      try {
        this.page += 1;
        console.log(this.page)
        const response = await axios.get('https://localhost:44384/api/Room', {
          params: {
            page: this.page,
            limit: this.limit
          }
        });
        this.rooms = [...this.rooms, ...response.data];
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
  height: 100vh;
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

.left {
  width: 100%;
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

.filters-button{
  text-decoration: none;
  border: none;
  font-weight: bold;
  font-size: 20px;
  padding: 0.5rem 3rem;
  background-color: #A4907C;
  color: white;
  border-radius: 4px;
  cursor: pointer;
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

.header.number {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}

.header.capacity {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}

.header.type {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}

.header.status {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}

.header.action {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}

.observer-anchor {
  height: 20px;
}

</style>