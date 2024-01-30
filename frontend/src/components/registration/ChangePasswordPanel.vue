<template>
  <form @submit.prevent="confirm" class="space-y-3 text-left">
    <div class="mt-4 mb-8">
      <h1 class="text-3xl font-medium tracking-tight">
        Welcome to tourister.com
      </h1>
      <p>Please change your password to proceed as an administrator.</p>
    </div>

    <PasswordInput
      required
      class="h-12 w-full"
      name="currentPassword"
      label="Current password"
      placeholder="Enter Password"
    />
    <PasswordInput
      required
      class="h-12 w-full"
      name="newPassword"
      label="New password"
      placeholder="Enter Password"
    />
    <div class="text-sm text-red-500">
      {{ formError }}
    </div>
    <div class="flex justify-end">
      <Button
        type="submit"
        class="group mt-4 flex items-center space-x-2 !rounded-full bg-emerald-600 !px-6 !py-2.5 text-white"
      >
        <div>Confirm</div>
        <ArrowNarrowRightIcon stroke-width="1.25" />
      </Button>
    </div>
    <div>
      <p class="mt-6 text-center text-xs leading-tight text-gray-600">
        By proceeding, you agree to our
        <span class="underline">Terms of Use</span> and confirm you have read
        our <span class="underline"> Privacy and Cookie Statement</span>.
      </p>
    </div>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { formData } from '../utility/forms'
import PasswordInput from '../ui/PasswordInput.vue'
import Button from '../ui/Button.vue'
import { ArrowNarrowRightIcon } from 'vue-tabler-icons'
import api from '@/api/api'

const props = defineProps(['data'])
const formError = ref('')

const confirm = async event => {
  formError.value = ''
  const form = formData(event)
  if (props.data.role == 'Admin') {
    const [, error] = await api.auth.setAdminPassword(
      props.data.email,
      form.currentPassword,
      form.newPassword
    )
    error && (formError.value = error)
  }
}
</script>
