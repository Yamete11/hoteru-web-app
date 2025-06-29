<template>
  <div v-if="rooms.length > 0" class="list">
    <transition-group name="list">
      <room-item
          v-for="room in rooms"
          :room="room"
          :key="room.idRoom"
          @deleteRoom="deleteRoom"
          @occupiedDeleteAttempt="occupiedDeleteAttempt"
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
    },
    occupiedDeleteAttempt() {
      this.$emit('occupiedDeleteAttempt');
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