<script setup>
import {defineProps, ref, watch} from 'vue'
import api from '@/api/api'
import { user } from '@/stores/userStore'
import Modal from '@/components/ui/Modal.vue'
import Button from '@/components/ui/Button.vue'
import Loader from "@/components/ui/Loader.vue";

const props = defineProps({
  isOpen: Boolean
})

const emit = defineEmits(['closed'])

const subscriptionSettings = {
  customer: [
    {
      id: 1,
      value: "ReservationUpdated",
      label: "Reservation Updated",
      description: "Get notified when your reservation is accepted or rejected by the property host"
    },
  ],
  host: [
    {
      id: 1,
      value: "ReservationRequested",
      label: "Reservation Requested",
      description: "Get notified when a customer requests a reservation for one of your properties"
    },
    {
      id: 2,
      value: "ReservationCancelled",
      label: "Reservation Cancelled",
      description: "Get notified when a customer cancels a reservation for one of your properties"
    },
    {
      id: 3,
      value: "PropertyReviewed",
      label: "Property Reviewed",
      description: "Get notified when a customer reviews one of your properties"
    },
    {
      id: 4,
      value: "HostReviewed",
      label: "Host Reviewed",
      description: "Get notified when a customer reviews you as a host"
    }
  ]
}

const subscriptions = ref([])
const formError = ref()
const isLoading = ref(false)

watch(() => props.isOpen, async (open) => {
  if (!open) return
  isLoading.value = false
  formError.value = ''
  await getSubscription()
})

const getSubscription = async () => {
  formError.value = ''
  const [data, error] = await api.reservations.getSubscriptions()
  
  if (error) {
    if (error.errors) formError.value = Object.values(error.errors)[0].join(', ')
    else formError.value = error.detail
    return
  }
  
  subscriptions.value = data.split(',').map(sub => sub.trim())
}

const updateSubscription = async () => {
  formError.value = ''
  isLoading.value = true
  
  const subscription = subscriptions.value.join(',') || 'None'
  const [data, error] = await api.reservations.updateSubscriptions(subscription)

  console.log(data, error)
  if (error) {
    isLoading.value = false
    if (error.errors) formError.value = Object.values(error.errors)[0].join(', ')
    else formError.value = error.detail
    return
  }
  
  setTimeout(() => {
    emit('closed')
  }, 300)

}
</script>

<template>
  <Modal 
    @modalClosed="emit('closed')" 
    :is-open="props.isOpen" 
    class="bg-white pt-8 pb-8 px-6 text-left" 
    light
  >
    <h1 class="text-lg font-medium">Subscriptions</h1>
    <p class="text-sm text-neutral-500 pb-8">Select the types of notifications you want to receive</p>
    <div 
      v-for="setting of subscriptionSettings[user.role.toLowerCase()]" 
      :key="setting.id" 
      class="flex space-x-1.5 pb-5"
    >
      <input
        type="checkbox"
        :id="setting.id"
        :value="setting.value"
        v-model="subscriptions"
        class="block cursor-pointer rounded border-neutral-300 p-1.5 text-sm text-black placeholder-neutral-500 
               transition-colors hover:border-neutral-500 focus:border-neutral-500 focus:ring-0 focus:ring-transparent 
               disabled:text-neutral-300 hover:disabled:border-neutral-300 focus:disabled:border-neutral-300"
      />
      <label 
        :for="setting.id"
        class="flex flex-col cursor-pointer select-none pl-px -mt-0.5"
      >
        <span class="text-sm font-medium text-neutral-700 transition-all">{{setting.label}}</span>
        <span class="text-[13.5px] leading-snug text-neutral-500 w-[320px]">{{setting.description}}</span>
      </label>
    </div>
    <p class="text-sm text-center text-red-500">{{ formError }}</p>
    <div class="flex justify-end space-x-2.5 pt-6">
      <Button 
        class="bg-white text-black !py-2.5 px-5 border border-neutral-300 hover:bg-neutral-50 transition" 
        @click="emit('closed')"
      >
        Cancel
      </Button>
      <Button
        class="transition bg-black text-white !py-2.5 px-8 hover:bg-black/90 flex items-center space-x-1.5" 
        :disabled="isLoading"
        @click="updateSubscription()"
      >
        <div v-if="isLoading" class="flex items-center">
          <Loader class="mr-3" />
          Updating...
        </div>
        <div v-else>Update Settings</div>
      </Button>
    </div>
  </Modal>
</template>