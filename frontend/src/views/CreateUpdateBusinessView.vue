<template>
  <div>
    <div class="my-6 flex flex-col items-center justify-between">
      <div class="ml-2 grid auto-cols-fr grid-cols-3">
        <div class="ml-auto mr-8">
          <div v-for="(step, idx) in stepNames" :key="step">
            <div
              class="flex text-lg font-medium"
              :class="{
                'text-neutral-300': idx !== currentStep
              }"
            >
              <div
                class="mr-3.5 mt-0.5 flex aspect-square h-8 w-8 items-center justify-center rounded-full transition-all"
                :class="[
                  idx === currentStep
                    ? 'bg-black text-white'
                    : 'border border-neutral-300 text-neutral-300',
                  idx < currentStep ? 'border border-black' : ''
                ]"
              >
                <CheckIcon
                  v-if="idx < currentStep"
                  class="h-5 w-5 text-black"
                />
                <div v-else>
                  {{ idx + 1 }}
                </div>
              </div>
              <div>
                {{ step }}
                <div
                  :class="{ 'text-neutral-500': idx === currentStep }"
                  class="text-sm"
                >
                  {{ stepDescriptions[idx] }}
                </div>
              </div>
            </div>
            <div
              v-if="idx < steps.length - 1"
              class="-mt-2 mb-0.5 ml-[15.5px] h-7 w-px bg-neutral-300"
            ></div>
          </div>
        </div>
        <KeepAlive>
          <Component
            ref="currentComponent"
            @change="e => onChange(e)"
            :is="steps[currentStep]"
            v-bind="property"
            class="mt-0.5"
          />
        </KeepAlive>
      </div>
    </div>
    <div class="mx-auto flex w-96 items-end">
      <Button
        v-if="currentStep > 0"
        @click="back()"
        type="button"
        class="-ml-2.5 flex items-center space-x-1 !pr-5 transition hover:bg-gray-50"
      >
        <ArrowNarrowLeftIcon stroke-width="1.5" class="text-gray-800" />
        <div>Previous</div>
      </Button>
      <Button
        v-if="currentStep < steps.length - 1"
        @click="forward()"
        type="submit"
        class="ml-auto -mr-1 bg-black !px-7 text-white transition hover:bg-black/90"
      >
        Next
      </Button>
      <Button
        v-if="currentStep === steps.length - 1"
        @click="forward()"
        :disabled="isLoading"
        type="submit"
        class="ml-auto -mr-1 bg-black !px-12 text-white transition hover:bg-black/90 disabled:cursor-not-allowed disabled:hover:bg-black"
      >
        <div v-if="isLoading" class="flex items-center">
          <Loader class="mr-3" />
          Processing...
        </div>
        <div v-else>Finish</div>
      </Button>
    </div>
  </div>
</template>

<script setup>
import { ref, shallowRef } from 'vue'
import { ArrowNarrowLeftIcon, CheckIcon } from 'vue-tabler-icons'
import { useRouter, useRoute } from 'vue-router'
import api from '@/api/api.js'

import MainInfo from '@/components/business/create/MainInfo.vue'
import AddLocation from '@/components/business/create/AddLocation.vue'
import AddImages from '@/components/business/create/AddImages.vue'
import AddRules from '@/components/business/create/AddRules.vue'
import Pricing from '@/components/business/create/Pricing.vue'
import Button from '@/components/ui/Button.vue'
import Loader from '@/components/ui/Loader.vue'

const router = useRouter()
const route = useRoute()

const action = route.params.action

const property = ref({})

if (action === 'update') {
  const [res, error] = await api.properties.get(route.query.id)
  if (!error) {
    property.value = res
  }
}

const stepNames = [
  'Main Information',
  'Location',
  'Images',
  'Capacity',
  'Pricing'
]

const stepDescriptions = [
  'Tell us the most important details',
  'Where the property is located',
  'Show us some pretty shots',
  'Tell us how many people are allowed',
  'Set the price for your property'
]

const currentStep = ref(0)
const steps = shallowRef([
  MainInfo,
  AddLocation,
  AddImages,
  AddRules,
  Pricing
])

const isLoading = ref(false)
const currentComponent = ref()

const forward = () => {
  currentComponent.value.getValues()
}

const back = () => currentStep.value--

const onChange = async newData => {
  property.value = {
    ...property.value,
    ...newData
  }

  if (currentStep.value === stepNames.length - 1) {
    isLoading.value = true
    const [data] =
      action === 'update'
        ? await api.properties.update({ id: route.query.id, ...property.value })
        : await api.properties.create(property.value)
    if (data) {
      await router.push(`/properties/${data.id}`)
    }
    isLoading.value = false
  } else {
    currentStep.value++
  }
}
</script>
