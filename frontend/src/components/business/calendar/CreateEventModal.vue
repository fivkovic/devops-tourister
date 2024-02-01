<template>
  <Modal :is-open="props.isOpen" class="bg-white pt-8 pb-8 px-6 text-left" light>
    <h1 class="mb-3 text-xl font-medium">
      Add availability period
    </h1>
    <p class="text-sm text-neutral-500 -mt-3">Your property will be available for booking in the specified period</p>
    <form @submit.prevent="createAvailability()" class="space-y-4">
      <div class="mt-4 flex space-x-3">
        <div >
          <label
            class="mb-2 block w-min whitespace-nowrap pl-px text-sm font-medium tracking-tight text-neutral-700"
          >
            Start
          </label>
          <DateInput
            :required="true"
            @change="value => (start = value)"
            placeholder="Start date"
            class="h-12 w-full pl-11 -ml-3"
          />
        </div>
        <div>
          <label class="mb-2 block w-min whitespace-nowrap pl-px text-sm font-medium tracking-tight text-neutral-700">
            End
          </label>
          <DateInput
            :required="true"
            @change="value => (end = value)"
            placeholder="End date"
            class="h-12 w-full pl-11 -ml-3"
          />
        </div>
      </div>
      <div class=" flex justify-between ">
        <Checkbox
          id="create-availability-custom-price"
          label="Set custom price for this period"
          v-model="isCustomPrice"
        />
        <div class="">
          <NumberInput 
            :required="isCustomPrice"
            :disabled="!isCustomPrice" 
            v-model="price"
            :min="1" 
            :hasSpinner="false" 
            class="w-[228px] h-12"
            :class="{ 'pl-5': isCustomPrice && price }"
            :placeholder="isCustomPrice ? '$0.00' : 'Default price'"
          >
            <template v-if="isCustomPrice && price" #prepend>
              <span class="pl-2.5">$</span>
            </template>
          </NumberInput>
        </div>
      </div>
      <p class="text-sm text-neutral-500 w-[460px]">
        If you don't set a custom price, the default price entered while creating the property will be used.
      </p>
      <p class="text-sm text-center text-red-500">{{formError}}</p>
      <div class="ml-1.5 flex w-full justify-center">
        <Button class="mt-2.5 w-full bg-black text-white">Add</Button>
      </div>
    </form>
  </Modal>
</template>

<script setup>
import { ref, defineProps, watch } from 'vue'
import { useRoute } from 'vue-router'

import Modal from '@/components/ui/Modal.vue'
import DateInput from '@/components/ui/DateInput.vue'
import Button from '@/components/ui/Button.vue'
import api from '@/api/api.js'
import Checkbox from "@/components/ui/Checkbox.vue";
import Input from "@/components/ui/Input.vue";
import NumberInput from "@/components/ui/NumberInput.vue";

const props = defineProps({
  isOpen: Boolean,
})
const emit = defineEmits('success', 'modalClosed')

const start = ref()
const end = ref()
const isCustomPrice = ref(false)
const price = ref('')
const businessId = useRoute().params.id

const formError = ref()

watch(() => isCustomPrice.value, (custom) => {
  if (!custom) price.value = ''
})

watch(() => props.isOpen, (open) => {
  if (!open) return
  start.value = null
  end.value = null
  formError.value = ''
  isCustomPrice.value = false
  price.value = ''
})

const createAvailability = async () => {
  formError.value = ''
  const [data, error] = await api.properties.createAvailability(
      businessId, 
      start.value, 
      end.value,
      isCustomPrice.value ? Number(price.value) : null
  )
  if (error) {
    if (error.errors) formError.value = Object.values(error.errors)[0].join(', ')
    else formError.value = error.detail
  }
  else {
    emit('success', data)
    emit('modalClosed')
  }
}
</script>
