import { instance, fetch } from './http.js'
import { user } from "@/stores/userStore"

const getProfile = () => fetch(instance.get(`users/${user.id}`))
const getUser = id => fetch(instance.get(`users/${id}`))
const update = userData => fetch(instance.put(`users/${userData.id}`, userData))
const getReviews = id => fetch(instance.get(`users/${id}/reviews`))
const deleteProfile = reason => fetch(instance.delete('users/'))
const deleteReview = (ownerId, reviewId) => fetch(instance.delete(`users/${ownerId}/reviews/${reviewId}`))

export default {
  update,
  deleteProfile,
  getProfile,
  getUser,
  getReviews,
  deleteReview
}
