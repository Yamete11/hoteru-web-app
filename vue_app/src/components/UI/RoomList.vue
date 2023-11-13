<template>
  <div v-if="rooms.length > 0" class="room-list">
    <transition-group name="room-list">
      <room-item
          v-for="room in rooms"
          :room="room"
          :key="room.id"
      />
    </transition-group>
  </div>
  <h2 v-else>
    List of rooms is empty
  </h2>
</template>

<script>
export default {
  name: "RoomList",
  components: {},

  props:{
    rooms:{
      type: Array,
      required: true
    }
  }
}
</script>

<style scoped>
</style>