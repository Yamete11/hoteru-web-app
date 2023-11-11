<template>
  <div class="input-form">
    <label><slot></slot></label>
    <input :value="modelValue" @input="updateInput" class="input" type="text">
  </div>
</template>

<script>
export default {
  name: "RegistrationInput",
  props:{
    modelValue: [String, Number]
  },
  methods:{
    updateInput(event){
      this.$emit('update:modelValue', event.target.value)
    }
  }
}
</script>

<style scoped>
.input-form {
  display: flex;
  align-items: center;
  padding: 5px;
  margin: 5px;
  width: 50px;
}

.input-form label {
  margin-right: 10px;
  font-weight: bold;
  color: white;
}

.input-form input[type="text"] {
  flex-grow: 1;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  width: 150px;
}
</style>


