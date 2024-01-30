<template>
  <div>
    <label class="pl-px text-[15px] font-medium tracking-tight">{{
      $attrs.label
    }}</label>
    <textarea
      @input="e => onInput(e)"
      :value="props.modelValue"
      rows="3"
      v-bind:="$attrs"
      spellcheck="false"
      class="mt-1 w-full resize-none rounded-md border-neutral-300 !px-3 !py-2.5 text-sm placeholder-neutral-400 focus:border-neutral-500 focus:outline-none focus:outline-transparent focus:ring-0 focus:ring-transparent"
    ></textarea>
  </div>
</template>
<script setup>
import { ref } from 'vue'
const inputValue = ref(props.modelValue)
const props = defineProps(['modelValue'])
const emit = defineEmits(['update:modelValue'])

const onInput = event => {
  inputValue.value = event.target.value
  emit('update:modelValue', inputValue.value)
}
</script>
