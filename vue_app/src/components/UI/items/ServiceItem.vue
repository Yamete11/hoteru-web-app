<template>
  <div class="item-div">
    <span class="item title" data-testid="service-item-title">{{ service.title }}</span>
    <span class="item sum" data-testid="service-item-price">{{ service.sum }}</span>
    <span class="item description" data-testid="service-item-description">{{ service.description }}</span>
    <div class="item-btns">
      <button class="btn" type="button" @click="viewServiceDetails(service.idService)" data-testid="service-item-details-button">
        Details
      </button>
      <button class="btn" type="button" @click="deleteService(service.idService)" data-testid="service-item-delete-button">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import store from "@/store";


export default {
  name: "ServiceItem",
  props:{
    service:{
      type: Object,
      required: true
    }
  },
  methods: {
    viewServiceDetails(idService) {
      this.$router.push({ name: 'ServiceDetails', params: { idService: idService } });
    },
    deleteService(idService) {
      axios.delete(`https://localhost:44384/api/Service/${idService}`, {
        headers: {
          'Authorization': `Bearer ${store.getters.getToken}`
        },
      })
          .then(() => {
            this.$emit('deleteService', idService);
          })
          .catch(error => {
            console.error(error);
          });
    }

  }
}
</script>

<style scoped>
.item-div{
  padding: 0.5rem 0.5rem 0.5rem 3rem;
  margin: 1rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color:#C8B6A6;
  border-radius: 5px;
  font-weight: bold;
  font-size: 18px;
}
.btn {
  padding: 0.5rem 1rem;
  font-size: 1rem;
  font-weight: bold;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin: 5px;
}

.btn:hover {
  background-color: #A4907C;
  color: #FFFFFF;
}

.item.title {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.item.sum {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.description {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item-btns {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
</style>