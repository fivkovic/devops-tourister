import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import SignInModal from '../components/registration/SignInModal.vue'

const modalRouteFunc = (to, from, component) => {
  const fromMatch = from.matched[0]
  const toMatch = to.matched[0]
  toMatch.components['modal-router'] = component
  fromMatch && (toMatch.components.default = fromMatch.components.default)
  !fromMatch &&
    (toMatch.components.default = () => import('../views/HomeView.vue'))
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/property/:action',
      name: 'create-update-property',
      component: () => import('../views/CreateUpdateBusinessView.vue')
    },
    {
      path: '/results',
      name: 'results',
      component: () => import('../views/ResultsView.vue')
    },
    {
      path: '/signin',
      name: 'signin',
      beforeEnter: (to, from) => modalRouteFunc(to, from, SignInModal)
    },
    {
      path: '/verification',
      name: 'verification',
      props: true,
      beforeEnter: (to, from) =>
        modalRouteFunc(to, from, () =>
          import('../components/registration/VerificationCodeModal.vue')
        )
    },
    {
      path: '/properties',
      name: 'properties',
      component: () => import('../views/OwnersBusinesses.vue')
    },
    {
      path: '/properties/:id',
      name: 'property',
      component: () => import('../views/business/PropertyProfileView.vue')
    },
    {
      path: '/properties/:id/calendar',
      name: 'property-calendar',
      component: () => import('../views/CalendarView.vue')
    },
    {
      path: '/properties/search',
      name: 'search',
      component: () => import('../views/ResultsView.vue')
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('../views/user/ProfileView.vue')
    },
    {
      path: '/reservations',
      name: 'reservations',
      component: () => import('../views/user/Reservations.vue')
    },
  ]
})

const modalNames = [
  'signin',
  'admin-signup',
  'verification',
  'admin-verification'
]
const nonSearchRoutes = ['profile', 'reservations']

export { modalNames, nonSearchRoutes }
export default router
