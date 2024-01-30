<template>
  <div
    class="flex flex-row items-center space-x-1.5"
    :class="{ 'flex-row-reverse  space-x-reverse': labelPosition === 'left' }"
  >
    <input
      :checked="modelValue"
      @change="$emit('update:modelValue', $event.target.checked)"
      type="checkbox"
      v-bind="$attrs"
      class="block cursor-pointer rounded border-neutral-300 p-1.5 text-sm text-black placeholder-neutral-500 transition-colors hover:border-neutral-500 focus:border-neutral-500 focus:ring-0 focus:ring-transparent disabled:text-neutral-300 hover:disabled:border-neutral-300 focus:disabled:border-neutral-300"
    />
    <label
      v-if="$attrs.label"
      :for="$attrs.id"
      class="block cursor-pointer select-none pl-px text-sm font-medium text-neutral-700 transition-all"
      :class="{ '!text-neutral-400': $attrs.disabled }"
    >
      {{ $attrs.label }}
    </label>
  </div>
</template>

<script>
export default {
  inheritAttrs: false,
  props: ['modelValue', 'labelPosition'],
  emits: ['update:modelValue']
}
</script>

<style scoped></style>
