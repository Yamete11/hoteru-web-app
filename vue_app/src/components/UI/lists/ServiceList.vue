<template>
  <div v-if="services.length > 0" class="list">
    <transition-group name="list">
      <service-item
          v-for="service in services"
          :service="service"
          :key="service.idService"
          @deleteService="deleteService"
          @notificationDeleteAttempt="notificationDeleteAttempt"
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
    },
    notificationDeleteAttempt() {
      this.$emit('notificationDeleteAttempt');
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