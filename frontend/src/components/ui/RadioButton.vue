<template>
  <div
    class="flex flex-row items-center space-x-1.5"
    :class="{ 'flex-row-reverse  space-x-reverse': labelPosition == 'left' }"
  >
    <input
      :checked="modelValue"
      @change="$emit('update:modelValue', $event.target.checked)"
      type="radio"
      v-bind="$attrs"
      :value="value"
      class="block cursor-pointer rounded border-neutral-300 p-1.5 text-sm text-emerald-600 placeholder-neutral-500 transition-colors hover:border-neutral-500 focus:border-neutral-500 focus:ring-0 focus:ring-transparent disabled:text-neutral-300 hover:disabled:border-neutral-300 focus:disabled:border-neutral-300"
    />
    <label
      v-if="$attrs.label"
      :for="$attrs.id"
      class="block cursor-pointer select-none pl-px text-sm text-neutral-700"
    >
      {{ $attrs.label }}
    </label>
  </div>
</template>

<script>
export default {
  inheritAttrs: false,
  props: ['modelValue', 'labelPosition', 'value'],
  emits: ['update:modelValue']
}
</script>

<style scoped></style>
