<template>
  <div v-if="guests.length > 0" class="guest-list">
    <transition-group name="guest-list">
      <guest-item
          v-for="guest in guests"
          :guest="guest"
          :key="guest.idGuest"
          @deleteRoom="deleteGuest"
      />
    </transition-group>
  </div>
  <h2 v-else>
    List of guests is empty
  </h2>
</template>

<script>
export default {
  name: "GuestList",
  props:{
    guests:{
      type: Array,
      required: true
    }
  },
  methods: {
    deleteGuest(idGuest) {
      this.$emit('deleteGuest', idGuest);
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