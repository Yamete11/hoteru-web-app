<template>
  <div class="input-form">
    <label :for="inputId"><slot></slot></label>
    <input
        :id="inputId"
        :value="value"
        @input="updateInput($event.target.value)"
        class="input"
        type="text"
        :placeholder="placeholder"
    >
  </div>
</template>

<script>
export default {
  name: "RegistrationInput",
  props:{
    value: {
      type: [String, Number],
      default: ""
    },
    placeholder: String,
    inputId: String
  },
  methods:{
    updateInput(value){
      this.$emit('input', value);
    }
  }
}
</script>

<style scoped>
.input-form {
  display: flex;
  flex-direction: column;
  margin: 5px;
}

.input-form label {
  margin-bottom: 5px;
  font-weight: bold;
  color: #FFFFFF;
}

.input-form input[type="text"] {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  margin-bottom: 10px; /* Space after the input for any potential error messages */
}
</style>
