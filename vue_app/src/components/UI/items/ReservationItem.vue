<template>
  <div class="item-div">
    <span class="item in">{{ reservation.in }}</span>
    <span class="item out">{{ reservation.out }}</span>
    <span class="item name">{{ reservation.name }} {{ reservation.surname }}</span>
    <span class="item roomNumber">{{ reservation.roomNumber }}</span>
    <span class="item bookedBy">{{ reservation.bookedBy }}</span>
    <div class="item-btns">
      <button class="btn" type="button" @click="viewReservationDetails(reservation.idReservation)">
        Details
      </button>
      <button class="btn" type="button" @click="deleteReservation(reservation.idReservation)">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import store from "@/store";

export default {
  name: "ReservationItem",
  props:{
    reservation:{
      type: Object,
      required: true
    }
  },
  methods: {
    viewReservationDetails(idReservation) {
      this.$router.push({ name: 'ReservationDetails', params: { idReservation: idReservation } });
    },
    deleteReservation(idReservation) {
      axios.delete(`https://localhost:44384/api/Reservation/${idReservation}`, {
        headers: {
          'Authorization': `Bearer ${store.getters.getToken}`
        }
      })
          .then(() => {
            this.$emit('deleteReservation', idReservation);
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
  padding: 0.7rem 0.5rem 0.7rem 3rem;
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

.item.in {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.item.out {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.name {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.roomNumber {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.bookedBy {
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