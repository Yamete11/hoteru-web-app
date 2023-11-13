<template>
  <div class="item-div">
    <span class="item number">{{ room.number }}</span>
    <span class="item capacity">{{ room.capacity }}</span>
    <span class="item type">{{ room.type }}</span>
    <span class="item status">{{ room.status }}</span>
    <div class="item-btns">
      <button class="btn details">
        Details
      </button>
      <button class="btn delete">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
export default {
  name: "RoomItem",
  props:{
    room:{
      type: Object,
      required: true
    }
  }
}
</script>

<style scoped>
.item-div {
  display: flex;
  align-items: center;
  padding: 0.5rem;
  margin: 0.5rem 0;
  background-color: #C8B6A6;
  border-radius: 5px;
  font-weight: bold;
}

.item {
  flex: 1; /* Allow each item to grow */
  text-align: center;
  display: flex;
  justify-content: center;
  align-items: center;
}

/* Match the flex-basis with the header */
.item.number { flex-basis: 20%; }
.item.capacity { flex-basis: 20%; }
.item.type { flex-basis: 20%; }
.item.status { flex-basis: 20%; }

.item-btns {
  flex-basis: 20%;
  display: flex;
  justify-content: space-evenly; /* This will space out your buttons */
}

.btn {
  padding: 0.5rem 1rem;
  font-size: 1rem;
  font-weight: bold;
  border-radius: 10px;
  border: none; /* Style as needed */
  background-color: #444444;
  color: #FFFFFF;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin: 0 5px;
}

.btn.details {
  background-color: #A4907C;
}

.btn.delete {
  background-color: #A4907C;
}

.btn:hover {
  background-color: #837C70; /* A darker shade for hover */
}
</style>
