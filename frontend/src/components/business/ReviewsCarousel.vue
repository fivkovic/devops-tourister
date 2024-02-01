<script setup>
import { formatDistanceToNow, parseJSON } from 'date-fns'

const props = defineProps({
  reviews: { type: Array },
  userId: { type: String },
  reviewType: { type: String }
})

const emit = defineEmits(['deleteReview'])
</script>

<template>
  <div class="w-full space-y-3">
    <div v-if="props.reviews?.length > 0" class="flex items-start space-x-5 pb-4">
      <div
          v-for="review in props.reviews"
          :key="review.id"
          class="w-[300px] shrink-0 flex flex-col space-y-3 rounded-lg border px-5 py-6 min-h-[250px]"
      >
        <div class="flex items-center space-x-3">
          <span class="flex h-8 w-8 items-center justify-center rounded-full bg-black font-bold text-white text-sm">
            {{ review.customer.firstName[0] }}{{ review.customer.lastName[0] }}
          </span>
          <div>
            <p class="text-sm font-medium">
              {{ review.customer.firstName }} {{ review.customer.lastName }}
            </p>
            <p class="text-sm text-neutral-500">
              {{ formatDistanceToNow(parseJSON(review.timestamp), { addSuffix: true })}} 
            </p>
          </div>
          <div class="flex flex-1 justify-end">
            <button 
              v-if="props.userId === review.customer.id"
              @click="emit('deleteReview', review.id)"
              class="text-[13px] font-medium rounded-full px-2.5 py-0.5 border border-red-600
              text-red-600 hover:bg-red-600 hover:text-white transition mb-2"
            >
              Delete
            </button>
          </div>
        </div>
        <div>
          <span>{{ review.rating }}</span>
          <span class="text-sm text-neutral-500">/5</span>
          <span>⭐</span>
        </div>
        <p
          class="text-sm text-gray-600 flex-1"
          :class="{ 'line-clamp-4 ': !review.expanded }"  
        >
          {{ review.content }}
        </p>
        <p 
          @click="review.expanded = !review.expanded"
          class="text-sm cursor-pointer" 
        >
          {{ review.expanded ? 'Collapse' : 'Read more' }}
        </p>
      </div>
    </div>
    <p v-else class="text-[15px] text-neutral-500">There are no reviews for this {{ props.reviewType }} yet</p>
  </div>
</template>