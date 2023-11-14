<template>
  <div v-if="services.length > 0" class="room-list">
    <transition-group name="room-list">
      <service-item
          v-for="service in services"
          :service="service"
          :key="service.idService"
          @deleteService="deleteService"
      />
    </transition-group>
  </div>
  <h2 v-else>
    List of services is empty
  </h2>
</template>

<script>
export default {
  name: "ServiceList",
  props:{
    services:{
      type: Array,
      required: true
    }
  },
  methods: {
    deleteService(idService) {
      this.$emit('deleteService', idService);
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