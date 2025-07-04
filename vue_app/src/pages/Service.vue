<template>
  <div class="service-component">
    <notifications position="top right" />
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
              :placeholder="`Search by ${searchField}...`"
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
            <service-list :services="services" @deleteService="deleteService" @notificationDeleteAttempt="showServiceDeletedNotification"/>
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
import { notify } from "@kyvg/vue3-notification";

export default {
  name: "Service",
  data() {
    return {
      isLoading: false,
      services: [],
      searchQuery: '',
      searchField: 'title',
      totalServices: 0,
      page: 1,
      limit: 15,
      isServiceDeletedNotificationVisible: false
    };
  },
  mounted() {
    this.fetchServices();

    if (this.$route.query.created === 'true') {
      notify({
        title: 'Service Created',
        text: 'The service has been successfully created.',
        type: 'success',
        duration: 3000
      });

      this.$router.replace({ query: {} });
    }
  },
  watch: {
    searchQuery: 'fetchServices',
    searchField: 'fetchServices'
  },
  methods: {
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
      this.services = this.services.filter(service => service.idService !== idService);
    },
    async fetchServices() {
      try {
        this.isLoading = true;
        this.page = 1;
        const response = await axios.get('https://localhost:44384/api/Service', {
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
        this.services = response.data.list;
        this.totalServices = Math.ceil(response.data.totalCount / this.limit);
      } catch (error) {
        console.error(error);
      } finally {
        this.isLoading = false;
      }
    },

    async loadMore() {
      if (this.page >= this.totalServices) return;
      try {
        this.page++;
        const response = await axios.get('https://localhost:44384/api/Service', {
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
        this.services = [...this.services, ...response.data.list];
      } catch (error) {
        console.error(error);
      }
    }
  }
}
</script>

<style scoped>
.service-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
}

.new-service-button {
  font-weight: bold;
  font-size: 20px;
  padding: 0.5rem 3rem;
  background-color: #A4907C;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  white-space: nowrap;
}

.header.title,
.header.sum,
.header.description,
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
