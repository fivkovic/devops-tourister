<template>
  <form @submit.prevent="forward" class="space-y-3">
    <Input
      required
      class="h-12 w-full"
      type="email"
      name="email"
      placeholder="Email"
      label="Email address"
    />
    <PasswordInput
      required
      class="h-12 w-full"
      name="password"
      placeholder="Password"
      label="Enter password"
    />
    <div class="flex justify-end">
      <Button
        type="submit"
        class="group mt-7 flex items-center space-x-2 !rounded-full bg-black !px-6 !py-2.5 text-white"
      >
        <div>Register</div>
        <ArrowNarrowRightIcon stroke-width="1.25" />
      </Button>
    </div>
  </form>
</template>

<script setup>
import { formData } from '../utility/forms'
import Input from '../ui/Input.vue'
import PasswordInput from '../ui/PasswordInput.vue'
import Button from '../ui/Button.vue'
import { ArrowNarrowRightIcon } from 'vue-tabler-icons'

const emit = defineEmits(['register'])

const forward = event => {
  const data = formData(event)
  emit('register', data)
}
</script>
