<template>
  <div class="sidebar" data-testid="sidebar">
    <sidebar-button
        v-for="(button, index) in buttons"
        :key="index"
        :class="{ active: activeButton === button.path }"
        @click="setActive(button.path)"
        :path="button.path"
        :data-testid="`sidebar-button-${button.name.toLowerCase()}`"
    >
      {{ button.name }}
    </sidebar-button>
  </div>
</template>

<script>
export default {
  name: "Sidebar",
  data() {
    return {
      activeButton: '',
      buttons: [
        { name: 'Arrivals', path: '/arrivals' },
        { name: 'Reservations', path: '/reservations' },
        { name: 'Guests', path: '/guests' },
        { name: 'Rooms', path: '/rooms' },
        { name: 'Services', path: '/services' },
        { name: 'History', path: '/history' },
      ]
    };
  },
  created() {
    this.setActive(this.$route.path);
  },
  methods: {
    setActive(path) {
      this.activeButton = path;
      if (this.$route.path !== path) {
        this.$router.push(path);
      }
    }
  }
}
</script>

<style scoped>
.sidebar {
  position: fixed;
  top: 8vh;
  left: 0;
  display: flex;
  align-items: center;
  flex-direction: column;
  width: 8%;
  background-color: #A4907C;
  height: 92vh;
}
</style>
