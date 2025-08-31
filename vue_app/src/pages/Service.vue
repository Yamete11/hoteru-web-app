<template>
  <div class="service-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <select v-model="searchField" class="search-select" data-testid="service-search-select">
            <option value="title">Title</option>
            <option value="sum">Sum</option>
            <option value="description">Description</option>
          </select>

          <input
              type="text"
              class="search-input"
              v-model="searchQuery"
              :placeholder="searchField === 'sum' ? 'Min sum…' : `Search by ${searchField}...`"
              data-testid="search-input"
          />

          <router-link to="/new-service" class="new-service-button" data-testid="new-service-button">
            New Service
          </router-link>
        </div>

        <div class="main-bot">
          <div class="table-headers">
            <span class="header title">Title</span>
            <span class="header sum">Sum</span>
            <span class="header description">Description</span>
            <span class="header action">Action</span>
          </div>

          <div v-if="!isLoading">
            <service-list
                :services="services"
                @deleteService="deleteService"
                @notificationDeleteAttempt="showServiceDeletedNotification"
            />
            <div
                :key="`${searchField}:${searchQuery}`"
                v-intersection="loadMore"
                class="observer"
            ></div>
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
import { services } from "@/api";
import debounce from "lodash.debounce";

export default {
  name: "Service",
  data() {
    return {
      isLoading: false,
      isLoadingMore: false,
      services: [],
      searchQuery: "",
      searchField: "title",
      totalPages: 0,
      page: 1,
      limit: 15,
      isServiceDeletedNotificationVisible: false,

      _debouncedFetch: null,
      _requestId: 0,
    };
  },

  created() {
    this._debouncedFetch = debounce(() => this.fetchServices(), 350);
  },

  mounted() {
    this.fetchServices();
    if (this.$route.query.created === "true") {
      notify({
        title: "Service Created",
        text: "The service has been successfully created.",
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
    sumThreshold() {
      const text = String(this.searchQuery ?? "").trim();
      if (!text) return null;
      const normalized = text.replace(/\s+/g, "").replace(",", ".");
      const m = normalized.match(/-?\d+(?:\.\d+)?/);
      if (!m) return null;
      const n = parseFloat(m[0]);
      return Number.isFinite(n) ? n : null;
    },

    sumValue(val) {
      if (typeof val === "number") return val;
      const s = String(val ?? "");
      const normalized = s.replace(/\s+/g, "").replace(",", ".");
      const m = normalized.match(/-?\d+(?:\.\d+)?/);
      return m ? parseFloat(m[0]) : NaN;
    },

    showServiceDeletedNotification() {
      if (this.isServiceDeletedNotificationVisible) return;
      this.isServiceDeletedNotificationVisible = true;
      notify({
        title: "Service Deleted",
        text: "Service has been successfully deleted.",
        type: "success",
        duration: 4000,
      });
      setTimeout(() => {
        this.isServiceDeletedNotificationVisible = false;
      }, 4000);
    },

    deleteService(idService) {
      this.services = this.services.filter((s) => s.idService !== idService);
    },

    async fetchServices() {
      const rid = ++this._requestId;
      try {
        this.isLoading = true;
        this.page = 1;

        const params = { page: this.page, limit: this.limit };

        if (this.searchField !== "sum") {
          params.searchField = this.searchField;
          params.searchQuery = this.searchQuery?.trim();
        }

        const data = await services.list(params);
        if (rid !== this._requestId) return;

        let list = data?.list ?? [];

        if (this.searchField === "sum") {
          const thr = this.sumThreshold();
          if (thr != null) {
            list = list.filter((s) => this.sumValue(s?.sum) >= thr);
          }
        }

        this.services = list;
        const totalCount = data?.totalCount ?? 0;
        this.totalPages = Math.max(1, Math.ceil(totalCount / this.limit));
      } catch (e) {
        console.error("services.fetchServices error", e);
        this.services = [];
        this.totalPages = 1;
      } finally {
        if (rid === this._requestId) this.isLoading = false;
      }
    },

    async loadMore() {
      if (this.isLoading || this.isLoadingMore) return;
      if (this.page >= this.totalPages) return;

      try {
        this.isLoadingMore = true;
        const nextPage = this.page + 1;

        const params = { page: nextPage, limit: this.limit };
        if (this.searchField !== "sum") {
          params.searchField = this.searchField;
          params.searchQuery = this.searchQuery?.trim();
        }

        const data = await services.list(params);

        let nextChunk = data?.list ?? [];
        if (this.searchField === "sum") {
          const thr = this.sumThreshold();
          if (thr != null) {
            nextChunk = nextChunk.filter((s) => this.sumValue(s?.sum) >= thr);
          }
        }

        this.services = [...(this.services || []), ...nextChunk];
        this.page = nextPage;

        const totalCount = data?.totalCount;
        if (typeof totalCount === "number") {
          this.totalPages = Math.max(1, Math.ceil(totalCount / this.limit));
        }
      } catch (e) {
        console.error("services.loadMore error", e);
      } finally {
        this.isLoadingMore = false;
      }
    },
  },
};
</script>

<style scoped>
.service-component {
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

.new-service-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 36px;
  padding: 0 16px;
  font-size: 0.9rem;
  font-weight: 700;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  text-decoration: none;
  transition: background-color .2s ease, transform .05s ease;
}
.new-service-button:hover {
  background-color: #3a3a3a;
}
.new-service-button:active {
  transform: translateY(1px);
}


.table-headers {
  display: grid;
  grid-template-columns: 1.4fr 120px 2fr 240px;
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
.header.title, .header.description {
  justify-content: flex-start;
}
.header.action {
  justify-content: flex-end;
}

.main-bot { margin-top: 8px; }

.observer { height: 10px; margin-bottom: 20px; }
</style>

