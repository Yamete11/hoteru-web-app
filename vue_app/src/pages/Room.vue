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

          <template v-if="searchField === 'number'">
            <input
                type="text"
                class="search-input"
                v-model="searchQuery"
                placeholder="Search by number…"
                data-testid="search-input"
            />
          </template>

          <template v-else-if="searchField === 'capacity'">
            <div class="flex gap-3 items-center">
              <input
                  type="number"
                  min="0"
                  class="search-input"
                  v-model.number="capacityMin"
                  placeholder="Min capacity"
                  data-testid="capacity-min"
              />
              <span>–</span>
              <input
                  type="number"
                  min="0"
                  class="search-input"
                  v-model.number="capacityMax"
                  placeholder="Max capacity"
                  data-testid="capacity-max"
              />
            </div>
          </template>

          <template v-else-if="searchField === 'type'">
            <select v-model="selectedType" class="search-select" data-testid="room-type-select">
              <option value="">All types</option>
              <option v-for="opt in typeOptions" :key="opt" :value="opt">{{ opt }}</option>
            </select>
          </template>

          <template v-else-if="searchField === 'status'">
            <select v-model="selectedStatus" class="search-select" data-testid="room-status-select">
              <option value="">All statuses</option>
              <option v-for="opt in statusOptions" :key="opt" :value="opt">{{ opt }}</option>
            </select>
          </template>

          <router-link to="/new-room" class="new-room-button" data-testid="new-room-button">
            New Room
          </router-link>
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
            <room-list
                :rooms="rooms"
                @deleteRoom="deleteRoom"
                @occupiedDeleteAttempt="showOccupiedWarning"
            />
            <div
                :key="`${searchField}:${searchQuery}:${capacityMin}:${capacityMax}:${selectedType}:${selectedStatus}`"
                v-intersection="loadMore"
                class="observer"
            />
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
import { rooms } from "@/api";
import debounce from "lodash.debounce";

export default {
  name: "Room",
  data() {
    return {
      isLoading: false,
      isLoadingMore: false,
      rooms: [],

      searchField: "number",
      searchQuery: "",
      capacityMin: null,
      capacityMax: null,
      selectedType: "",
      selectedStatus: "",

      typeOptions: [],
      statusOptions: [],

      page: 1,
      limit: 15,
      totalPages: 0,

      isOccupiedWarningVisible: false,

      _debouncedFetch: null,
    };
  },

  created() {
    this._debouncedFetch = debounce(() => this.fetchRooms(), 300);
  },

  async mounted() {
    await this.fetchMeta();
    await this.fetchRooms();

    if (this.$route.query.created === "true") {
      notify({
        title: "Room Created",
        text: "Room has been successfully created.",
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
    searchField() {
      this.page = 1;
      this.searchQuery = "";
      this.capacityMin = null;
      this.capacityMax = null;
      this.selectedType = "";
      this.selectedStatus = "";
      this._debouncedFetch();
    },
    searchQuery() {
      if (this.searchField === "number") {
        this.page = 1;
        this._debouncedFetch();
      }
    },
    capacityMin() {
      if (this.searchField === "capacity") {
        this.page = 1;
        this._debouncedFetch();
      }
    },
    capacityMax() {
      if (this.searchField === "capacity") {
        this.page = 1;
        this._debouncedFetch();
      }
    },
    selectedType() {
      if (this.searchField === "type") {
        this.page = 1;
        this._debouncedFetch();
      }
    },
    selectedStatus() {
      if (this.searchField === "status") {
        this.page = 1;
        this._debouncedFetch();
      }
    },
  },

  methods: {
    asArray(src) {
      return Array.isArray(src) ? src : (Array.isArray(src?.list) ? src.list : []);
    },

    async fetchMeta() {
      try {
        const [typesRes, statusesRes] = await Promise.all([rooms.types(), rooms.statuses()]);

        const typesArr = Array.isArray(typesRes?.data) ? typesRes.data : [];
        const statusesArr = Array.isArray(statusesRes?.data) ? statusesRes.data : [];

        const toStr = (x) =>
            typeof x === "string"
                ? x
                : (x?.title ?? x?.name ?? x?.label ?? x?.value ?? "").toString();

        this.typeOptions = typesArr.map(toStr).filter(Boolean);
        this.statusOptions = statusesArr.map(toStr).filter(Boolean);

      } catch (e) {
        console.error("fetchMeta error", e);
        this.typeOptions = [];
        this.statusOptions = [];
      }
    },


    showOccupiedWarning() {
      if (this.isOccupiedWarningVisible) return;
      this.isOccupiedWarningVisible = true;
      notify({
        title: "Warning",
        text: 'Cannot delete a room with status "Occupied".',
        type: "warn",
        duration: 3000,
      });
      setTimeout(() => {
        this.isOccupiedWarningVisible = false;
      }, 3000);
    },

    deleteRoom(idRoom) {
      this.rooms = this.rooms.filter((r) => r.idRoom !== idRoom);
      notify({
        title: "Room Deleted",
        text: "Room has been deleted.",
        type: "success",
        duration: 3000,
      });
    },

    toInt(v) {
      const n = parseInt(v, 10);
      return Number.isFinite(n) ? n : null;
    },

    async fetchRooms() {
      try {
        this.isLoading = true;
        this.page = 1;

        const params = { page: this.page, limit: this.limit };

        if (this.searchField === "number") {
          params.searchField = "number";
          params.searchQuery = (this.searchQuery || "").trim();
        } else if (this.searchField === "type" && this.selectedType) {
          params.searchField = "type";
          params.searchQuery = this.selectedType;
        } else if (this.searchField === "status" && this.selectedStatus) {
          params.searchField = "status";
          params.searchQuery = this.selectedStatus;
        }

        const data = await rooms.list(params);
        let list = Array.isArray(data?.list) ? data.list : [];

        if (this.searchField === "capacity") {
          const min = this.toInt(this.capacityMin);
          const max = this.toInt(this.capacityMax);
          if (min != null || max != null) {
            list = list.filter((r) => {
              const cap = this.toInt(r?.capacity);
              if (cap == null) return false;
              if (min != null && cap < min) return false;
              if (max != null && cap > max) return false;
              return true;
            });
          }
        }

        this.rooms = list;
        const totalCount = data?.totalCount ?? 0;
        this.totalPages = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (e) {
        console.error("rooms.fetchRooms error", e);
        this.rooms = [];
        this.totalPages = 1;
      } finally {
        this.isLoading = false;
      }
    },

    async loadMore() {
      if (this.isLoading || this.isLoadingMore) return;
      if (this.page >= this.totalPages) return;

      try {
        this.isLoadingMore = true;
        const next = this.page + 1;

        const params = { page: next, limit: this.limit };
        if (this.searchField === "number") {
          params.searchField = "number";
          params.searchQuery = (this.searchQuery || "").trim();
        } else if (this.searchField === "type" && this.selectedType) {
          params.searchField = "type";
          params.searchQuery = this.selectedType;
        } else if (this.searchField === "status" && this.selectedStatus) {
          params.searchField = "status";
          params.searchQuery = this.selectedStatus;
        }

        const data = await rooms.list(params);
        let nextChunk = Array.isArray(data?.list) ? data.list : [];

        if (this.searchField === "capacity") {
          const min = this.toInt(this.capacityMin);
          const max = this.toInt(this.capacityMax);
          if (min != null || max != null) {
            nextChunk = nextChunk.filter((r) => {
              const cap = this.toInt(r?.capacity);
              if (cap == null) return false;
              if (min != null && cap < min) return false;
              if (max != null && cap > max) return false;
              return true;
            });
          }
        }

        this.rooms = [...this.rooms, ...nextChunk];
        this.page = next;

        const totalCount = data?.totalCount ?? 0;
        this.totalPages = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (e) {
        console.error("rooms.loadMore error", e);
      } finally {
        this.isLoadingMore = false;
      }
    },
  },
};
</script>

<style scoped>
.room-component { display: flex; flex-direction: column; background-color: #F1DEC9; height: 100vh; }
.new-room-button { font-weight: bold; font-size: 20px; padding: 0.5rem 3rem; background-color: #A4907C; color: white; text-decoration: none; border-radius: 4px; white-space: nowrap; }
.header.number,.header.capacity,.header.type,.header.status,.header.action { display: flex; justify-content: center; flex-basis: 10%; }
.observer { height: 10px; margin-bottom: 20px; }
.flex { display: flex; } .gap-3 { gap: 0.75rem; } .items-center { align-items: center; }
</style>
