<template>
  <form @submit.prevent="forward" class="w-full space-y-3">
    <div class="flex space-x-2">
      <Input
        required
        placeholder="First Name"
        class="h-12 w-full"
        name="firstName"
        label="First Name"
      />
      <Input
        required
        placeholder="Last Name"
        class="h-12 w-full"
        name="lastName"
        label="Last Name"
      />
    </div>
    <Input
      required
      placeholder="Location of Residence"
      class="h-12 w-full"
      name="residence"
      type="text"
      label="Residence"
    />
    <p class="mt-5 text-sm text-red-500">{{ error }}</p>
    <div class="flex justify-end">
      <Button
        type="submit"
        class="group mt-7 flex items-center space-x-2 !rounded-full bg-black !px-6 !py-2.5 text-white"
      >
        <div>Next</div>
        <ArrowNarrowRightIcon stroke-width="1.25" />
      </Button>
    </div>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { ArrowNarrowRightIcon } from 'vue-tabler-icons'
import { formData } from '@/components/utility/forms.js'

const emit = defineEmits(['next'])
const error = ref('')

const forward = event => {
  const data = formData(event)
  emit('next', data)
}
</script>
