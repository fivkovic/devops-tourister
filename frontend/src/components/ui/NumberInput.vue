<template>
  <div
    @mouseover="hovered = true"
    @mouseleave="hovered = false"
    class="group max-w-min"
  >
    <label
      v-if="$attrs.label"
      :for="$attrs.id"
      class="pb-0.5 block pl-px text-sm font-medium tracking-tight text-neutral-700"
    >
      {{ $attrs.label }}
    </label>
    <div
      class="relative flex w-full items-center transition-all group-hover:border-neutral-500"
      :class="{
        'pointer-events-none text-neutral-300': $attrs.disabled === ''
      }"
    >
      <div
        ref="prepend"
        v-if="$slots.prepend"
        class="absolute left-0 flex h-full items-center"
      >
        <slot
          name="prepend"
          :input="input"
          :focused="focused"
          :hovered="hovered"
        />
      </div>
      <div v-if="props.hasSpinner" class="peer absolute right-1.5 select-none">
        <ChevronUpIcon
          @click="increment()"
          class="h-4 w-4 cursor-pointer rounded hover:bg-gray-100"
        />
        <ChevronDownIcon
          @click="decrement()"
          class="h-4 w-4 cursor-pointer rounded hover:bg-gray-100"
        />
      </div>

      <input
        v-bind="$attrs"
        type="number"
        :value="modelValue ?? inputValue"
        @input="onInput"
        onkeydown="return event.keyCode !== 69 && event.keyCode !== 189 && event.keyCode !== 187"
        class="block select-none rounded-md border-neutral-300 px-2.5 py-2.5 text-sm placeholder-neutral-400
               transition-colors focus:border-neutral-500 focus:ring-0 focus:ring-transparent 
               disabled:text-neutral-300 focus:disabled:border-neutral-300 group-hover:border-neutral-500 
               group-hover:disabled:border-neutral-300 peer-hover:border-neutral-500"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, useAttrs } from 'vue'
import { ChevronUpIcon, ChevronDownIcon } from 'vue-tabler-icons'

const props = defineProps({
  modelValue: { type: [String, Number], default: null },
  hasSpinner: { type: Boolean, default: true }
})
const emit = defineEmits(['update:modelValue'])
const attrs = useAttrs()

const inputValue = ref(1)
const focused = ref(false)
const hovered = ref(false)
const input = ref()

const onInput = event => {
  props.modelValue || (inputValue.value = event.target.value)
  emit('update:modelValue', event.target.value)
}

const decrement = () => {
  const value = Math.max(
    Number(props.modelValue ?? inputValue.value) - Number(attrs.step ?? 1),
    attrs.min ?? -100
  )
  props.modelValue || (inputValue.value = value)
  emit('update:modelValue', value)
}

const increment = () => {
  const value = Math.min(
    Number(props.modelValue ?? inputValue.value) + Number(attrs.step ?? 1),
    attrs.max ?? 100
  )
  props.modelValue || (inputValue.value = value)
  emit('update:modelValue', value)
}
</script>

<script>
export default { inheritAttrs: false }
</script>

<style>
input[type='number']::-webkit-outer-spin-button,
input[type='number']::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type='number'] {
  -moz-appearance: textfield;
}
</style>
