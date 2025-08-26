<template>
  <div class="guest-component">
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
            <div :key="`${searchField}:${searchQuery}`" v-intersection="loadMore" class="observer"></div>
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
import { guests } from "@/api";
import debounce from "lodash.debounce";

export default {
  name: "Guest",
  data() {
    return {
      isLoading: false,
      isLoadingMore: false,
      guests: [],
      searchQuery: "",
      searchField: "name",
      page: 1,
      limit: 15,
      totalGuests: 0,
      isGuestDeletedNotificationVisible: false,

      _debouncedFetch: null,
      _requestId: 0,
    };
  },

  created() {
    this._debouncedFetch = debounce(() => this.fetchGuests(), 350);
  },

  mounted() {
    this.fetchGuests();

    if (this.$route.query.created === "true") {
      notify({
        title: "Guest Created",
        text: "The new guest has been successfully created.",
        type: "success",
        duration: 3000,
      });
      this.$router.replace({ query: {} });
    }
  },

  beforeUnmount() {
    this._debouncedFetch?.cancel?.();
  },

  watch: {
    searchQuery() {
      this.page = 1;
      this._debouncedFetch();
    },
    searchField() {
      this.page = 1;
      this._debouncedFetch();
    },
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
      setTimeout(() => { this.isGuestDeletedNotificationVisible = false; }, 3000);
    },

    deleteGuest(idPerson) {
      this.guests = this.guests.filter(g => g.idPerson !== idPerson);
    },

    async fetchGuests() {
      const rid = ++this._requestId;
      try {
        this.isLoading = true;
        this.page = 1;

        const data = await guests.list({
          page: this.page,
          limit: this.limit,
          searchQuery: this.searchQuery?.trim(),
          searchField: this.searchField,
        });

        if (rid !== this._requestId) return;

        this.guests = data?.list ?? [];
        const totalCount = data?.totalCount ?? 0;
        this.totalGuests = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (e) {
        console.error("guests.fetchGuests error", e);
        this.guests = [];
        this.totalGuests = 1;
      } finally {
        if (rid === this._requestId) this.isLoading = false;
      }
    },

    async loadMore() {
      if (this.isLoading || this.isLoadingMore) return;
      if (this.page >= this.totalGuests) return;

      try {
        this.isLoadingMore = true;
        const nextPage = this.page + 1;

        const data = await guests.list({
          page: nextPage,
          limit: this.limit,
          searchQuery: this.searchQuery?.trim(),
          searchField: this.searchField,
        });

        const nextChunk = data?.list ?? [];
        this.guests = [...(this.guests || []), ...nextChunk];
        this.page = nextPage;

        const totalCount = data?.totalCount;
        if (typeof totalCount === "number") {
          this.totalGuests = Math.max(1, Math.ceil(totalCount / this.limit));
        }
      } catch (e) {
        console.error("guests.loadMore error", e);
      } finally {
        this.isLoadingMore = false;
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
