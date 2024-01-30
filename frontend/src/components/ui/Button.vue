<template>
  <button
    :disabled="props.disabled"
    class="rounded-3xl px-4 py-3 text-sm font-medium"
    :class="{ 'opacity-50': props.disabled }"
  >
    <slot />
  </button>
</template>
<script setup>
const props = defineProps({
  disabled: Boolean
})
</script>
