<template>
  <div class="-mt-20 h-full basis-[11.2rem] space-y-3">
    <p class="text-lg font-medium">Filter by</p>
    <div id="rating" class="space-y-1.5 py-3">
      <p class="font-medium">Rating</p>
      <CheckboxGroup
        :key="ratingHigher"
        @change="setRating"
        :items="ratingFilters"
        :selectedValue="
          ratingFilters.find(filter => filter.value == ratingHigher)
        "
      ></CheckboxGroup>
    </div>
    <div
      id="rooms"
      class="space-y-1.5 py-3"
    >
      <p class="font-medium">Room number</p>
      <CheckboxGroup
        :key="rooms"
        @change="setRoomNumber"
        :items="roomFilters"
        :selectedValue="roomFilters.find(filter => filter.value == rooms)"
      ></CheckboxGroup>
    </div>
    <div class="space-y-1.5 py-3">
      <p class="font-medium">Price (per person)</p>
      <CheckboxGroup
        :key="priceLow"
        @change="setPrice"
        :items="priceFilters"
        :selectedValue="
          priceFilters.find(
            filter =>
              filter.value.priceLow == priceLow &&
              filter.value.priceHigh == priceHigh
          )
        "
      ></CheckboxGroup>
    </div>
    <Button @click="resetFilters()" class="!px-0 font-normal"
      >Reset filters</Button
    >
  </div>
</template>
<script setup>
import { ref, watch } from 'vue'
import CheckboxGroup from '../ui/CheckboxGroup.vue'
import Button from '../ui/Button.vue'
import { useRoute, useRouter } from 'vue-router'

const props = defineProps({ businessType: { required: false, type: String } })
const emit = defineEmits(['filter'])
const route = useRoute()
const router = useRouter()

const ratingHigher = ref(route.query.ratingHigher)
const rooms = ref(route.query.rooms)
const priceLow = ref(route.query.priceLow)
const priceHigh = ref(route.query.priceHigh)
const seats = ref(route.query.seats)

const ratingFilters = [
  { label: 'Excellent: 4+', id: 'rating5', value: 4 },
  { label: 'Very good: 3+', id: 'rating4', value: 3 },
  { label: 'Good: 2+', id: 'rating3', value: 2 },
  { label: 'Not bad 1+', id: 'rating2', value: 1 }
]
const roomFilters = [
  { label: '1 room', id: 'room1', value: 1 },
  {
    label: '2 rooms',
    id: 'room2',
    value: 2
  },
  {
    label: '3 rooms',
    id: 'room3',
    value: 3
  },
  {
    label: 'more',
    id: 'room_more',
    value: 4
  }
]
const seatFilters = [
  { label: '1 seat', id: 'seat1', value: 1 },
  {
    label: '2 seats',
    id: 'seat2',
    value: 2
  },
  {
    label: '3 seats',
    id: 'seat3',
    value: 3
  },
  {
    label: '4 seats',
    id: 'seat4',
    value: 4
  },
  {
    label: 'more',
    id: 'seat_more',
    value: 5
  }
]
const priceFilters = [
  {
    label: '$0-50 /unit',
    value: { priceLow: 0, priceHigh: 50 },
    id: 'price1'
  },
  {
    label: '$50-100 /unit',
    value: { priceLow: 50, priceHigh: 100 },
    id: 'price2'
  },
  {
    label: '$100-200 /unit',
    value: { priceLow: 100, priceHigh: 200 },
    id: 'price3'
  },
  { label: 'more', value: { priceLow: 200 }, id: 'price4' }
]

const setRating = newRating => (ratingHigher.value = newRating)
const setRoomNumber = newRoomNumber => (rooms.value = newRoomNumber)
const setPrice = newPrice => {
  priceLow.value = newPrice.priceLow
  priceHigh.value = newPrice.priceHigh
}
const setSeats = newSeatNumber => (seats.value = newSeatNumber)
watch([ratingHigher, rooms, priceLow, priceHigh, seats], () => {
  const query = {
    ...route.query,
    ratingHigher: ratingHigher.value,
    rooms: rooms.value,
    priceLow: priceLow.value,
    priceHigh: priceHigh.value,
    seats: seats.value
  }
  let filtered = Object.fromEntries(
    // eslint-disable-next-line no-unused-vars
    Object.entries(query).filter(([_, v]) => v != '')
  )
  router.push({
    name: 'search',
    query: {
      ...filtered
    },
    params: { type: props.businessType }
  })
  setTimeout(() => emit('filter', filtered), 0)
})

const resetFilters = () => {
  ratingHigher.value = ''
  rooms.value = ''
  priceLow.value = ''
  priceHigh.value = ''
  seats.value = ''
}
</script>
