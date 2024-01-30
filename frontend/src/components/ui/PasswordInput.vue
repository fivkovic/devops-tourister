<template>
  <Input :type="visible ? 'text' : 'password'">
    <template #append="{ input }">
      <button
        type="button"
        @click="toggleVisibility(input)"
        class="mr-1 px-2 pt-0.5 text-[13px] font-medium text-neutral-600 hover:text-neutral-800"
      >
        {{ visible ? 'Hide' : 'Show' }}
      </button>
    </template>
  </Input>
</template>

<script setup>
import { ref } from 'vue'
import Input from './Input.vue'

const visible = ref(false)

const toggleVisibility = input => {
  visible.value = !visible.value
  input.focus()
}
</script>

<style>
::-ms-reveal {
  display: none;
}
</style>
