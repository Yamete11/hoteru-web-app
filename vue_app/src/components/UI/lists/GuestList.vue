<template>
  <div v-if="guests.length > 0" class="list">
    <transition-group name="list">
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