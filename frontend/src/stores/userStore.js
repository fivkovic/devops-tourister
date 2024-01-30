import { computed, ref, reactive } from 'vue'
import api from '@/api/api.js'

const authenticatedUser = JSON.parse(localStorage.getItem('authenticated-user'))
let user = reactive({
  ...authenticatedUser
})

const setUser = newUser => {
  Object.assign(user, newUser)
  localStorage.setItem('authenticated-user', JSON.stringify(newUser))
}

const removeUser = () => {
  user = reactive({})
  localStorage.removeItem('authenticated-user')
  savedReservationUrl.value = null
  notifications.value = []
}

const isAuthenticated = computed(() => Object.keys(user).length !== 0)

const isCustomer = computed(() => isAuthenticated.value && user?.role === 'Customer')

const isHost = computed(() => isAuthenticated.value && user?.role === 'Host')

const savedReservationUrl = ref()

const notifications = ref([])
const unreadNotifications = computed(() => notifications.value.length)

export {
  user,
  setUser,
  removeUser,
  isAuthenticated,
  isCustomer,
  isHost,
  savedReservationUrl,
  unreadNotifications,
  notifications
}
