<template>
  <div class="item-div">
    <span class="item name" data-testid="guest-name">{{ guest.name }}</span>
    <span class="item surname" data-testid="guest-surname">{{ guest.surname }}</span>
    <span class="item telNumber" data-testid="guest-telNumber">{{ guest.telNumber }}</span>
    <span class="item email" data-testid="guest-email">{{ guest.email }}</span>
    <div class="item-btns">
      <button class="btn" type="button" @click="viewGuestDetails(guest.idPerson)">
        Details
      </button>
      <button class="btn" type="button" data-testid="delete-guest-button" @click="deleteGuest(guest.idPerson)">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import store from "@/store";

export default {
  name: "GuestItem",
  props:{
    guest:{
      type: Object,
      required: true
    }
  },
  methods: {
    viewGuestDetails(idPerson) {
      this.$router.push({ name: 'GuestDetails', params: { idPerson: idPerson } });
    },
    deleteGuest(idPerson) {
      axios.delete(`https://localhost:44384/api/Guest/${idPerson}`, {
        headers: {
          'Authorization': `Bearer ${store.getters.getToken}`
        }
      })
          .then(() => {
            this.$emit('deleteGuest', idPerson);
            this.$emit('notificationDeleteAttempt');
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

.item.name {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.item.surname {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.telNumber {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.email {
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