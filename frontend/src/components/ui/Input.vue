<template>
  <div
    @mouseover="hovered = true"
    @mouseleave="hovered = false"
    class="group w-full"
  >
    <label
      v-if="$attrs.label"
      :for="$attrs.id"
      class="mb-1 block w-min whitespace-nowrap pl-px text-sm font-medium tracking-tight text-neutral-700"
    >
      {{ $attrs.label }}
    </label>
    <div class="relative">
      <div
        ref="prepend"
        v-if="$slots.prepend"
        class="absolute left-0 flex h-full items-center pb-0.5"
      >
        <slot
          name="prepend"
          :input="input"
          :focused="focused"
          :hovered="hovered"
        ></slot>
      </div>
      <input
        ref="input"
        type="text"
        spellcheck="false"
        v-bind="$attrs"
        :value="modelValue ?? inputValue"
        :style="inputStyle"
        :name="name"
        @focusin="focused = true"
        @focusout="focused = false"
        @input="e => onInput(e)"
        class="block rounded-md border-neutral-300 px-2.5 py-2.5 text-sm placeholder-neutral-400 transition-colors hover:border-neutral-500 focus:border-neutral-500 focus:ring-0 focus:ring-transparent disabled:text-neutral-300 hover:disabled:border-neutral-300 focus:disabled:border-neutral-300"
      />
      <div
        ref="append"
        class="absolute inset-y-0 right-0 flex h-full items-center"
      >
        <div
          @click="clearInput()"
          v-if="(clearable || clearable === '') && !!inputValue"
          class="m-1 cursor-pointer p-1 text-neutral-400 group-hover:text-gray-700"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-[18px] w-[18px]"
            viewBox="0 0 24 24"
            stroke-width="2"
            stroke="currentColor"
            fill="none"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </div>
        <slot
          name="append"
          v-else-if="$slots.append"
          :input="input"
          :focused="focused"
          :hovered="hovered"
        ></slot>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, defineExpose } from 'vue'

const props = defineProps(['modelValue', 'clearable', 'name'])
const emit = defineEmits(['update:modelValue', 'mounted'])

const input = ref()
const prepend = ref()
const append = ref()
const inputStyle = ref('')
const inputValue = ref(props.modelValue)
const focused = ref(false)
const hovered = ref(false)

defineExpose({ focused })

const onInput = event => {
  props.modelValue || (inputValue.value = event.target.value)
  emit('update:modelValue', event.target.value)
}

const clearInput = () => {
  inputValue.value = ''
  emit('update:modelValue', '')
}

onMounted(() => {
  inputStyle.value = `
    padding-left: ${prepend.value?.clientWidth + 4}px;
    padding-right: ${append.value?.clientWidth + 4}px;
  `
  emit('mounted', input)
})
</script>

<script>
export default {
  inheritAttrs: false
}
</script>

<style>
input[type='datetime-local']::-webkit-calendar-picker-indicator,
input[type='date']::-webkit-calendar-picker-indicator {
  background: transparent;
  bottom: 0;
  color: transparent;
  cursor: pointer;
  height: auto;
  left: 0;
  position: absolute;
  right: 0;
  top: 0;
}
</style>
