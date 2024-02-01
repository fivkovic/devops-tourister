<template>
  <div>
    <div>
      <h2 class="mb-3 text-lg font-medium">
        Showing schedule for {{ format(selectedDay, 'MMM dd') }}
      </h2>
      <div v-if="selectedDayEvents.length === 0" class="text-neutral-500 text-sm -mt-3">
        There are no periods for the selected date
      </div>
      <div v-else class="space-y-3 ">
        <div
          v-for="event in selectedDayEvents"
          :key="event.id"
          class="rounded-xl border px-4 py-3 text-[15px]"
          :class="{ 'border-neutral-700': selectedEvent?.id === event.id }"
        >
          <p class="text-sm font-medium">Property is available for booking</p>
          <div class="flex items-center space-x-3">
            <p class="flex items-center space-x-1">
              <span class="">
                {{ format(getEventStart(event), 'MMM dd') }}
              </span>
              <span class="text-[10px] text-neutral-300">&bullet;</span>
              <span class="text-sm text-neutral-500">
                {{ format(getEventStart(event), 'hh:mm a') }}
              </span>
            </p>
            <div class="text-sm text-neutral-600">&rarr;</div>
            <p class="flex items-center space-x-1">
              <span class="">
                {{ format(getEventEnd(event), 'MMM dd') }}
              </span>
              <span class="text-[10px] text-neutral-300">&bullet;</span>
              <span class="text-sm text-neutral-500">
                {{ format(getEventEnd(event), 'hh:mm a') }}
              </span>
            </p>
          </div>
          <div v-if="event.customPrice" class="">
            <p class="text-sm text-neutral-500 pr-1">Custom price for this period</p>
            <p>${{event.customPrice.toFixed(2)}}</p>
          </div>
          <p v-else class="text-sm text-neutral-500">The default property price applies for this period</p>
          <Button
            @click="$emit('deleteEvent', event)"
            class="flex ml-auto mt-2 !px-3 !py-0.5 text-red-600 hover:bg-neutral-50 border border-red-600"
          >
            Delete
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { format } from 'date-fns'
import Button from '@/components/ui/Button.vue'
const props = defineProps(['selectedEvent', 'selectedDayEvents', 'selectedDay'])
const emit = defineEmits(['eventSelected', 'deleteEvent', 'eventReported'])

const business = ref()
const customerId = ref()

const getEventStart = e => e.chunked ? e.originalEvent.start : e.start
const getEventEnd = e => e.chunked ? e.originalEvent.end : e.end
</script>
