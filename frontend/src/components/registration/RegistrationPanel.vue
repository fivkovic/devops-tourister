<template>
  <form @submit.prevent class="relative text-left">
    <h1 class="mt-10 text-2xl font-medium leading-7">
      {{ title }}
    </h1>
    <Transition mode="out-in">
      <div v-if="showPrevious" class="mt-8">
        <button @click="back()" class="group flex space-x-2">
          <ArrowNarrowLeftIcon
            stroke-width="1.25"
            class="group-hover:stroke-2"
          />
          <div class="group-hover:font-semibold">Previous</div>
        </button>
      </div>
    </Transition>
    <Transition mode="out-in">
      <KeepAlive>
        <Component
          :is="panels[selectedPanel]"
          @selectedRole="changeRole"
          @next="forward"
          @register="register"
          success-text="Your account has sucessfully been registered! Please sign in using your email and password."
          class="mt-10"
        />
      </KeepAlive>
    </Transition>
    <div class="absolute w-full flex justify-center top-[18.5rem] mt-5 text-sm text-red-500">
      {{ formError }}
    </div>
    <div class="flex justify-end"></div>
    <Divider class="mt-10">Already a member?</Divider>
    <p class="mt-4 text-center font-thin">
      <button @click="emit('switchAuth')" class="font-medium underline">
        Sign in
      </button>
      using your account.
    </p>
    <p class="mt-5 text-center text-xs leading-tight text-gray-600">
      By proceeding, you agree to our
      <span class="underline">Terms of Use</span> and confirm you have read our
      <span class="underline"> Privacy and Cookie Statement</span>.
    </p>
  </form>
</template>

<script setup>
import { ref, shallowRef, computed } from 'vue'
import RolePanel from './RolePanel.vue'
import UserInformationForm from './UserInformationForm.vue'
import AccountInformationForm from './AccountInformationForm.vue'
import VerificationPanel from './VerificationPanel.vue'
import Divider from '@/components/ui/Divider.vue'
import { ArrowNarrowLeftIcon } from 'vue-tabler-icons'
import api from '@/api/api.js'

const emit = defineEmits(['switchAuth'])
const selectedPanel = ref(0)
const selectedRole = ref('')
let data = {}
const formError = ref('')

const panels = shallowRef([
  RolePanel,
  UserInformationForm,
  AccountInformationForm,
  VerificationPanel
])
const title = computed(() =>
  selectedPanel.value < panels.value.length - 1 || selectedPanel.value === 0
    ? 'Join to unlock the best of tourister.com'
    : 'Registration successful!'
)

const changeRole = role => selectedRole.value = role

const showPrevious = computed(
  () => selectedPanel.value > 0 && selectedPanel.value < panels.value.length - 1
)

const back = () => {
  selectedPanel.value--
}

const forward = newData => {
  data = { ...data, ...newData }
  selectedPanel.value++
}

const register = async newData => {
  data = { ...data, ...newData }
  data.role = selectedRole.value.charAt(0).toUpperCase() + selectedRole.value.slice(1)
  
  formError.value = ''
  const [, error] = await api.auth.register(data)
  if (error) formError.value = error.detail
  
  if (!error) {
    forward()
  }
}
</script>

<style scoped>
.v-enter-active,
.v-leave-active {
  transition: opacity 0.2s ease-in-out;
}

.v-enter-from,
.v-leave-to {
  opacity: 0;
}
</style>
