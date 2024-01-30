import { instance, fetch } from './http.js'
import defaultImage from '@/assets/images/default.png'

const get = async (id, query) => fetch(instance.get(`properties/${id}`))

const createOrUpdate = async (data, action) => {
  
  if (action === 'update') {
    console.log('NOT IMPLEMENTED')
    return
  }
  
  const formData = new FormData()
  data.images.forEach(i => formData.append('files', i))
  
  delete data.images
  
  const [property, error] = await fetch(instance.post('properties?', formData, { params: data }))
  return [property, error]
}

const create = async (data) => createOrUpdate(data, 'create')
const update = async (data) => createOrUpdate(data, 'update')

const search = async (query, page) =>
  fetch(instance.get('/properties/search', { params: { ...query, page } }))

const getProperties = () => fetch(instance.get('/properties'))

const getCalendar = async (id, start, end) =>
  fetch(instance.get(`properties/${id}/availability`, {
      params: {
        start,
        end
      }
    })
  )

const updateSettings = async params => fetch(
  instance.post(`properties/${params.id}/settings`, {}, { params: params  })
)

const createAvailability = async (id, start, end, customPrice) =>
  fetch(instance.post(`properties/${id}/availability`, { start, end, customPrice }))

const deleteAvailability = async (id, eventId) =>
  fetch(instance.delete(`properties/${id}/availability/${eventId}`))

const makeResrvation = async (id, start, end, people) =>
  fetch(
    instance.post(`properties/${id}/reservations`, {
      people,
      start,
      end
    })
  )

const image = id => {
  if (!id) return defaultImage
  return instance.defaults.baseURL + 'images/p/' + id
}

const deleteReview = async (propertyId, reviewId) => {
  return fetch(instance.delete(`properties/${propertyId}/reviews/${reviewId}`))
}

export default {
  get,
  create,
  update,
  search,
  getCalendar,
  createAvailability,
  deleteAvailability,
  makeResrvation,
  getProperties,
  image,
  updateSettings,
  deleteReview
}
