<template>
  <router-link :to="path" class="button-link"><slot></slot></router-link>
</template>

<script>
export default {
  name: "SidebarButton",
  props: {
    path: {
      type: String,
      required: true
    }
  }
}
</script>

<style scoped>
.button-link {
  display: inline-block;
  padding: 30px 40px;
  margin: 10px;
  width: 100px;
  height: 20px;
  background-color: #776B5D;
  color: white;
  text-align: center;
  text-decoration: none;
  font-weight: bold;
  border: none;
  border-radius: 5px;
  box-shadow: 0 2px 2px rgba(0, 0, 0, 0.2);
  transition: background-color 0.3s, box-shadow 0.3s;
}

.button-link:hover, .button-link:focus {
  background-color: #7C6A58;
  box-shadow: 0 4px 4px rgba(0, 0, 0, 0.2);
  text-decoration: none;
}

.button-link.active {
  background-color: #444444;
}
</style>