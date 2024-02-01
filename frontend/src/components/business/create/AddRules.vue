<template>
  <form
    id="add-rules-form"
    name="add-rules-form"
    @submit.prevent="onSubmit()"
    class="w-96"
  >
    <h3 class="mt-4 text-lg font-medium">Capacity</h3>
    <h5 class="mb-3 text-neutral-500">Tell us how many people are allowed</h5>
    <NumberInput
      required
      v-model="people"
      min="1"
      max="100"
      class="h-12 w-20 cursor-default"
    />
  </form>
</template>

<script setup>
import { ref } from 'vue'
import NumberInput from '@/components/ui/NumberInput.vue'

const props = defineProps(['people'])
const emit = defineEmits(['change'])
const people = ref(props.people ?? 1)

const onSubmit = () => emit('change', { maxPeople: people.value })
const getValues = () => document.getElementById('add-rules-form').requestSubmit()

defineExpose({ getValues })
</script>

<script>
export default {
  inheritAttrs: false
}
</script>
