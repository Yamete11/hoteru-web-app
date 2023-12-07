<template>
  <div v-if="rooms.length > 0" class="room-list">
    <transition-group name="room-list">
      <room-item
          v-for="room in rooms"
          :room="room"
          :key="room.idRoom"
          @deleteRoom="deleteRoom"
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
  },
  methods: {
    deleteRoom(idRoom) {
      this.$emit('deleteRoom', idRoom);
    }
  }
}
</script>

<style scoped>
h2 {
  display: flex;
  justify-content: center;
}

.room-list-item {
  display: inline-block;
  margin-right: 10px;
}
.room-list-enter-active,
.room-list-leave-active {
  transition: all 0.4s ease;
}
.room-list-enter-from,
.room-list-leave-to {
  opacity: 0;
  transform: translateX(130px);
}

.room-list-move {
  transition: transform 0.4s ease;
}
</style>