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
import { reservations } from '@/api';

export default {
  name: 'ReservationItem',
  props: {
    reservation: { type: Object, required: true },
  },
  methods: {
    viewReservationDetails(idReservation) {
      this.$router.push({ name: 'ReservationDetails', params: { idReservation } });
    },
    async deleteReservation(idReservation) {
      try {
        await reservations.remove(idReservation);
        this.$emit('deleteReservation', idReservation);
      } catch (err) {
        console.error('Failed to delete reservation:', err);
      }
    },
  },
};
</script>


<style scoped>
.item-div{
  display: grid;
  grid-template-columns: 130px 130px 1.6fr 110px 1.2fr 240px;
  align-items: center;
  gap: 10px;

  padding: 10px 14px;
  margin: 8px 16px;
  background-color:#C8B6A6;
  border-radius: 8px;
  font-weight: 600;
  font-size: 16px;
}

.item { display: flex; justify-content: center; }
.item.name { justify-content: flex-start; }

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
