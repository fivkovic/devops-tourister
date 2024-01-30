<template>
  <form
    name="pricing-form"
    id="pricing-form"
    @submit.prevent="onSubmit()"
    class="w-96"
  >
    <h3 class="mt-6 text-lg font-medium">Pricing</h3>
    <h5 class="text-neutral-500">How much do you want to charge per unit?</h5>
    <p class="text-sm text-neutral-500 mb-3 mt-1">
      A unit is defined per person or per night, which you can change later for each property
    </p>
    <div>
      <div class="w-[180px] space-x-4">
        <Input
            required
            min="0"
            max="1000000"
            v-model="unitPrice"
            class="h-12 w-full !pl-7 !pr-2.5"
            :class="{ '!pl-10': currency === 'RSD' }"
            label="Unit price"
            type="number"
            name="price"
            id="price"
            placeholder="0.00"
            step="0.01"
          >
            <template #prepend="{ focused, hovered }">
              <div class="pointer-events-none flex items-center pl-3">
                <span
                  :class="[
                    'mt-0.5 text-[15px]',
                    focused || hovered ? 'text-neutral-600' : 'text-neutral-400'
                  ]"
                >
                  $
                </span>
              </div>
            </template>
            <template #append> </template>
          </Input>
      </div>

      <div v-if="unitPrice !== ''" class="mt-3">
        <p class="whitespace-nowrap">
          <span class="font-semibold text-emerald-500">
            {{ symbols[currency] }}{{ priceAfterTax }}
          </span>
          Your earnings (including taxes)
        </p>
        <p class="whitespace-nowrap text-[13px] text-neutral-500">
          20% tourister.com commission
        </p>
      </div>
    </div>
  </form>
</template>

<script setup>
import { ref, computed } from 'vue'

import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const props = defineProps(['services', 'pricePerUnit', 'cancellationFee'])
const emit = defineEmits(['change'])

const symbols = {
  USD: '$',
  EUR: '€',
  RSD: 'din '
}

const service = ref('')
const servicePrice = ref('')
const unitPrice = ref(props.pricePerUnit?.amount ?? '')
const currency = ref(props.pricePerUnit?.currency ?? 'USD')
const cancellationFee = ref(props.cancellationFee ?? '')
const services = ref(props.services ?? [])

const priceAfterTax = computed(() => (Number(unitPrice.value) * 0.8).toFixed(2))

const serviceValid = computed(() => {
  return (
    !services.value.find(
      s => s.name.toLowerCase() == service.value.toLowerCase()
    ) &&
    service.value != '' &&
    servicePrice.value != '' &&
    Number(servicePrice.value) > 0
  )
})

const addService = () => {
  const newService = {
    name: service.value,
    price: {
      amount: Number(servicePrice.value),
      currency: currency.value
    }
  }
  services.value.push(newService)
  service.value = ''
  servicePrice.value = ''
}

const removeService = serviceName => {
  services.value = services.value.filter(r => r.name !== serviceName)
}

const onSubmit = () =>
  emit('change', {
    services: services.value,
    unitPrice: unitPrice.value,
    pricingStrategy: 0,
    cancellationFee: cancellationFee.value
  })

const getValues = () => document.getElementById('pricing-form').requestSubmit()

defineExpose({
  getValues
})
</script>

<script>
export default {
  inheritAttrs: false
}
</script>
