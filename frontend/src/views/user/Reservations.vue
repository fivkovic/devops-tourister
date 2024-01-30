<template>
  <div class="mx-auto max-w-4.5xl pb-10 h-full">
    <h1 class="text-2xl font-medium">Reservations</h1>
    <p class="text-sm text-neutral-500">View and manage all of your reservations</p>
    <div class="mt-3 flex space-x-3">
      <Dropdown
        @change="e => (currentReservationStatus = e.value)"
        label="Status"
        :values="reservationStatuses"
        class="mt-auto w-fit"
      />
    </div>
    <div v-if="reservations?.length !== 0" class="mt-4 space-y-5">
      <div
        v-for="reservation in reservations"
        :key="reservation.id"
        class="divide-y divide-neutral-300 rounded-2xl border border-neutral-300 py-3.5"
      >
        <div class="flex items-center pb-3 px-4">
          <div>
            <span class="text font-medium">{{reservationStatus(reservation)}}</span>
            <div class="flex items-center space-x-1.5 text-sm">
              <span class="text-neutral-500">from</span>
              <span class="font-medium">{{format(parse(reservation.start, 'yyyy-MM-dd', new Date()), 'MMM d, yyyy') }}</span>
              <span class="text-neutral-500">to</span>
              <span class="font-medium">{{format(parse(reservation.end,  'yyyy-MM-dd', new Date()), 'MMM d, yyyy') }}</span>
            </div>
            <div v-if="reservation.status === 'Requested' && isCustomer" class="flex items-center space-x-2 text-sm">
              <button
                  @click="deleteReservation(reservation)"
                  class="mt-2 flex items-center space-x-1 text-sm font-medium text-red-600 hover:underline"
              >
                <span>Delete Reservation</span>
                <XIcon class="h-4 w-4 stroke-2" />
              </button>
            </div>
            <div class="flex space-x-2">
              <div v-if="allowReview(reservation, 'PROPERTY')" class="flex items-center space-x-2 text-sm">
                <button
                  @click="writeReview(reservation, 'PROPERTY')"
                  class="mt-2 flex items-center space-x-1 text-sm font-medium text-emerald-700 hover:underline"
                >
                  <span>Review Property</span>
                  <PencilIcon class="h-4 w-4 stroke-2 text-emerald-700" />
                </button>
              </div>
              <div v-if="allowReview(reservation, 'HOST')" class="flex items-center space-x-2 text-sm">
                <button
                    @click="writeReview(reservation, 'HOST')"
                    class="mt-2 flex items-center space-x-1 text-sm font-medium text-emerald-700 hover:underline"
                >
                  <span>Review Host</span>
                  <PencilIcon class="h-4 w-4 stroke-2 text-emerald-700" />
                </button>
              </div>
            </div>
            <div v-if="canUpdate(reservation)" class="flex items-center space-x-2 text-sm">
              <button
                @click="update(reservation, true)"
                class="mt-2 flex items-center space-x-1 text-sm font-medium text-emerald-700 hover:underline"
              >
                <span>Accept Reservation</span>
                <CheckIcon class="h-4 w-4 stroke-2" />
              </button>
              <button
                  @click="update(reservation, false)"
                  class="mt-2 flex items-center space-x-1 text-sm font-medium text-red-600 hover:underline"
              >
                <span>Reject Reservation</span>
                <XIcon class="h-4 w-4 stroke-2" />
              </button>
            </div>
            <div v-if="reservationStatus(reservation) === 'Accepted' && isCustomer">
              <button
                @click="cancelReservation(reservation)"
                class="mt-3 flex items-center space-x-1 text-red-600 text-sm font-medium hover:underline"
              >
                <span>Cancel reservation</span>
                <XIcon class="h-3 w-3 stroke-[3px]" />
              </button>
              <p class="mt-1 text-sm text-gray-600">
                If you cancel this reservation you must pay a cancellation fee of 10%.
              </p>
            </div>
          </div>
          <div class="ml-auto text-right text-sm">
            <p>
              <span class="text-neutral-500"> Reservation date: </span>
              {{ format(parseISO(reservation.createdAt), 'MMM d, yyyy') }}
            </p>
            <p>
              <span class="text-neutral-500">Reservation ID: </span>
              <span class="" :title="reservation.id.replaceAll('-', '')">
                {{reservation.id.replaceAll('-', '').substring(0, 16) }}
              </span>
            </p>
          </div>
        </div>
        <div class="flex justify-between pt-4 px-4">
          <div class="flex space-x-4">
            <img
              :src="api.properties.image(reservation.property.images?.[0])"
              alt=""
              class="h-24 w-24 rounded-lg object-cover"
            />
            <div>
              <RouterLink :to="`/properties/${reservation.property.id}`" class="font-medium">
                {{ reservation.property.name }}
              </RouterLink>
              <h3 class="text-sm text-neutral-500">
                {{ reservation.property.location }}
              </h3>
              <p v-if="isCustomer" class="text-sm mt-3">
                Owner {{ reservation.property.owner.firstName }} {{ reservation.property.owner.lastName }}
                {{ reservation.property.owner.email}}
              </p>
              <p v-else class="text-sm mt-2">
                Customer {{ reservation.customer.firstName }} {{reservation.customer.lastName }}
              </p>
              <h3 class="text-sm">
                Reservation for {{ reservation.people }} {{ reservation.people > 1 ? 'people' : 'person'}}
              </h3>
            </div>
          </div>
          <div class="flex flex-col justify-center items-end">
            <div class="text-sm text-neutral-500">Total Cost</div>
            <div class="font-medium">
              ${{ reservation.price }}
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-else-if="!isLoading" class="-mt-[140px] flex h-full justify-center items-center text-neutral-500 text-sm">
      There are currently no reservations for the selected options
    </div>
  </div>
  <CreateReview
    @modalClosed="isReviewModalOpen = false"
    @success="reviewSuccess()"
    :isOpen="isReviewModalOpen"
    :name="name"
    :start="start"
    :end="end"
    :address="address"
    :duration="duration"
    :unit="unit"
    :id="reservationId"
    :propertyId="id"
    :type="reviewType"
    :host="reviewHost"
  />
  <CancelReservation
    @modalClosed="isCancelModalOpen = false"
    :isOpen="isCancelModalOpen"
    :reservationId="reservationId"
  />
</template>
<script setup>
import { ref, watchEffect } from 'vue'
import { RouterLink } from 'vue-router'
import {
  ChevronDownIcon,
  ChevronUpIcon,
  PencilIcon,
  XIcon,
  CheckIcon
} from 'vue-tabler-icons'
import {
  format,
  parseJSON,
  parse,
  isPast,
  differenceInHours,
  parseISO, 
  differenceInDays
} from 'date-fns'
import api from '@/api/api'
import Dropdown from '@/components/ui/Dropdown.vue'
import CreateReview from '@/components/reservations/CreateReview.vue'
import CancelReservation from '@/components/reservations/CancelReservation.vue'
import { formatAddress } from '@/components/utility/address'
import {isHost, isCustomer, user} from "@/stores/userStore";
import router from "@/router";

const reservationStatuses = [
  {
    name: 'All',
    value: null
  },
  {
    name: 'Requested',
    value: 'Requested'
  },
  {
    name: 'Accepted',
    value: 'Accepted'
  },
  {
    name: 'Rejected',
    value: 'Rejected'
  },
  {
    name: 'Cancelled',
    value: 'Cancelled'
  }
]

const reservations = ref()
const isReviewModalOpen = ref(false)
const isCancelModalOpen = ref(false)
const id = ref()
const name = ref()
const address = ref()
const start = ref()
const end = ref()
const duration = ref()
const unit = ref()
const reservationId = ref()
const currentReservationStatus = ref(reservationStatuses[0].value)
const reviewType = ref()
const reviewHost = ref()
const isLoading = ref(false)

const reservationStatus = reservation => {
  if (['Requested', 'Cancelled', 'Rejected'].includes(reservation.status)) return reservation.status
  if (isPast(parse(reservation.end, 'yyyy-MM-dd', new Date()))) return 'Completed'
  if (isPast(parse(reservation.start, 'yyyy-MM-dd', new Date()))) return 'Ongoing'
  if (reservation.status === 'Accepted') return 'Accepted'
  return 'Pending'
}

const canUpdate = reservation => {
  return reservation.status === 'Requested' && isHost.value
}

const allowReview = (reservation, type) => {
  return isCustomer.value && 
    reservationStatus(reservation) === 'Accepted' && 
    (type === 'PROPERTY' ? !reservation.reviewedProperty : !reservation.reviewedHost)
}

const fetchData = async () => {
  isLoading.value = true
  const [reservationsData, reservationsError] =
      await api.reservations.getReservations(
          0,
          "NEWEST",
          currentReservationStatus.value
      )

  if (!reservationsError) {
    reservations.value = reservationsData
  }
  isLoading.value = false
}

watchEffect(fetchData)

const reviewSuccess = () => {
  isReviewModalOpen.value = false
  fetchData()
}

const saveData = (reservation, type) => {
  const startDate = parse(reservation.start, 'yyyy-MM-dd', new Date())
  const endDate = parse(reservation.end, 'yyyy-MM-dd', new Date())

  start.value = format(startDate, 'MMM d, yyyy')
  end.value = format(endDate, 'MMM d, yyyy')

  id.value = reservation.property.id
  reservationId.value = reservation.id
  
  name.value = reservation.property.name
  address.value = reservation.property.location

  const days = differenceInDays(endDate, startDate)
  unit.value = days === 1 ? 'day' : 'days'
  duration.value = days
  reviewType.value = type
  reviewHost.value = reservation.property.owner
}

const writeReview = (reservation, type) => {
  saveData(reservation, type)
  isReviewModalOpen.value = true
}

const cancelReservation = reservation => {
  reservationId.value = reservation.id
  isCancelModalOpen.value = true
}

const update = async (reservation, accepted) => {
  const [, error] = await api.reservations.update(reservation.id, accepted)
  if (!error) {
    await fetchData()
  }
}

const deleteReservation = async reservation => {
  const [, error] = await api.reservations.deleteReservation(reservation.id)
  if (!error) {
    await fetchData()
  }
}

</script>
