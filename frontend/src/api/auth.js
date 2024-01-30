import { setUser, removeUser, savedReservationUrl } from '@/stores/userStore'
import { instance, fetch, setAccesToken, removeAccesToken } from './http.js'
import router from '@/router'

const setAuthData = async data => {
  setAccesToken(data.token)
  setUser(data.user)
  if (savedReservationUrl.value) {
    await router.push(savedReservationUrl.value)
  }
  else {
    window.location.href = '/'
  }
}

const signIn = body => {
  return fetch(instance.post('authentication/signin', body), data =>
    setAuthData(data)
  )
}

const signOut = async () => {
  removeUser()
  removeAccesToken()
  window.location.href = '/'
}

const register = data => {
  return fetch(instance.post('authentication/signup', data))
}

const changePassword = (password, newPassword) =>
  fetch(
    instance.post('authentication/change-password', {
      password,
      newPassword
    }),
    () => {
      removeAccesToken()
      setUser({})
    }
  )

export default {
  signIn,
  signOut,
  register,
  changePassword
}
