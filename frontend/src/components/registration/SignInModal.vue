<template>
  <Modal
    :isOpen="isModalOpen"
    @modalClosed="closeModal()"
    :light="true"
    class="w-[500px] bg-white px-14 py-10"
  >
    <Transition
      enter-active-class="transition-opacity duration-100"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-100"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
      mode="out-in"
    >
      <Component
        @switchAuth="e => switchAuth(e)"
        :data="data"
        :is="authPanel"
      />
    </Transition>
  </Modal>
</template>

<script setup>
import { ref, shallowRef, watch, onBeforeMount } from 'vue'
import Modal from '../ui/Modal.vue'
import SignInForm from './SignInPanel.vue'
import RegistrationForm from './RegistrationPanel.vue'
import ChangePasswordPanel from './ChangePasswordPanel.vue'

const isModalOpen = ref(false)
const closeModal = ref()
const data = ref('')

const authPanel = shallowRef(SignInForm)
const switchAuth = (d = null) => {
  if (d) {
    data.value = d
    authPanel.value = ChangePasswordPanel
  } else {
    authPanel.value =
      authPanel.value === SignInForm ? RegistrationForm : SignInForm
  }
}

onBeforeMount(async () => {
  const { useModal } = await import('@/stores/modalStore.js')
  const { isModalOpen: isOpen, closeModal: close } = useModal()
  watch(isOpen, () => (isModalOpen.value = isOpen.value), { immediate: true })
  closeModal.value = close
})
</script>
