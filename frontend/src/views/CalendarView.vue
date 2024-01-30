<template>
  <div class="flex flex-col-reverse xl:mx-28 2xl:mx-56">
    <Transition mode="out-in">
      <Suspense @resolve="calculateCalendarStyle">
        <div>
          <Component
            ref="calendar"
            :business-name="businessName"
            :business-id="businessId"
            business-type="property"
            :is="currentCalendar"
          />
        </div>
      </Suspense>
    </Transition>
    <header
      v-if="calendar"
      :style="calendarStyle"
      class="relative z-20 flex items-center justify-between border-b border-gray-200 py-4 px-1 lg:flex-none"
    >
      <h1 class="text-2xl text-gray-900 flex items-center space-x-2">
        <h2 class="text-xl font-medium">{{ businessName }}</h2>
        <span>-</span>
        <time class="text-base font-medium" :datetime="calendar.currentDate.toISOString().split('T')[0]">{{
          format(
            calendar.currentDate,
            calendarName == 'year' ? 'yyyy' : 'LLLL yyyy'
          )
        }}</time>
      </h1>
      <div class="flex items-center">
        <div class="flex items-center rounded-md shadow-sm md:items-stretch">
          <button
            @click="calendar.previous()"
            type="button"
            class="flex items-center justify-center rounded-l-md border border-r-0 border-gray-300 bg-white py-2 pl-3 pr-4 text-gray-400 hover:text-gray-500 focus:relative md:w-9 md:px-2 md:hover:bg-gray-50"
          >
            <span class="sr-only">Previous month</span>
            <ChevronLeftIcon class="h-5 w-5" aria-hidden="true" />
          </button>
          <button
            @click="calendar.goToToday()"
            type="button"
            class="hidden border-t border-b border-gray-300 bg-white px-3.5 text-sm font-medium text-gray-700 hover:bg-gray-50 hover:text-gray-900 focus:relative md:block"
          >
            Today
          </button>
          <span class="relative -mx-px h-5 w-px bg-gray-300 md:hidden" />
          <button
            @click="calendar.next()"
            type="button"
            class="flex items-center justify-center rounded-r-md border border-l-0 border-gray-300 bg-white py-2 pl-4 pr-3 text-gray-400 hover:text-gray-500 focus:relative md:w-9 md:px-2 md:hover:bg-gray-50"
          >
            <span class="sr-only">Next month</span>
            <ChevronRightIcon class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>
        <div class="flex md:ml-4 md:items-center">
          <Menu as="div" class="relative">
            <MenuButton
              type="button"
              class="flex items-center rounded-md border border-gray-300 bg-white py-2 pl-3 pr-2 text-sm font-medium capitalize text-gray-700 shadow-sm hover:bg-gray-50"
            >
              {{ calendarName }} view
              <ChevronDownIcon
                class="ml-2 h-5 w-5 text-gray-400"
                aria-hidden="true"
              />
            </MenuButton>

            <transition
              enter-active-class="transition ease-out duration-100"
              enter-from-class="transform opacity-0 scale-95"
              enter-to-class="transform opacity-100 scale-100"
              leave-active-class="transition ease-in duration-75"
              leave-from-class="transform opacity-100 scale-100"
              leave-to-class="transform opacity-0 scale-95"
            >
              <MenuItems
                class="absolute right-0 mt-3 w-36 origin-top-right overflow-hidden rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none"
              >
                <div class="py-1">
<!--                  <MenuItem v-slot="{ active }">-->
<!--                    <div-->
<!--                      @click="switchCalendar('week')"-->
<!--                      :class="[-->
<!--                        active ? 'bg-gray-100 text-gray-900' : 'text-gray-700',-->
<!--                        'block cursor-pointer px-4 py-2 text-sm'-->
<!--                      ]"-->
<!--                    >-->
<!--                      Week view-->
<!--                    </div>-->
<!--                  </MenuItem>-->
                  <MenuItem v-slot="{ active }">
                    <div
                      @click="switchCalendar('month')"
                      :class="[
                        active ? 'bg-gray-100 text-gray-900' : 'text-gray-700',
                        'block cursor-pointer px-4 py-2 text-sm'
                      ]"
                    >
                      Month view
                    </div>
                  </MenuItem>
<!--                  <MenuItem v-slot="{ active }">-->
<!--                    <div-->
<!--                      @click="switchCalendar('year')"-->
<!--                      :class="[-->
<!--                        active ? 'bg-gray-100 text-gray-900' : 'text-gray-700',-->
<!--                        'block cursor-pointer px-4 py-2 text-sm'-->
<!--                      ]"-->
<!--                    >-->
<!--                      Year view-->
<!--                    </div>-->
<!--                  </MenuItem>-->
                </div>
              </MenuItems>
            </transition>
          </Menu>
          <div class="ml-6 h-6 w-px bg-gray-300" />
          <button
            @click="calendar.openEventModal()"
            type="button"
            class="ml-6 rounded-md border border-transparent bg-black py-2 px-4 text-sm font-medium text-white shadow-sm hover:bg-black/80"
          >
            Add availability period
          </button>
        </div>
      </div>
    </header>
  </div>
</template>

<script setup>
import { ref, shallowRef } from 'vue'
import { useRoute } from 'vue-router'
import { format } from 'date-fns'
import {
  ChevronDownIcon,
  ChevronLeftIcon,
  ChevronRightIcon
} from 'vue-tabler-icons'
import { Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/vue'
import MonthCalendar from '@/components/business/calendar/MonthCalendar.vue'
import api from '@/api/api'

const route = useRoute()

const businessId = route.params.id
const businessName = route.query.name

const calendar = ref()
const calendarStyle = ref({})
const currentCalendar = shallowRef(MonthCalendar)
const calendarName = ref('month')

const calculateCalendarStyle = () => {
  const element = document.getElementById('current-calendar')
  if (element) {
    calendarStyle.value = {
      width: element.clientWidth + 'px'
    }
  }
}

const switchCalendar = c => {
  calendarName.value = c
  // if (c == 'week') currentCalendar.value = WeekCalendar
  // else if (c == 'month') currentCalendar.value = MonthCalendar
  // else if (c == 'year') currentCalendar.value = YearCalendar
}
</script>
