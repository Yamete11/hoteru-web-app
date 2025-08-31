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
import { services } from '@/api';



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
    async deleteService(idService) {
      try {
        await services.remove(idService);
        this.$emit('deleteService', idService);
        this.$emit('notificationDeleteAttempt');
      } catch (e) {
        console.error(e);
      }
    }

  }
}
</script>

<style scoped>
.item-div{
  display: grid;
  grid-template-columns: 1.4fr 120px 2fr 240px;
  align-items: center;
  gap: 10px;

  padding: 10px 14px;
  margin: 8px 16px;
  background-color:#C8B6A6;
  border-radius: 8px;
  font-weight: 600;
  font-size: 16px;
}

.item { display: flex; align-items: center; justify-content: center; min-width: 0; }
.item.title,
.item.description { justify-content: flex-start; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

.item-btns {
  justify-self: end;
  display: grid;
  grid-template-columns: 120px 120px;
  gap: 10px;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 36px;
  padding: 0 12px;
  font-size: 0.85rem;
  font-weight: 700;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.2s ease, opacity 0.2s ease, transform 0.05s ease;
  text-decoration: none;
  box-sizing: border-box;
}
.btn:hover  { background-color: #3a3a3a; }
.btn:active { transform: translateY(1px); }
.btn:disabled {
  background-color: #9a9a9a;
  border-color: #bdbdbd;
  color: #ececec;
  cursor: not-allowed;
  opacity: 0.85;
}
</style>
