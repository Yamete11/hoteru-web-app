<template>
  <div class="service-component">
    <navbar></navbar>
    <div class="content">
      <sidebar></sidebar>
      <div class="main">
        <div class="main-top">
          <input type="text" class="search-input" v-model="searchQuery" placeholder="Search service by title ..." />
          <router-link to="/new-service" class="new-service-button">New Service</router-link>
        </div>
        <div class="main-bot">
          <div class="table-headers">
            <span class="header title">Title</span>
            <span class="header sum">Sum</span>
            <span class="header description">Description</span>
            <span class="header action">Action</span>
          </div>
          <div>
            <service-list :services="sortedAndSearchedPosts" @deleteService="deleteService"/>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "Service",

  data() {
    return {
      services: [],
      searchQuery: ''
    };
  },
  mounted() {
    console.log(this.$refs.observer);
    this.fetchServices();
  },
  computed:{
    sortedAndSearchedPosts(){
      return this.services.filter(service => service.title.toLowerCase().startsWith(this.searchQuery.toLowerCase()));
    }
  },
  methods: {
    async fetchServices() {
      try {
        const response = await axios.get('https://localhost:44384/api/Service');
        this.services = response.data;
        console.log(this.services)
      } catch (error) {
        console.error(error);
      }
    },
    deleteService(idService) {
      this.services = this.services.filter(service => service.idService !== idService);
    }
  },
}
</script>

<style scoped>
.service-component {
  display: flex;
  flex-direction: column;
  background-color: #F1DEC9;
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
.header.title {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.header.sum {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.header.description {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.header.action {
  display: flex;
  justify-content: center;
  flex-basis: 10%; }
</style>