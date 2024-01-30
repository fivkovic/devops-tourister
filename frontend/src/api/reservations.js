import { instance, fetch } from './http.js'
import { isAuthenticated, notifications } from '@/stores/userStore.js'

function getReservations (page, sort, status) {
  return fetch(instance.get('/reservations', {
    params: {
      page,
      sort,
      status
    }
  }))
}

function cancel(id) {
  return fetch(instance.post(`/reservations/${id}/cancel`))
}

function update(id, accepted) {
  return fetch(instance.post(`/reservations/${id}/update`, {}, {
    params: {
      accepted
    }
  }))
}

function deleteReservation(id) {
  return fetch(instance.delete(`/reservations/${id}`))
}

function getNotifications() {
  return fetch(instance.get('/reservations/notifications'))
}

function deleteNotification(id) {
  return fetch(instance.delete(`/reservations/notifications/${id}`))
}

function getSubscriptions() {
  return fetch(instance.get('/reservations/notifications/settings'))
}

function updateSubscriptions(subscription) {
  return fetch(instance.post('/reservations/notifications/settings', {}, {
    params: { subscription }
  }))
}

const review = async (id, content, rating, type = "PROPERTY" /* or "HOST" */) =>
  fetch(
    instance.post(`/reservations/${id}/review`, {}, {
      params: {
        content,
        rating,
        type
      }
    })
  )

const syncNotifications = () => {
  if (!isAuthenticated.value) return
  getNotifications().then(([data, error]) => {
    if (!error) notifications.value = data.reverse()
  })
}

setInterval(syncNotifications, 10000)
setTimeout(syncNotifications, 500)

export default { 
  getReservations,
  cancel,
  update,
  deleteReservation,
  review,
  getNotifications,
  deleteNotification,
  getSubscriptions,
  updateSubscriptions
}