<template>
  <div class="mb-10 flex space-x-8">
    <div id="current-calendar" class="flex-1 lg:flex lg:h-full lg:flex-col">
      <div
        class="shadow ring-1 ring-black ring-opacity-5 lg:flex lg:flex-auto lg:flex-col"
      >
        <div
          class="grid grid-cols-7 gap-px border-b border-gray-300 bg-gray-200 text-center text-xs font-semibold leading-6 text-gray-700 lg:flex-none"
        >
          <div class="bg-white py-2">
            M<span class="sr-only sm:not-sr-only">on</span>
          </div>
          <div class="bg-white py-2">
            T<span class="sr-only sm:not-sr-only">ue</span>
          </div>
          <div class="bg-white py-2">
            W<span class="sr-only sm:not-sr-only">ed</span>
          </div>
          <div class="bg-white py-2">
            T<span class="sr-only sm:not-sr-only">hu</span>
          </div>
          <div class="bg-white py-2">
            F<span class="sr-only sm:not-sr-only">ri</span>
          </div>
          <div class="bg-white py-2">
            S<span class="sr-only sm:not-sr-only">at</span>
          </div>
          <div class="bg-white py-2">
            S<span class="sr-only sm:not-sr-only">un</span>
          </div>
        </div>
        <div
          class="flex bg-gray-200 text-xs leading-6 text-gray-700 lg:flex-auto"
        >
          <div class="isolate grid w-full grid-cols-7 grid-rows-6 gap-px">
            <button
              v-for="(day, date) in days"
              :key="date"
              @click="selectDay(day.date)"
              type="button"
              :class="[
                day.isCurrentMonth ? 'bg-white' : 'bg-gray-50',
                (selectedDay === day.date || day.isToday) && 'font-semibold',
                selectedDay === day.date && 'text-white',
                selectedDay !== day.date && day.isToday && 'text-emerald-600',
                !selectedDay === day.date &&
                  day.isCurrentMonth &&
                  !day.isToday &&
                  'text-gray-900',
                selectedDay !== day.date &&
                  !day.isCurrentMonth &&
                  !day.isToday &&
                  'text-gray-500',
                'flex h-20 flex-col py-2 px-3 hover:bg-gray-100 focus:z-10'
              ]"
            >
              <time
                :datetime="date"
                :class="[
                  selectedDay.toISOString().split('T')[0] ===
                    day.date.toISOString().split('T')[0] &&
                    'flex h-6 w-6 items-center justify-center rounded-full',
                  selectedDay.toISOString().split('T')[0] ===
                    day.date.toISOString().split('T')[0] &&
                    day.isToday &&
                    'bg-black text-white',
                  selectedDay === day.date && !day.isToday && 'bg-gray-900',
                  'ml-auto'
                ]"
              >
                {{ date.split('-').pop().replace(/^0/, '') }}
              </time>
              <p class="sr-only">{{ day.events.length }} events</p>
              <div
                v-if="day.events.length > 0"
                class="-mx-0.5 mt-auto flex flex-wrap-reverse space-x-1"
              >
                <div
                  v-for="event in day.events"
                  :key="event.id"
                  :class="{
                    'bg-emerald-600': event.type === 'availability',
                    
                    'bg-emerald-700 ring-2 ring-emerald-700 ring-offset-2': 
                      event.type === 'availability' && selectedEvent?.id === event.id,
                      
                    'bg-indigo-600': event.type === 'unavailable',
                    
                    'bg-indigo-700 ring-2 ring-indigo-700 ring-offset-2':
                      event.type === 'unavailable' && selectedEvent?.id === event.id,
                      
                    '!bg-neutral-300 !text-black': isPast(
                      event.chunked ? event.originalEvent.end : event.end
                    ),
                    
                    '!bg-neutral-400 ring-2 !ring-neutral-400 ring-offset-2':
                      isPast(
                        event.chunked ? event.originalEvent.end : event.end
                      ) && selectedEvent?.id === event.id
                  }"
                  class="mx-0.5 mb-1 h-1.5 w-1.5 rounded-full"
                />
              </div>
            </button>
          </div>
        </div>
      </div>
    </div>
    <div class="basis-80">
      <EventsSidebar
        :selected-event="selectedEvent"
        :selected-day="selectedDay"
        :selected-day-events="selectedDayEvents"
        @event-selected="e => (selectedEvent = e)"
        @delete-event="e => deleteEvent(e)"
        @event-reported="eventReported()"
      />
    </div>
  </div>
  <CreateEventModal
    :isOpen="eventModalOpen"
    @modalClosed="eventModalOpen = false"
    @success="renderCalendar()"
  />
</template>

<script setup>
import { ref } from 'vue'
import { format, parseISO, sub, add, isSameDay, isPast } from 'date-fns'
import { useEvents } from '@/components/utility/events.js'

import EventsSidebar from './EventsSidebar.vue'
import CreateEventModal from '@/components/business/calendar/CreateEventModal.vue'

import api from '@/api/api.js'

defineEmits(['switchCalendar'])
const props = defineProps(['businessId', 'businessType', 'businessName'])

const eventReported = () => {
  if (selectedEvent.value.chunked) {
    selectedEvent.value.originalEvent.reported = true
  } else {
    selectedEvent.value.reported = true
  }
}

const today = new Date()
const currentDate = ref(new Date())
const startDate = ref()
const endDate = ref()
const eventModalOpen = ref(false)

const {
  days,
  selectedDay,
  selectedDayEvents,
  selectedEvent,
  selectDay,
  chunkEvents,
  deleteEvent,
  transformEvent
} = useEvents(props.businessId, props.businessType)

const goToToday = async () => {
  currentDate.value = today
  selectedDay.value = today
  await renderCalendar()
  selectDay(today)
}

const previousMonth = async () => {
  currentDate.value = sub(currentDate.value, {
    days: currentDate.value.getDate() + 1
  })
  await renderCalendar()
}

const nextMonth = async () => {
  currentDate.value = add(currentDate.value, {
    days: new Date(
      currentDate.value.getFullYear(),
      currentDate.value.getMonth(),
      0
    ).getDate()
  })
  await renderCalendar()
}

const renderCalendar = async () => {
  const currentMonthLastDay = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() + 1,
    0
  ).getDate()

  const previousMonthLastDay = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth(),
    0
  ).getDate()

  const currentMonthFirstDayIndex =
    (new Date(
      currentDate.value.getFullYear(),
      currentDate.value.getMonth(),
      1
    ).getDay() +
      6) %
    7

  const nextMonthDays = 42 - currentMonthFirstDayIndex - currentMonthLastDay

  startDate.value = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() - 1,
    previousMonthLastDay - currentMonthFirstDayIndex + 1
  )

  endDate.value = new Date(
    currentDate.value.getFullYear(),
    currentDate.value.getMonth() + 3,
    nextMonthDays
  )

  const [data, error] = await api.properties.getCalendar(
    props.businessId,
    startDate.value.toISOString().split('T')[0],
    endDate.value.toISOString().split('T')[0]
  )

  days.value = {}

  for (let i = currentMonthFirstDayIndex; i > 0; i--) {
    const date = new Date(
      currentDate.value.getFullYear(),
      currentDate.value.getMonth() - 1,
      previousMonthLastDay - i + 2
    )

    days.value[date.toISOString().split('T')[0]] = {
      date: sub(date, { hours: 2 }),
      events: []
    }
  }

  for (let i = 1; i <= currentMonthLastDay; i++) {
    const date = new Date(
      currentDate.value.getFullYear(),
      currentDate.value.getMonth(),
      i + 1
    )
    days.value[date.toISOString().split('T')[0]] = {
      date: sub(date, { hours: 2 }),
      isCurrentMonth: true,
      isToday:
        today.toDateString() ===
        new Date(
          currentDate.value.getFullYear(),
          currentDate.value.getMonth(),
          i
        ).toDateString(),
      events: []
    }
  }

  for (let i = 1; i <= nextMonthDays; i++) {
    const date = new Date(
      currentDate.value.getFullYear(),
      currentDate.value.getMonth() + 1,
      i + 1
    )
    days.value[date.toISOString().split('T')[0]] = {
      date: sub(date, { hours: 2 }),
      events: []
    }
  }

  if (error) return

  data.forEach(e => {
    e.type = 'availability'
    e.start = parseISO(e.start)
    e.end = parseISO(e.end)

    const sameDay = isSameDay(
      sub(e.start, { hours: 2 }),
      sub(e.end, { hours: 2 })
    )
    if (!sameDay) {
      chunkEvents(e, days)
    } else {
      const datetime = e.start.toISOString().split('T')[0]
      days.value[datetime].events.push(
        transformEvent(datetime, {
          ...e,
          time: format(sub(e.start, { hours: 2 }), 'hh:mm a')
        })
      )
    }
  })
}

defineExpose({
  currentDate,
  previous: previousMonth,
  next: nextMonth,
  openEventModal: () => (eventModalOpen.value = true),
  goToToday
})

await renderCalendar()
selectDay(today)
</script>
