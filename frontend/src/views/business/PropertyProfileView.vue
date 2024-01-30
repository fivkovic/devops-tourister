<template>
  <BusinessProfileView v-if="property" :entity="property" />
  <main v-else class="grid min-h-full place-items-center bg-white px-6">
    <div class="text-center -mt-10">
      <p class="text-base font-semibold">404</p>
      <h1 class="mt-4 text-3xl font-bold tracking-tight text-gray-900 sm:text-5xl">Property not found</h1>
      <p class="mt-6 text-base leading-7 text-gray-600">
        Sorry, we couldn’t find the property you’re looking for. <br>
        It might have been deleted by its host or is currently unavailable. 
      </p>
      <div class="mt-10 flex items-center justify-center gap-x-6">
        <Button @click="router.back()" class="flex space-x-1.5 border border-neutral-300 !px-4 !py-1.5 transition hover:bg-neutral-50">
          Go back
        </Button>
        <button class="text-sm font-semibold text-gray-900">Contact support <span aria-hidden="true">&rarr;</span></button>
      </div>
    </div>
  </main>
</template>

<script setup>
import { ref } from 'vue'
import api from '@/api/api.js'
import BusinessProfileView from './BusinessProfileView.vue'
import { parseISO } from 'date-fns'
import { useRoute } from 'vue-router'
import Button from "@/components/ui/Button.vue"
import router from "@/router"

const property = ref()
const route = useRoute()

const start = route.query.start ? parseISO(route.query.start) : null
const end = route.query.start ? parseISO(route.query.end) : null

const getProperty = async () => {
  const [data] = await api.properties.get(route.params.id, {
    start,
    end
  })
  if (data) property.value = data
}

await getProperty()

</script>
