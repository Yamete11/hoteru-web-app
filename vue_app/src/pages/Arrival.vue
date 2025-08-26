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
            <option value="date">Date Range</option>
          </select>
          <div v-if="searchField !== 'date'">
            <input
                type="text"
                class="search-input"
                v-model="searchQuery"
                :placeholder="`Search by ${searchField}...`"
                data-testid="arrival-search-input"
            />
          </div>
          <div v-else class="date-range-container">
            <input
                type="date"
                class="search-input"
                v-model="dateFrom"
                data-testid="arrival-date-from"
            />
            <input
                type="date"
                class="search-input"
                v-model="dateTo"
                data-testid="arrival-date-to"
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
            <arrival-list :reservations="filteredReservations" @deleteReservation="deleteReservation"/>
            <div :key="`${searchField}:${searchQuery}:${dateFrom}:${dateTo}`" v-intersection="loadMore" class="observer"></div>
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
import { reservations } from "@/api";
import debounce from "lodash.debounce";

export default {
  name: "Arrival",
  data() {
    return {
      isLoading: false,
      isLoadingMore: false,
      reservations: [],
      searchQuery: "",
      searchField: "name",
      dateFrom: "",
      dateTo: "",
      page: 1,
      limit: 15,
      totalReservations: 0,

      _debouncedFetch: null,
      _requestId: 0,
    };
  },

  created() {
    this._debouncedFetch = debounce(() => this.fetchReservations(), 350);
  },

  mounted() {
    this.fetchReservations();
  },

  beforeUnmount() {
    this._debouncedFetch?.cancel?.();
  },

  computed: {
    filteredReservations() {
      if (this.searchField === "date") {
        return this.reservations.filter(res => {
          const dateIn = new Date(res.in);
          const dateOut = new Date(res.out);
          const from = this.dateFrom ? new Date(this.dateFrom) : null;
          const to = this.dateTo ? new Date(this.dateTo) : null;
          return (!from || dateIn >= from) && (!to || dateOut <= to);
        });
      }
      const field = this.searchField === "roomNumber" ? "room" : this.searchField;
      const q = this.searchQuery.toLowerCase();
      return this.reservations.filter(res => String(res?.[field] ?? "").toLowerCase().startsWith(q));
    },
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
    deleteReservation(idReservation) {
      this.reservations = this.reservations.filter(r => r.idReservation !== idReservation);
      notify({ title: "Reservation Deleted", text: "Reservation has been deleted.", type: "success", duration: 3000 });
    },

    async fetchReservations() {
      const rid = ++this._requestId;
      try {
        this.isLoading = true;
        this.page = 1;

        const params = { page: this.page, limit: this.limit };
        if (this.searchField !== "date") {
          const field = this.searchField === "roomNumber" ? "room" : this.searchField;
          params.searchField = field;
          params.searchQuery = this.searchQuery?.trim();
        }

        const data = await reservations.arrivals(params);
        if (rid !== this._requestId) return;

        this.reservations = data?.list ?? [];
        const totalCount = data?.totalCount ?? 0;
        this.totalReservations = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (e) {
        console.error("arrivals.fetchReservations error", e);
        this.reservations = [];
        this.totalReservations = 1;
      } finally {
        if (rid === this._requestId) this.isLoading = false;
      }
    },

    async loadMore() {
      if (this.isLoading || this.isLoadingMore) return;
      if (this.page >= this.totalReservations) return;

      this.isLoadingMore = true;
      const nextPage = this.page + 1;
      try {
        const params = { page: nextPage, limit: this.limit };
        if (this.searchField !== "date") {
          const field = this.searchField === "roomNumber" ? "room" : this.searchField;
          params.searchField = field;
          params.searchQuery = this.searchQuery?.trim();
        }

        const data = await reservations.arrivals(params);
        const chunk = data?.list ?? [];
        this.reservations = [...this.reservations, ...chunk];
        this.page = nextPage;

        const totalCount = data?.totalCount;
        if (typeof totalCount === "number") {
          this.totalReservations = Math.max(1, Math.ceil(totalCount / this.limit));
        }
      } catch (e) {
        console.error("arrivals.loadMore error", e);
      } finally {
        this.isLoadingMore = false;
      }
    },
  },
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
