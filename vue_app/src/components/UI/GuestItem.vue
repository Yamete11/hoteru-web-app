<template>
  <div class="item-div">
    <span class="item name">{{ guest.name }}</span>
    <span class="item surname">{{ guest.surname }}</span>
    <span class="item telNumber">{{ guest.telNumber }}</span>
    <span class="item email">{{ guest.email }}</span>
    <div class="item-btns">
      <button class="btn" type="button">
        Details
      </button>
      <button class="btn" type="button" @click="deleteGuest(guest.idGuest)">
        Delete
      </button>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "GuestItem",
  props:{
    guest:{
      type: Object,
      required: true
    }
  },
  methods: {
    deleteGuest(idGuest) {
      axios.delete(`https://localhost:44384/api/Room/${idGuest}`)
          .then(() => {
            this.$emit('deleteGuest', idGuest);
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
  padding: 0.5rem 0.5rem 0.5rem 3rem;
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

.item.name {
  display: flex;
  justify-content: center;
  flex-basis: 10%;

}
.item.surname {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.telNumber {
  display: flex;
  justify-content: center;
  flex-basis: 10%;
}
.item.email {
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