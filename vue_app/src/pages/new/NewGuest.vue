<template>
  <div class="newRoom-component">
    <navbar></navbar>
    <sidebar></sidebar>
    <div class="main">
      <h1>New Room</h1>
      <form @submit.prevent class="creating-form">
        <div class="input-form">
          <label>Name: </label>
          <input
              v-model="formData.Number"
              class="input"
              type="text"
              placeholder="Enter room number"
          >
          <span class="error-message" v-if="errors.Number">{{ errors.Number[0] }}</span>
        </div>
        <div class="input-form">
          <label>Surname: </label>
          <input
              v-model="formData.Capacity"
              class="input"
              type="text"
              placeholder="Enter room capacity"
          >
          <span class="error-message" v-if="errors.Capacity">{{ errors.Capacity[0] }}</span>
        </div>
        <div class="input-form">
          <label>Price: </label>
          <input
              v-model="formData.Price"
              class="input"
              type="text"
              placeholder="Enter room price"
          >
          <span class="error-message" v-if="errors.Price">{{ errors.Price[0] }}</span>
        </div>
        <div class="input-form">
          <label>Type: </label>
          <select v-model="formData.Type">
            <option disabled value="">Select type</option>
            <option v-for="roomType in roomTypes" :key="roomType.idRoomType" :value="String(roomType.idRoomType)">{{ roomType.title }}</option>
          </select>
          <span class="error-message" v-if="errors.Type">{{ errors.Type[0] }}</span>
        </div>
        <div class="input-form">
          <label>Status: </label>
          <select v-model="formData.Status">
            <option disabled value="">Select status</option>
            <option v-for="roomStatus in roomStatuses" :key="roomStatus.idRoomStatus" :value="String(roomStatus.idRoomStatus)">{{ roomStatus.title }}</option>
          </select>
          <span class="error-message" v-if="errors.Status">{{ errors.Status[0] }}</span>
        </div>
        <div class="registration-class">
          <router-link class="registration-btn" to="/rooms">Cancel</router-link>
          <button class="registration-btn" @click="addRoom">Confirm</button>
        </div>
      </form>
    </div>

  </div>
</template>

<script>
export default {
  name: "NewGuest"
}
</script>

<style scoped>

</style>