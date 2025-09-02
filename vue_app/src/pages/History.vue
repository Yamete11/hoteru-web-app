<template>
  <div class="room-component">
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
  name: "History",
  data() {
    return {
      isLoading: false,
      isLoadingMore: false,
      reservations: [],
      page: 1,
      limit: 15,
      totalReservations: 0,
      searchQuery: "",
      searchField: "name",
      dateFrom: "",
      dateTo: "",

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
          const dateIn = new Date(res.dateIn);
          const from = this.dateFrom ? new Date(this.dateFrom) : null;
          const to = this.dateTo ? new Date(this.dateTo) : null;
          return (!from || dateIn >= from) && (!to || dateIn <= to);
        });
      }
      return this.reservations.filter(res => {
        const rawValue = res[this.searchField];
        const fieldValue = String(rawValue ?? "").toLowerCase();
        return fieldValue.startsWith(this.searchQuery.toLowerCase());
      });
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
      notify({ title: "History Deleted", text: "History has been deleted.", type: "success", duration: 3000 });
    },

    async fetchReservations() {
      const rid = ++this._requestId;
      try {
        this.isLoading = true;
        this.page = 1;

        const params = { page: this.page, limit: this.limit };
        if (this.searchField !== "date") {
          params.searchQuery = this.searchQuery?.trim();
          params.searchField = this.searchField;
        }

        const data = await reservations.history(params);

        if (rid !== this._requestId) return;

        this.reservations = data?.list ?? [];
        const totalCount = data?.totalCount ?? 0;
        this.totalReservations = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (err) {
        console.error("history.fetchReservations error", err);
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
      const next = this.page + 1;
      try {
        const params = { page: next, limit: this.limit };
        if (this.searchField !== "date") {
          params.searchQuery = this.searchQuery?.trim();
          params.searchField = this.searchField;
        }

        const data = await reservations.history(params);
        const chunk = data?.list ?? [];
        this.reservations = [...this.reservations, ...chunk];
        this.page = next;

        const totalCount = data?.totalCount;
        if (typeof totalCount === "number") {
          this.totalReservations = Math.max(1, Math.ceil(totalCount / this.limit));
        }
      } catch (err) {
        console.error("history.loadMore error", err);
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

.main-top {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 16px 0;
}
.search-select, .search-input {
  margin-right: 10px;
}

.table-headers {
  display: grid;
  grid-template-columns: 130px 130px 1.6fr 110px 1.2fr 240px;
  align-items: center;
  gap: 10px;
  padding: 8px 14px;
  margin: 8px 16px 0;
  color: #3b3b3b;
  font-weight: 700;
}
.header {
  display: flex;
  align-items: center;
  justify-content: center;
}
.header.name   {
  justify-content: flex-start;
}
.header.action {
  justify-content: flex-end;
}

.observer {
  height: 10px; margin-bottom: 20px;
}

.date-range-container {
  display: flex; gap: 1rem;
}
</style>

