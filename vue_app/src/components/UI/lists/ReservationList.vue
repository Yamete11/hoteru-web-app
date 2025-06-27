<template>
  <div v-if="reservations.length > 0" class="list">
    <transition-group name="list">
      <reservation-item
          v-for="reservation in reservations"
          :reservation="reservation"
          :key="reservation.idReservation"
          @deleteReservation="deleteReservation"
      />
    </transition-group>
  </div>
  <h2 v-else>
    The reservation list is empty
  </h2>
</template>

<script>
export default {
  name: "ReservationList",
  props:{
    reservations:{
      type: Array,
      required: true
    }
  },
  methods: {
    deleteReservation(idReservation) {
      this.$emit('deleteReservation', idReservation);
    }
  }
}
</script>

<style scoped>
h2 {
  display: flex;
  justify-content: center;
}

.list-item {
  display: inline-block;
  margin-right: 10px;
}
.list-enter-active,
.list-leave-active {
  transition: all 0.4s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateX(130px);
}

.list-move {
  transition: transform 0.4s ease;
}
</style>