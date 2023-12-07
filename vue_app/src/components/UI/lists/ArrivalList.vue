<template>
  <div v-if="reservations.length > 0" class="room-list">
    <transition-group name="room-list">
      <arrival-item
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
  name: "ArrivalList",
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
</style>