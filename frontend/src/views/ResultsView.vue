<template>
  <div class="flex h-full flex-col space-y-10 px-10 py-3">
    <form
      @submit.prevent="search()"
      class="mx-auto ml-60 flex max-w-6xl justify-center space-x-3"
    >
      <div class="w-64">
        <Input
          id="location-input"
          required
          v-model="newLocation"
          placeholder="Enter a destination"
          clearable
          class="h-12 w-64 pl-8"
        >
          <template #prepend="{ focused, hovered }">
            <MapPinIcon
              class="ml-2.5 mr-1.5 h-[18px] w-[18px] transition-all"
              :class="[
                focused || hovered ? 'text-neutral-700' : 'text-neutral-400'
              ]"
            />
          </template>
        </Input>
      </div>
      <div class="w-[14.4rem]">
        <DateInput
          required
          @change="value => (start = value)"
          id="start"
          type="datetime-local"
          v-model="start"
          placeholder="Start of the journey"
          clearable
          class="h-12 w-full pl-8 text-sm"
        >
        </DateInput>
      </div>
      <div class="w-[14.4rem]">
        <DateInput
          required
          @change="value => (end = value)"
          type="datetime-local"
          v-model="end"
          placeholder="End of the journey"
          :lowerLimit="start"
          clearable
          class="h-12 w-full pl-8 text-sm"
        >
        </DateInput>
      </div>
      <NumberInput
        required
        v-model="people"
        min="1"
        clearable
        class="h-12 w-28 pl-10"
      >
        <template #prepend="{ focused, hovered }">
          <UsersIcon
            class="ml-2.5 mr-1.5 h-[18px] w-[18px] transition-all"
            :class="[
              focused || hovered ? 'text-neutral-700' : 'text-neutral-400'
            ]"
          />
        </template>
      </NumberInput>
      <Button
        type="submit"
        class="!ml-5 bg-black !px-3  text-white rounded-full"
      >
        <SearchIcon/>
      </Button>
    </form>
    <div class="flex space-x-10 px-5">
      <FilterPanel
        @filter="f => filter(f)"
      />
      <div v-if="isLoading">
        Loading...
      </div>
      <div v-else>
        <div class="justify mb-6 flex items-end justify-between">
          <h2 class="text-xl font-medium">
            {{ results.length }}
            {{ results.length === 1 ? 'Result' : 'Results' }}
            <span class="font-normal"> for {{ currentLocation }}</span>
            <div v-if="results.length === 0" class="mt-3 text-base font-normal">
              Unfortunately, there are no available properties. Please change the search parameters.
            </div>
          </h2>
          <Dropdown
            v-if="results.length !== 0"
            @change="changeSelectedOption"
            :slim="true"
            :selectedValue="direction"
            class="-mb-2.5"
            :values="sortingOptions"
          />
        </div>
        <div class="grid h-3/4 auto-rows-fr grid-cols-4 gap-8">
          <div
            v-for="result in results"
            :key="result.id"
            class="group max-w-[12rem] cursor-pointer space-y-3.5"
          >
            <RouterLink
              :to="{
                name: 'property',
                params: { id: result.property.id },
                query: { start, end, people, ...(result.slotUnitPrice && { slotUnitPrice: result.slotUnitPrice })  }
              }"
            >
              <ResultPreviewItem :result="result.property" :hasDetails="true" />
            </RouterLink>
          </div>
        </div>
        <div v-if="results.length" class="mt-10 flex justify-between space-x-10 text-sm">
          <div class="flex space-x-5">
            <button
              @click="previousPage()"
              v-if="hasPrevious"
              class="flex items-center space-x-2 hover:underline"
            >
              <ArrowLeftIcon class="h-4 w-4" />
              <p>Previous</p>
            </button>
            <p>
              Page <span class="font-medium">{{ currentPage }}</span> of
              <span class="font-medium">{{ totalPages }} </span>
            </p>
            <button
              @click="nextPage()"
              v-if="hasNext"
              class="flex items-center space-x-2 hover:underline"
            >
              <p>Next</p>
              <ArrowRightIcon class="h-4 w-4" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import {ref, onMounted, computed, watch} from 'vue'
import api from '../api/api'
import Dropdown from '@/components/ui/Dropdown.vue'
import Input from '../components/ui/Input.vue'
import DateInput from '../components/ui/DateInput.vue'
import NumberInput from '../components/ui/NumberInput.vue'
import Button from '../components/ui/Button.vue'
import FilterPanel from '../components/search/FilterPanel.vue'
import {
  MapPinIcon,
  UsersIcon,
  ArrowLeftIcon,
  ArrowRightIcon,
  SearchIcon
} from 'vue-tabler-icons'

import { useRoute, useRouter, RouterLink } from 'vue-router'
import { parseISO, formatISO } from 'date-fns'
import ResultPreviewItem from "@/components/search/ResultPreviewItem.vue";

const route = useRoute()
const router = useRouter()

const sortingOptions = [
  {
    name: 'Price - Highest first',
    value: 'price_desc'
  },
  {
    name: 'Price - Lowest first',
    value: 'price_asc'
  },
  {
    name: 'Rating - Highest first',
    value: 'rating_desc'
  }
]

const currentPage = ref(route.query?.page ?? 1)
const totalPages = ref(1)
const totalResults = ref(0)
const resultsFrom = computed(() => 1 + 6 * (currentPage.value - 1))
const resultsTo = computed(
  () => 6 * (currentPage.value - 1) + results.value.length
)
const hasNext = computed(() => currentPage.value < totalPages.value)
const hasPrevious = computed(() => currentPage.value > 1)
const filters = ref({})

const currentLocation = ref(route.query.location)
const newLocation = ref(currentLocation.value)

watch(() => route.query.location, location => currentLocation.value = location)

const start = ref(route.query.start)
const end = ref(route.query.end)
const people = ref(Number(route.query.people))
const direction = ref(
  sortingOptions.find(option => option.value === route.query.direction) ??
    sortingOptions[0]
)

const isLoading = ref(true)
const results = ref([])

const fetchResults = async () => {
  if (Object.keys(route.query).length < 4) return
  const queryData = { ...route.query }
  isLoading.value = true
  const [data] = await api.properties.search(
    { ...queryData },
    route.query.page - 1 || 0
  )
  isLoading.value = false
  if (data) {
    data.forEach(result => result.property.totalPrice = result.totalPrice)
    results.value = data
    // totalResults.value = data.totalResults
    // totalPages.value = Math.ceil(totalResults.value / 6)
  }
}

const changeSelectedOption = value => {
  direction.value = value
  search()
}

const filter = filtered => (filters.value = filtered)

const search = () => {
  router.push({
    name: 'search',
    query: {
      location: newLocation.value,
      start: start.value,
      end: end.value,
      people: people.value,
      direction: direction.value.value,
      page: currentPage.value,
      ...filters.value
    }
  })
  setTimeout(() => {
    fetchResults()
  }, 100)
}

fetchResults()

const nextPage = () => {
  currentPage.value++
  search()
}

const previousPage = () => {
  currentPage.value--
  search()
}
</script>
