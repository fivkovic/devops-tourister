import {
  add,
  sub,
  format,
  differenceInDays,
  startOfDay,
  endOfDay,
  isBefore
} from 'date-fns'
import { ref } from 'vue'
import api from '@/api/api'

let id, type

const selectedEvent = ref()
const selectedDay = ref(new Date())
const selectedDayEvents = ref([])
const days = ref({})

const transformEvent = (datetime, event) => ({
  ...event,
  id: event.id,
  name: event.name,
  minutesStart:
    sub(event.start, { hours: 2 }).getHours() * 60 + event.start.getMinutes(),
  minutesEnd:
    sub(event.end, { hours: 2 }).getHours() * 60 + event.end.getMinutes(),
  minutesDuration:
    sub(event.end, { hours: 2 }).getHours() * 60 +
    event.end.getMinutes() -
    ((event.start.getHours() - 2) * 60 + event.start.getMinutes()),
  datetime
})

const chunkEvents = e => {
  const delta = Math.max(differenceInDays(e.end, e.start), 1)
  for (let i = 0; i <= delta; i++) {
    const start = startOfDay(add(e.start, { days: i }))
    const end = endOfDay(start)

    const datetime = add(start, { hours: 2 }).toISOString().split('T')[0]
    if (datetime in days.value) {
      days.value[datetime].events.push(
        transformEvent(datetime, {
          ...e,
          start: isBefore(start, e.start) ? e.start : add(start, { hours: 2 }),
          end: isBefore(e.end, add(end, { hours: 1, minutes: 59, seconds: 59 }))
            ? e.end
            : add(end, { hours: 1, minutes: 59, seconds: 59 }),
          chunked: true,
          isFirstChunk: i == 0,
          time: format(sub(e.start, { hours: 2 }), 'hh:mm a'),
          originalEvent: {
            reported: e.reported,
            start: e.start,
            end: e.end
          }
        })
      )
    }
  }
}

const selectDay = async date => {
  const day = add(startOfDay(date), { hours: 2 }).toISOString().split('T')[0]
  selectedDay.value = date
  selectedDayEvents.value = days.value[day]?.events ?? []
}

const deleteEvent = async event => {
  const [, error] = await api.properties.deleteAvailability(id, event.id)

  if (!error) {
    selectedEvent.value = null
    Object.values(days.value).forEach(d => {
      d.events = d.events.filter(e => e.id !== event.id)
    })
    selectDay(selectedDay.value)
  }
}

const useEvents = (businessId, businessType) => {
  id = businessId
  type = businessType
  selectedDay.value = new Date()
  return {
    selectedEvent,
    selectedDay,
    selectedDayEvents,
    days,
    transformEvent,
    chunkEvents,
    selectDay,
    deleteEvent
  }
}

export { useEvents }
