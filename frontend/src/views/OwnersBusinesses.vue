<template>
  <div class="mx-auto max-w-4.5xl h-full flex flex-col">
    <h2 class="text-2xl font-medium">Properties</h2>
    <p class="text-sm text-neutral-500">View and manage all of your properties</p>
    <div v-if="results.length" class="flex flex-wrap gap-14 mt-6">
      <ResultPreviewItem
        v-for="result in results"
        :key="result.id"
        :result="result"
        @click="navigateToProperty(result.id)"
        priceUnit="night"
      />
    </div>
    <div v-else-if="!isLoading" class="flex flex-1 justify-center items-center text-neutral-500 text-sm">
      <div class="pb-16">
        <p>You haven't listed any of your properties on tourister.com yet</p>
        <RouterLink
          to="/property/create"
          class="flex mx-auto mt-2 items-center gap-x-1 bg-black rounded-full text-white !py-2 w-fit px-3.5"
        >
          <span class="text-sm font-medium">List your property</span>
          <PlusIcon class="h-3.5 w-3.5 text-white stroke-[3]" />
        </RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { PlusIcon } from 'vue-tabler-icons'
import api from '../api/api'
import ResultPreviewItem from '@/components/search/ResultPreviewItem.vue'

const router = useRouter()

const isLoading = ref(false)
const results = ref([])

const fetchResults = async () => {
  isLoading.value = true
  const [data, error] = await api.properties.getProperties()
  if (!error) {
    results.value = data
  }
  isLoading.value = false
}

const navigateToProperty = id => router.push(`/properties/${id}`)

fetchResults()

</script>
