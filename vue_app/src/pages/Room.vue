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
import { notify } from "@kyvg/vue3-notification";
import { rooms } from '@/api';

export default {
  name: "Room",
  data() {
    return {
      isLoading: false,
      isLoadingMore: false,
      rooms: [],
      searchQuery: '',
      searchField: 'number',
      page: 1,
      limit: 15,
      totalRooms: 0,
      isOccupiedWarningVisible: false,
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
    searchField: 'fetchRooms',
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
      setTimeout(() => { this.isOccupiedWarningVisible = false; }, 3000);
    },

    deleteRoom(idRoom) {
      this.rooms = this.rooms.filter(r => r.idRoom !== idRoom);
      notify({
        title: 'Room Deleted',
        text: 'Room has been deleted.',
        type: 'success',
        duration: 3000
      });
    },

    async fetchRooms() {
      try {
        this.isLoading = true;
        this.page = 1;

        const data = await rooms.list({
          page: this.page,
          limit: this.limit,
          searchQuery: this.searchQuery,
          searchField: this.searchField,
        });
        console.log(data);
        this.rooms = data?.list ?? [];
        console.log(this.rooms);

        const totalCount = data?.totalCount ?? 0;
        this.totalRooms = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (e) {
        console.error('rooms.fetchRooms error', e);
        this.rooms = [];
        this.totalRooms = 1;
      } finally {
        this.isLoading = false;
      }
    },

    async loadMore() {
      if (this.isLoading || this.isLoadingMore) return;
      if (this.page >= this.totalRooms) return;

      try {
        this.isLoadingMore = true;
        const next = this.page + 1;

        const data = await rooms.list({
          page: next,
          limit: this.limit,
          searchQuery: this.searchQuery,
          searchField: this.searchField,
        });

        const nextChunk = data?.list ?? [];
        this.rooms = [...this.rooms, ...nextChunk];
        this.page = next;

        const totalCount = data?.totalCount ?? 0;
        this.totalRooms = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (e) {
        console.error('rooms.loadMore error', e);
      } finally {
        this.isLoadingMore = false;
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
