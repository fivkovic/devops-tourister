<template>
  <form @submit.prevent="signIn()" class="relative text-left">
    <h1 class="mt-4 text-3xl font-medium tracking-tight">Welcome back.</h1>
    <div class="mt-10">
      <Input
        required
        autocomplete="email"
        placeholder="Email"
        class="h-12 w-full"
        v-model="email"
        type="email"
        label="Email address"
      />
    </div>
    <div class="mt-3">
      <PasswordInput
        required
        autocomplete="current-password"
        placeholder="Password"
        class="h-12 w-full"
        v-model="password"
        label="Password"
      />
    </div>
    <div class="mt-5 text-sm text-red-500">{{ formError }}</div>
    <div class="mt-3">
      <a href="" class="text-sm underline">Forgot password?</a>
    </div>
    <div class="mx-auto w-3/4">
      <Button class="mt-8 w-full !rounded-full bg-black !py-4 text-base text-white">
        Sign in
      </Button>
    </div>
    <Divider class="mt-8">Not a member?</Divider>
    <p class="mt-4 text-center font-thin">
      <button
        type="button"
        @click="emit('switchAuth')"
        class="font-medium underline"
      >
        Join
      </button>
      to unlock the best of tourister.com
    </p>
    <p class="mt-5 text-center text-xs leading-tight text-gray-600">
      By proceeding, you agree to our
      <span class="underline">Terms of Use</span> and confirm you have read our
      <span class="underline"> Privacy and Cookie Statement</span>.
    </p>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import Input from '@/components/ui/Input.vue'
import PasswordInput from '@/components/ui/PasswordInput.vue'
import Button from '@/components/ui/Button.vue'
import Divider from '@/components/ui/Divider.vue'
import api from '@/api/api.js'
const emit = defineEmits(['switchAuth'])
const email = ref('')
const password = ref('')
const formError = ref('')

const signIn = async () => {
  const formData = {
    email: email.value,
    password: password.value
  }
  const [, error] = await api.auth.signIn(formData)
  if (error) {
    formError.value = error.detail
  }
}
</script>
