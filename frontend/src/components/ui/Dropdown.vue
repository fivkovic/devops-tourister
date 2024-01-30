<template>
  <Listbox
    as="div"
    class="relative items-center"
    :class="containerClass"
    v-model="selection"
    v-slot="{ open }"
  >
    <ListboxLabel
      v-if="label"
      class="whitespace-nowrap text-sm font-medium tracking-tight text-neutral-700"
      :class="labelClass"
    >
      {{ label }}
    </ListboxLabel>
    <div>
      <ListboxButton
        class="relative flex w-full items-center whitespace-nowrap py-2.5 px-3 text-left text-[13.5px] font-medium"
        :class="[
            buttonClass(open),
            (attrs.disabled || attrs.disabled === '') && '!text-neutral-300'
          ]"
      >
        {{ selection.name }}
        <span class="pointer-events-none ml-2 flex items-center">
          <ChevronDownIcon
            :class="[
               slim ? 'ml-0.5 h-4 w-4' : 'h-5 w-5',
               (attrs.disabled || attrs.disabled === '') && '!text-neutral-300'
            ]"
            aria-hidden="true"
          />
        </span>
      </ListboxButton>

      <Transition
        leave-active-class="transition duration-100 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <ListboxOptions
          class="absolute z-10 mt-1 max-h-60 overflow-auto rounded-md bg-white py-1 text-[13px] shadow-lg ring-1 ring-black ring-opacity-10 focus:outline-none"
        >
          <ListboxOption
            v-slot="{ active, selected }"
            v-for="value in values"
            :key="value.value"
            :value="value"
            as="template"
          >
            <div
              :class="[
                { 'bg-neutral-50': active },
                'relative flex cursor-pointer select-none items-start justify-between space-x-3 py-2 px-3.5'
              ]"
            >
              <span
                class="text-[13.5px]"
                :class="[selected ? 'font-medium' : 'font-normal', 'block truncate']"
              >
                {{ value.name }}
              </span>
              <CheckIcon v-if="selected" class="h-4 w-4" aria-hidden="true" />
            </div>
          </ListboxOption>
        </ListboxOptions>
      </Transition>
    </div>
  </Listbox>
</template>

<script setup>
import { ref, watch, useAttrs } from 'vue'
import {
  Listbox,
  ListboxLabel,
  ListboxButton,
  ListboxOptions,
  ListboxOption
} from '@headlessui/vue'
import { CheckIcon, ChevronDownIcon } from 'vue-tabler-icons'

const attrs = useAttrs()
const emit = defineEmits(['change'])
const props = defineProps({
  values: Array,
  name: String,
  label: String,
  slim: Boolean,
  labelPosition: String,
  default: {
    type: String,
    required: false
  },
  selectedValue: Object
})

const containerClass = props.labelPosition == 'left' && 'flex'
const labelClass = props.labelPosition == 'left' ? !props.slim && 'mr-2' : '-mt-1 pl-px'
const buttonClass = open => {
  return (
    !props.slim &&
    `rounded-md border border-neutral-300 transition-all hover:border-neutral-500 ${
      open && 'border-neutral-500'
    }`
  )
}

const selection = ref()
selection.value = props.selectedValue ?? props.default ?? props.values[0]
watch(
  () => selection.value,
  () => {
    emit('change', selection.value)
  }
)
</script>
