<template>
  <form
    id="add-location-form"
    name="add-location-form"
    @submit.prevent="onSubmit()"
    class="w-96"
  >
    <h3 class="text-lg font-medium">Show us where the property is located</h3>
    <h5 class="mb-5 whitespace-nowrap text-neutral-500">
      Customers will want to know the precise location
    </h5>
    <Input
      required
      id="map-location-input"
      v-model="location"
      autocomplete="off"
      class="h-12 w-full"
      placeholder="City and country of the property"
      label="Location"
    />
  </form>
</template>

<script setup>
import { ref } from 'vue'
import Input from '@/components/ui/Input.vue'

const props = defineProps(['location'])
const emit = defineEmits(['change'])

const location = ref(props.location ?? '')

const onSubmit = () => emit('change', { location: location.value })
const getValues = () => document.getElementById('add-location-form').requestSubmit()

defineExpose({ getValues })

</script>

<script>
export default {
  inheritAttrs: false
}
</script>
