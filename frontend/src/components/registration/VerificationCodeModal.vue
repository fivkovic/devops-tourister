<template>
  <Modal
    :isOpen="isModalOpen"
    @modalClosed="closeModal"
    :light="true"
    class="w-[500px] space-y-3 bg-white px-14 py-16 text-left"
  >
    <div v-if="!success">
      <h1 class="text-3xl font-medium">Almost there</h1>
      <h2 class="mt-5 text-base leading-6 text-gray-700">
        Please enter the 6 digit confirmation code we sent you to your email to
        verify your account.
      </h2>
      <form
        @submit.prevent="verifyCode"
        class="mx-auto !mt-10 flex flex-col space-y-8"
      >
        <div class="mx-auto flex space-x-2">
          <Input
            required
            @keyup="e => onKeyUp(i, e)"
            v-for="i in [1, 2, 3, 4, 5, 6]"
            :key="i"
            :id="`box${i}`"
            maxlength="1"
            :name="i"
            autocomplete="new-password"
            class="h-12 w-10 border !pr-2 text-center text-xl"
          />
        </div>
        <div class="flex justify-center">
          <Button
            type="submit"
            class="bg-emerald-600 !px-24 !py-3.5 text-lg text-white"
            >Verify account</Button
          >
        </div>
        <div class="!mt-3 text-center text-sm text-red-500">
          {{ formError }}
        </div>
      </form>
      <h3 class="!mt-10 text-center text-sm">
        Didn't get the code?
        <button @click="resendCode()" class="text-emerald-600 hover:underline">
          Resend code.
        </button>
      </h3>
      <h3 class="text-center text-sm text-emerald-600">{{ status }}</h3>
    </div>
    <div v-else>
      <h2 class="mb-10 text-center text-3xl font-medium tracking-tight">
        Successfully signed up
      </h2>
      <VerificationPanel
        success-text="Your email has been sucessfully verified! Please sign in using your email and password."
      />
      <div class="!mt-8 flex w-full justify-center">
        <RouterLink to="signin">
          <Button class="bg-emerald-600 !px-10 text-white">
            Sign in now
          </Button>
        </RouterLink>
      </div>
    </div>
  </Modal>
</template>
<script setup>
import { ref } from 'vue'
import Modal from '@/components/ui/Modal.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import VerificationPanel from './VerificationPanel.vue'
import { useModal } from '@/stores/modalStore'
import { formData } from '@/components/utility/forms'
import { useRoute, RouterLink } from 'vue-router'
import api from '@/api/api.js'
const { isModalOpen, closeModal } = useModal()

const route = useRoute()
const formError = ref('')
const status = ref('')
const success = ref(false)

const onKeyUp = (index, event) => {
  if (
    event.keyCode === 8 &&
    index > 1 &&
    document.getElementById(`box${index}`).value === ''
  ) {
    document.getElementById(`box${index - 1}`).focus()
  } else if (event.keyCode != 8 && index < 6) {
    document.getElementById(`box${index + 1}`).focus()
  }
}

const verifyCode = async event => {
  const data = formData(event)
  const code = Object.values(data).join('')
  const email = route.query.email
  const [, error] = await api.auth.confirmEmail(email, code)
  formError.value = error
  error || (success.value = true)
}

const resendCode = async () => {
  const email = route.query.email
  const [, error] = await api.auth.sendConfirmation(email)
  formError.value = error
  !error && (status.value = 'New code sent! Check your email.')
}
</script>
