<template>
  <div v-if="guests.length > 0" class="guest-list">
    <transition-group name="guest-list">
      <guest-item
          v-for="guest in guests"
          :guest="guest"
          :key="guest.idPerson"
          @deleteGuest="deleteGuest"
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
    deleteGuest(idPerson) {
      this.$emit('deleteGuest', idPerson);
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