<template>
  <ErrorAlert @close="errorAlert = false" v-if="errorAlert">
    {{ reservationError }}
  </ErrorAlert>
  <div class="relative mx-auto h-screen max-w-6xl space-y-5 border-t !py-8">
    <div class="flex w-full space-x-10">
      <div class="h-full w-[62.5%] space-y-3">
        <div class="flex h-[405px] space-x-4">
          <div class="relative flex-1">
            <img
                :src="api.properties.image(props.entity?.images[0])"
                alt="Business image"
                class="h-full w-full rounded-sm object-cover"
            />
            <div
                class="absolute right-3 bottom-3 rounded-full bg-black/[0.5] py-2 px-10 text-sm text-white"
            >
              {{ props.entity.images.length === 0 ? 0 : 1 }} /
              {{ props.entity?.images?.length }}
            </div>
          </div>
          <div class="max h-full w-1/4 space-y-3.5">
            <div class="h-[31%] w-full">
              <img
                  :src="api.properties.image(props.entity?.images[1])"
                  alt=""
                  class="h-full w-full rounded-sm object-cover"
              />
            </div>
            <div class="h-[31%] w-full">
              <img
                  :src="api.properties.image(props.entity?.images[2])"
                  alt=""
                  class="h-full w-full rounded-sm object-cover"
              />
            </div>
            <div class="relative h-[31%] w-full">
              <img
                  :src="api.properties.image(props.entity?.images[3])"
                  alt=""
                  class="h-full w-full rounded-sm object-cover"
              />
              <button
                  @click="isModalOpen = true"
                  class="absolute top-0 left-0 z-10 flex h-full w-full cursor-pointer items-center justify-center space-x-1 rounded-sm bg-black/[0.5] text-sm text-white"
              >
                <PhotoIcon class="h-5 w-5 text-white"/>
                <div>{{ props.entity?.images?.length }}</div>
              </button>
            </div>
          </div>
        </div>
        <div class="relative !mt-5 h-max flex-1">
          <div class="flex w-full items-center justify-between">
            <div class="text-3xl font-medium">{{ props.entity.name }}</div>
            <div class="flex space-x-2">
              <div class="-space-y-1">
                <div class="font-medium text-lg">{{ ratingDesc }}</div>
                <div v-if="props.entity.numberOfReviews" class="text-sm text-right">
                  {{ props.entity.numberOfReviews }}
                  {{ props.entity.numberOfReviews === 1 ? 'review' : 'reviews' }}
                </div>
              </div>
              <span 
                v-if="props.entity.numberOfReviews" 
                class="flex items-center rounded-md bg-black px-2 text-xl font-bold text-white"
              >
                {{ props.entity.averageRating.toFixed(1) }}
              </span>
            </div>
          </div>
          <div class="border-b pb-4">
            <div class="flex items-center space-x-1">
              <MapPinIcon class="h-5 w-5 -mt-0.5 text-neutral-700 stroke-[2]"/>
              <h2 class="font-medium text-black">{{ props.entity.location }}</h2>
              <span class="text-[8px] text-neutral-400 px-1.5">&bullet;</span>
              <p class="text-black font-medium">
                <span class="text-[15px] text-neutral-600 font-normal">Hosted by</span> 
                {{ props.entity.owner.firstName }} {{ props.entity.owner.lastName }}
              </p>
            </div>
            <div v-if="isOwnersBusiness" class="mt-3 flex space-x-3">
              <RouterLink
                v-if="isOwnersBusiness"
                :to="`${$route.path}/calendar?name=${props.entity.name}`"
              >
                <Button class="flex space-x-1 border !py-1 !text-sm hover:bg-neutral-50">
                  <span class="font-medium">View Calendar</span>
                  <CalendarIcon class="-mt-px h-5 w-5"/>
                </Button>
              </RouterLink>
            </div>
          </div>
          <div class="!mt-5 space-y-3">
            <h2 class="text-2xl font-medium">About</h2>
            <p class="text-[15px] text-gray-600">{{ props.entity.description }}</p>
          </div>
        </div>
      </div>
      <div class="flex-1 space-y-5">
        <div
            v-if="start && end && !isOwnersBusiness"
            class="!mt-2 rounded-lg border p-5"
        >
          <h2 class="text-lg font-medium">Your reservation</h2>
          <div class="flex justify-between pt-2">
            <p class="flex items-center text-sm font-medium text-neutral-600">
              <span>{{ start }}</span>
              <span class="text-[8px] mx-1.5 text-neutral-400">&bullet;</span>
              <span>{{ end }}</span>
            </p>
            <div class="flex items-start space-x-2 text-sm font-medium text-neutral-600">
              <UsersIcon class="h-4 w-4 text-neutral-400 stroke-[2.5]"/>
              <div>{{ people }} {{ people === 1 ? 'Person' : 'People' }}</div>
            </div>
          </div>
          <div class="flex justify-between py-3">
            <p class="font-medium">Price</p>
            <p class="text-sm text-neutral-600">
              ${{ slotUnitPrice ?? props.entity.unitPrice }}
              x {{
                props.entity.pricingStrategy === 'PerNight'
                    ? `${units} ${units === 1 ? 'Night' : 'Nights'}`
                    : `${people} ${people === 1 ? 'Person' : 'People'}`
              }}
            </p>
          </div>
          <div class="flex justify-between border-t py-3">
            <p class="text-lg font-medium">Total amount</p>
            <p class="text-lg font-medium">
              ${{ totalPrice.toFixed(2) }}
            </p>
          </div>
          <div class="">
            <div v-if="!isAuthenticated" class="text-[13px] text-center text-neutral-600">
              You need to
              <RouterLink @click="signInFlow()" to="/signin" class="underline">sign in</RouterLink>
              in order to make a reservation
            </div>
            <div
                v-if="isAuthenticated && user.role !== 'Customer'"
                class="text-[13px] text-neutral-600 text-center"
            >
              You cannot make a reservation as a business owner. Please make a
              customer account.
            </div>
            <Button
                @click="makeReservation()"
                :disabled="!isCustomer || isLoading"
                class="mt-4 w-full justify-center bg-black text-white flex items-center space-x-2 text-[15px]"
            >
              <span class="pl-5">Make a reservation</span>
              <Loader class="text-white transition-all opacity-0" :class="{'opacity-100': isLoading}"/>
            </Button>
          </div>
        </div>
        <div v-if="isAuthenticated && isOwnersBusiness">
          <form ref="formRef" @submit.prevent="updateSettings" class="flex gap-x-3 items-end">
            <Dropdown
                :disabled="isLoading"
                label="Pricing model"
                :values="pricingStrats"
                :selected-value="pricingStrats.find(s => s.value === props.entity.pricingStrategy)"
                @change="updateSettings({ pricingStrategy: $event.value })"
                class="w-fit"
            />
            <NumberInput
                required
                label="Unit price"
                placeholder="0.00"
                :min="1"
                :max="1000000"
                :has-spinner="false"
                :disabled="isLoading"
                v-model="props.entity.unitPrice"
                @blur="() => formRef.requestSubmit()"
                @keydown.enter="$event.target.blur()"
                class="w-24 pl-5 !text-[15px]"
            >
              <template #prepend>
                <span class="pl-2 transition" :class="{'text-neutral-300': isLoading}">$</span>
              </template>
            </NumberInput>
          </form>
          <div class="flex items-center gap-x-4 mb-3 mt-3">
            <Checkbox
                id="auto-accept-reservations"
                label="Automatically accept reservations"
                @change="updateSettings"
                v-model="props.entity.autoAcceptReservations"
                :disabled="isLoading"
            />
            <Loader class="text-black transition-all opacity-0" :class="{'opacity-100': isLoading}"/>
          </div>
          <p class="text-sm text-gray-600">
            By enabling this option all reservations made by customers will be automatically accepted.
            Otherwise you will have to manually accept each reservation.
          </p>
        </div>
        <div
            v-if="!isAuthenticated || user.role === 'Customer'"
            class="h-32 w-full rounded-md bg-amber-50 bg-gradient-to-b from-emerald-50 px-8 py-4"
        >
          <div class="w-2/3 space-y-1">
            <p class="text-2xl font-medium">We have the best offers!</p>
            <RouterLink
                :to="'/'"
                class="flex items-center space-x-1 text-sm font-medium text-emerald-800 hover:underline"
            >
              <p>Explore more</p>
              <ArrowNarrowRightIcon class="h-4 w-4 text-emerald-800"/>
            </RouterLink>
          </div>
        </div>
      </div>
    </div>
    <div class="flex items-center space-x-2 pt-2">
      <h2 class="text-lg font-medium">
        See what guests said about the
      </h2>
      <div class="flex space-x-1 p-1 border border-neutral-300 rounded-full">
        <button 
          class="rounded-full text-sm font-medium px-3 py-1.5 transition"
          :class="{ 'bg-black text-white': reviewOption === 'property'}"
          @click="reviewOption = 'property'"
        >
          Property
        </button>
        <button 
          class="rounded-full text-sm font-medium px-5 py-1.5 transition"
          :class="{ 'bg-black text-white': reviewOption === 'host'}"
          @click="reviewOption = 'host'"
        >
          Host
        </button>
      </div>
    </div>
    <div class="overflow-x-auto">
      <ReviewsCarousel 
        :reviews="reviewOption === 'property' ? props.entity.reviews : hostReviews" 
        :reviewType="reviewOption" 
        :user-id="user.id"
        @delete-review="deleteReview"
      />
    </div>
    <div class="h-8"></div>
  </div>
  <ImagesModal
      @modalClosed="isModalOpen = false"
      :isOpen="isModalOpen"
      :images="props.entity.images"
  />
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import {
  PhotoIcon,
  MapPinIcon,
  ArrowNarrowRightIcon,
  UsersIcon,
  CalendarIcon,
} from 'vue-tabler-icons'
import Checkbox from '@/components/ui/Checkbox.vue'
import Button from '@/components/ui/Button.vue'
import ImagesModal from '@/components/business/view/ImagesModal.vue'
import { isAuthenticated, savedReservationUrl, user, isCustomer } from '@/stores/userStore.js'
import { format, parseISO, differenceInDays } from 'date-fns'
import api from '@/api/api'
import ErrorAlert from '@/components/ui/alerts/ErrorAlert.vue'
import Loader from '@/components/ui/Loader.vue'
import Dropdown from "@/components/ui/Dropdown.vue"
import NumberInput from "@/components/ui/NumberInput.vue"
import ReviewsCarousel from '@/components/business/ReviewsCarousel.vue'

const isModalOpen = ref(false)
const isLoading = ref(false)
const reservationError = ref('')
const errorAlert = ref(false)
const hostReviews = ref([])
const reviewOption = ref('property')

const props = defineProps({
  entity: {
    type: Object,
    required: true
  }
})
const formRef = ref()

const pricingStrats = [
  {value: 'PerPerson', name: 'Charge per Person'},
  {value: 'PerNight', name: 'Charge per Night'}
]

const showErrorAlert = () => {
  setTimeout(() => (errorAlert.value = true), 100)
  setTimeout(() => (errorAlert.value = false), 5000)
}

const route = useRoute()
const router = useRouter()

const updateSettings = async settings => {

  if (settings?.pricingStrategy) {
    props.entity.pricingStrategy = settings.pricingStrategy
  }

  if (!props.entity.unitPrice || props.entity.unitPrice < 1) {
    return
  }

  isLoading.value = true

  const [, error] = await api.properties.updateSettings({
    id: props.entity.id,
    pricingStrategy: props.entity.pricingStrategy,
    unitPrice: props.entity.unitPrice,
    autoAccept: props.entity.autoAcceptReservations
  })
  setTimeout(() => isLoading.value = false, 300)

  if (error) {
    reservationError.value = Object.values(error.errors)[0].join(' ')
    showErrorAlert()
  }
}

const makeReservation = async () => {

  if (isLoading.value) return

  isLoading.value = true
  const [, error] = await api.properties.makeResrvation(
      props.entity.id,
      route.query.start,
      route.query.end,
      people.value
  )

  if (error) {
    reservationError.value = error.detail
    showErrorAlert()
    isLoading.value = false
  } else {
    setTimeout(async () => {
      isLoading.value = false
      await router.push({name: 'reservations'})
    }, 1000)
  }
}

const isOwnersBusiness = user.id === props.entity.ownerId

const people = ref(Number(route.query.people))
const slotUnitPrice = ref(route.query.slotUnitPrice && Number(route.query.slotUnitPrice))

const dateFormat = 'EEE, dd MMM'

const start = route.query.start
    ? ref(format(parseISO(route.query.start), dateFormat))
    : null

const end = route.query.start
    ? ref(format(parseISO(route.query.end), dateFormat))
    : null

const units = ref(differenceInDays(parseISO(route.query.end), parseISO(route.query.start)))

const totalPrice = computed(() =>
    (slotUnitPrice.value ?? props.entity.unitPrice) *
    (props.entity.pricingStrategy === 'PerNight' ? units.value : people.value)
)

const ratingDesc = computed(() => {
  const rating = props.entity.averageRating
  if (props.entity.numberOfReviews === 0) return 'No reviews yet'
  if (rating >= 4.5) return 'Excellent'
  else if (rating >= 3.5) return 'Very good'
  else if (rating >= 2.5) return 'Good'
  else if (rating >= 1.5) return 'Okay'
  else return 'Bad'
})

const deleteReview = async (reviewId) => {
  if (reviewOption.value === 'property') {
    const [data, error] = await api.properties.deleteReview(props.entity.id, reviewId)
    if (!error) {
      props.entity.reviews = props.entity.reviews.filter(r => r.id !== reviewId)
    }
  } else if (reviewOption.value === 'host') {
    const [data, error] = await api.users.deleteReview(props.entity.ownerId, reviewId)
    if (!error) {
      hostReviews.value = hostReviews.value.filter(r => r.id !== reviewId)
    }
  }
}

const signInFlow = () => {
  savedReservationUrl.value = router.currentRoute.value.fullPath
}

const getHostReviews = async () => {
  const [data] = await api.users.getReviews(props.entity.ownerId)
  if (data) hostReviews.value = data
}

getHostReviews()
</script>