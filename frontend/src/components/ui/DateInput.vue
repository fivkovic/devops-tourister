<template>
  <div class="relative h-full w-60 border-0">
    <Input
      ref="extraRef"
      :required="props.required"
      @mounted="saveInputReference"
      @click="showDatePicker()"
      @focus="showDatePicker()"
      @change="formatInput()"
      class="group -mt-2 -ml-8 h-12 w-[230px]"
      v-model="inputValue"
      :clearable="false"
      :min="lowerLimit"
      :max="upperLimit"
    >
      <template #prepend="{ focused, hovered }">
        <CalendarIcon
          class="z-10 -ml-5 mr-1.5 h-[18px] w-[18px] transition-all"
          :class="[
            focused || hovered ? 'text-neutral-700' : 'text-neutral-400'
          ]"
        />
        <div class="w-8"></div> 
      </template>
      </Input>
    <div
      @click="inputRef.focus()"
      class="absolute top-7 z-20 ml-2 h-7 w-2/3 -translate-y-1/2 whitespace-nowrap bg-white text-sm"
      :class="textColor"
      :style="{ width: inputRef?.actualWidth + 'px' }"
    >
      {{ dateValue }}
    </div>
  </div>
</template>
<script setup>
import { ref, watchEffect, computed, onMounted } from 'vue'
import Input from './Input.vue'
import { parseISO, format } from 'date-fns'
import { CalendarIcon } from 'vue-tabler-icons'
import {is} from "date-fns/locale";

const props = defineProps([
  'hasTime',
  'placeholder',
  'modelValue',
  'upperLimit',
  'lowerLimit',
  'required'
])
const emit = defineEmits(['change'])
const inputRef = ref(null)
const extraRef = ref(null)
const dateValue = ref(props.placeholder)
const inputValue = ref(props.modelValue)
const textColor = computed(() =>
  dateValue.value === props.placeholder ? 'text-gray-400' : 'text-black'
)

const saveInputReference = input => {
  inputRef.value = input.value
  inputRef.value.type = props.hasTime ? 'datetime-local' : 'date'
}

onMounted(() => props.modelValue && formatInput())

watchEffect(() => {
  if (!inputValue.value) dateValue.value = props.placeholder
})

const showDatePicker = () => {
  try {
    inputRef.value.showPicker()
    inputRef.value.focus()
  }
  catch {}
}
const formatInput = () => {
  const date = parseISO(inputValue.value)
  const dateFormat = props.hasTime ? 'EEE, MMM dd, HH:mm' : 'EEE, MMM dd'
  const newDate = format(date, dateFormat)
  dateValue.value = newDate
  emit('change', inputValue.value)
}
</script>
