<template>
  <form 
    @submit.prevent="onSubmit" 
    id="main-info-form" 
    name="main-info-form" 
    class="w-96"
  >
    <h3 class="text-lg font-medium">Tell us about your property</h3>
    <h5 class="mb-5 whitespace-nowrap text-neutral-500">Your customers will value an appealing description</h5>
    <Input
      v-model="name"
      required
      autocomplete="false"
      class="h-12 w-full"
      placeholder="Name of the property"
      name="name"
      label="Name"
    />
    <div class="mt-3"></div>
    <TextArea
      required
      v-model="description"
      rows="8"
      maxlength="2000"
      name="description"
      class="pt-1"
      placeholder="Promotional description of the property"
      label="Description"
    />
    <p class="mt-0 text-xs text-neutral-400">Maximum 2000 characters</p>
  </form>
</template>

<script setup>
import { ref } from 'vue'

import Input from '@/components/ui/Input.vue'
import TextArea from '@/components/ui/TextArea.vue'

const props = defineProps(['name', 'description'])
const emit = defineEmits(['change'])

const name = ref(props.name)
const description = ref(props.description)

const onSubmit = () => {
  emit('change', {
    name: name.value,
    description: description.value
  })
}

const getValues = () => {
  document.getElementById('main-info-form').requestSubmit()
}

defineExpose({
  getValues
})
</script>

<script>
export default {
  inheritAttrs: false
}
</script>
