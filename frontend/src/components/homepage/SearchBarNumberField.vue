<template>
  <label :for="props.name" class="group relative cursor-pointer">
    <input
      @change="emitChange()"
      required
      type="number"
      :id="props.name"
      min="1"
      v-model="number"
      :name="props.name"
      class="focus-within::outline-none peer ml-5 w-full max-w-[65px] border-0 py-0 text-[1.075rem] font-semibold valid:translate-y-3 valid:text-black focus-within:translate-y-3 focus-within:ring-0"
    />
    <div
      class="absolute top-0 flex cursor-pointer items-center space-x-2 text-[22px] transition-all group-focus-within:-translate-y-4 group-focus-within:text-base peer-valid:-translate-y-4 peer-valid:text-base"
    >
      <div class="-mt-0.5 text-gray-400">
        <slot></slot>
      </div>
      <p class="items-center text-black" style="font-size: inherit">
        {{ props.name }}
      </p>
    </div>
    <div
      class="mt-1 ml-8 whitespace-nowrap text-sm text-gray-400 transition-all group-focus-within:translate-y-2 peer-valid:translate-y-2"
    >
      <p class="">{{ props.description }}</p>
    </div>
  </label>
</template>

<script setup>
import { ref } from 'vue'
const props = defineProps({
  name: String,
  description: String
})
const emit = defineEmits(['valueChanged'])

const number = ref()
const emitChange = () =>
  emit('valueChanged', { [props.name.toLowerCase()]: number.value })
</script>

<style scoped="css">
input[type='number']::-webkit-inner-spin-button,
input[type='number']::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
input[type='number']:focus::-webkit-inner-spin-button,
input[type='number']::-webkit-outer-spin-button {
  -webkit-appearance: button;
}
</style>
