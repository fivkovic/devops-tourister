import { ref, watch } from 'vue'
import router, { modalNames } from '../router/index.js'

const isModalOpen = ref(false)
export const useModal = () => {
  const openModal = () => {
    isModalOpen.value = true
  }
  const closeModal = () => {
    isModalOpen.value = false
    setTimeout(() => {
      if (defaultRoute.value == undefined) router.push('/')
      else router.back()
    }, 100) // transition duration
  }

  return { isModalOpen, openModal, closeModal }
}

const defaultRoute = ref('')

watch(
  () => router.currentRoute.value,
  (route, previousRoute) => {
    if (modalNames.includes(route.name)) {
      isModalOpen.value = true
      defaultRoute.value = previousRoute?.name ?? '/'
    }
  },
  { immediate: true }
)
