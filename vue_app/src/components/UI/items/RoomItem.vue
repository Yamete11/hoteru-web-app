<template>
  <div class="item-div">
    <span class="item number" data-testid="room-number">{{ room.number }}</span>
    <span class="item capacity" data-testid="room-capacity">{{ room.capacity }}</span>
    <span class="item type" data-testid="room-type">{{ room.type }}</span>
    <span class="item status" data-testid="room-status">{{ room.status }}</span>
    <div class="item-btns">
      <button class="btn" type="button" @click="viewRoomDetails(room.idRoom)" data-testid="room-item-details-button">
        Details
      </button>

      <button class="btn" type="button" data-testid="delete-room-button" @click="deleteRoom(room.idRoom)">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
import { rooms } from '@/api';


export default {
  name: "RoomItem",
  props:{
    room:{
      type: Object,
      required: true
    }
  },
  methods: {
    viewRoomDetails(idRoom) {
      this.$router.push({ name: 'RoomDetails', params: { idRoom: idRoom } });
    },
    async deleteRoom(idRoom) {
      try {
        await rooms.remove(idRoom);
        this.$emit('deleteRoom', idRoom);
        this.$emit('notificationDeleteAttempt');
      } catch (e) {
        console.error(e);
      }
    }

  }
}
</script>



<style scoped>
.item-div{
  display: grid;
  grid-template-columns: 120px 120px 1.2fr 1.2fr 240px;
  align-items: center;
  gap: 10px;

  padding: 10px 14px;
  margin: 8px 16px;
  background-color:#C8B6A6;
  border-radius: 8px;
  font-weight: 600;
  font-size: 16px;
}

.item {
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 0;
}
.item.type, .item.status {
  justify-content: flex-start;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

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
.btn:hover  {
  background-color: #3a3a3a;
}
.btn:active {
  transform: translateY(1px);
}
.btn:disabled {
  background-color: #9a9a9a;
  border-color: #bdbdbd;
  color: #ececec;
  cursor: not-allowed;
  opacity: 0.85;
}
</style>
