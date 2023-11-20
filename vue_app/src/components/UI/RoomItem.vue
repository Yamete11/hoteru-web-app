<template>
  <div class="item-div">
    <span class="item number">{{ room.number }}</span>
    <span class="item capacity">{{ room.capacity }}</span>
    <span class="item type">{{ room.type }}</span>
    <span class="item status">{{ room.status }}</span>
    <div class="item-btns">
      <button class="btn" type="button" @click="viewRoomDetails(room.idRoom)">
        Details
      </button>

      <button class="btn" type="button" @click="deleteRoom(room.idRoom)">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
import axios from "axios";

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
    deleteRoom(idRoom) {
      axios.delete(`https://localhost:44384/api/Room/${idRoom}`)
          .then(() => {
            this.$emit('deleteRoom', idRoom);
          })
          .catch(error => {
            console.error(error);
          });
    }

  }
}
</script>



<style scoped>
.item-div{
  padding: 0.7rem 0.5rem 0.7rem 3rem;
  margin: 1rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color:#C8B6A6;
  border-radius: 5px;
  font-weight: bold;
  font-size: 18px;
}
.btn {
  padding: 0.5rem 1rem;
  font-size: 1rem;
  font-weight: bold;
  border-radius: 10px;
  border: 1px solid #D3C1AC;
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin: 5px;
}

.btn:hover {
  background-color: #A4907C;
  color: #FFFFFF;
}

.item.number {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.item.capacity {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.type {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.status {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item-btns {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}

</style>