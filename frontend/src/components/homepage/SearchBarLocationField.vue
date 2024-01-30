<template>
  <label :for="props.name" class="group relative cursor-pointer">
    <input
      :type="props.inputType"
      :id="props.name"
      :name="props.name"
      autocomplete="off"
      @blur="emit('valueChanged', { location })"
      v-model="location"
      placeholder=""
      class="focus-within:outline-none peer ml-8 w-full border-0 py-0 text-[1.075rem] font-semibold valid:translate-y-3 valid:text-black focus-within:translate-y-3 focus-within:ring-0 focus:outline-none"
      required
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
const location = ref('')
</script>