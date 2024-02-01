import axios from 'axios'
import router from '@/router/index.js'
import { setUser } from '../stores/userStore'

const baseURL = import.meta.env.DEV
  ? `http://${window.location.hostname}:8000/`
  : `http://${window.location.hostname}:8000/` // : 'https://api.tourister.com/'
  

const instance = axios.create({
  baseURL,
  withCredentials: true
})

const getAccessToken = () => localStorage.getItem('access-token')
const setAccesToken = token => localStorage.setItem('access-token', token)
const removeAccesToken = () => localStorage.removeItem('access-token')

instance.interceptors.request.use(request => {
  const accesToken = getAccessToken()
  if (accesToken != null) {
    request.headers['Authorization'] = 'Bearer ' + accesToken
  }
  return request
})

instance.interceptors.response.use(
  response => response,
  async error => {
    if (error.response.status == 403) {
      setUser({})
      router.push('/')
      return Promise.reject(error)
    }

    const originalRequest = error.config

    if (error.response.status !== 401 || originalRequest.isRetryAttempt) {
      return Promise.reject(error)
    }

    originalRequest.isRetryAttempt = true
    try {
      const response = await instance.get('/auth/token-refresh')
      originalRequest.headers['Authorization'] =
        'Bearer ' + response.data.accessToken
      setAccesToken(response.data.accessToken)
      return instance(originalRequest)
    } catch (error) {
      setUser({})
      router.push('/')
      return Promise.reject(error)
    }
  }
)

const fetch = async (request, onSuccess, onError) => {
  try {
    const response = await request
    onSuccess && onSuccess(response.data)
    return [response.data, null]
  } catch (error) {
    onError && onError(error.response.data)
    return [null, error.response.data]
  }
}

export {
  instance,
  fetch,
  setAccesToken,
  getAccessToken,
  removeAccesToken
}
