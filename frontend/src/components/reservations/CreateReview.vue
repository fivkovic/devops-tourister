<template>
  <Modal
    :isOpen="isOpen"
    @modalClosed="$emit('modalClosed')"
    :light="true"
    class="bg-white py-14 px-10 text-left"
  >
    <form ref="reviewForm" @submit.prevent="submitReview()">
      <h1 class="text-gray-600">How was your experience with</h1>
      <div v-if="props.type === 'HOST'">
        <h2 class="text-3xl font-medium">{{ props.host.firstName }} {{ props.host.lastName }}</h2>
        <h2 class="font-medium">{{ name }}</h2>
      </div>
      <h2 v-else-if="props.type === 'PROPERTY'" class="text-3xl font-medium">{{ name }}</h2>
<!--      <div class="text-gray-600 text-[15px]">-->
<!--        {{ duration }} {{ unit }} in {{ address }}-->
<!--      </div>-->
      <div class="text-gray-600 text-[15px]">From {{ start }} to {{ end }}</div>
      <h2 class="mt-7 font-medium">1. How would you rate your experience?</h2>
      <h3 class="text-sm text-gray-600">
        Please give a rating for this {{ props.type === 'PROPERTY' ? 'property' : 'host' }}.
      </h3>
      <div class="mt-5 flex space-x-2">
        <div
          v-for="grade in grades"
          :key="grade.grade"
          @click="selectedGrade = grade.grade"
          :class="{
            'outline outline-2 outline-black': selected(grade.grade)
          }"
          class="flex h-24 w-24 cursor-pointer flex-col items-center justify-center space-y-1 rounded-2xl border border-gray-300 hover:bg-gray-50"
        >
          <component
            :is="grade.icon"
            :class="{ '': selected(grade.grade) }"
            class="h-9 w-9 stroke-1"
          />
          <div
            :class="{ 'font-medium': selected(grade.grade) }"
            class="text-sm"
          >
            {{ grade.description }}
          </div>
        </div>
      </div>
      <h2 class="mt-10 font-medium">2. Give us your review</h2>
      <h3 class="text-sm text-gray-600">
        In a few sentences, describe your experience. Point out some good and
        bad parts.
      </h3>
      <TextArea
        required
        v-model="content"
        rows="5"
        :placeholder="`The ${props.type === 'PROPERTY' ? 'property' : 'host'} was really nice because...`"
        class="mt-3 rounded-lg"
      />
      <p class="text-sm text-red-500">{{ errorMessage }}</p>
      <Button class="flex ml-auto mt-5 bg-black text-white space-x-2" :disabled="isLoading">
        <span>Submit your review</span>
        <Loader v-if="isLoading"/>
      </Button>
    </form>
  </Modal>
</template>
<script setup>
import { ref, watch } from 'vue'
import Modal from '@/components/ui/Modal.vue'
import TextArea from '@/components/ui/TextArea.vue'
import Button from '@/components/ui/Button.vue'
import api from '@/api/api'
import {
  MoodCryIcon,
  MoodSadIcon,
  MoodEmptyIcon,
  MoodSmileIcon,
  MoodHappyIcon
} from 'vue-tabler-icons'
import { useRouter } from 'vue-router'
import Loader from "@/components/ui/Loader.vue";

const props = defineProps([
  'name',
  'address',
  'start',
  'end',
  'isOpen',
  'duration',
  'unit',
  'id',
  'propertyId',
  'type',
  'host'
])

const emit = defineEmits(['modalClosed', 'success'])

const router = useRouter()
const reviewForm = ref()
const selectedGrade = ref(0)
const content = ref('')
const errorMessage = ref('')
const isLoading = ref(false)
const selected = grade => selectedGrade.value === grade

const grades = [
  { grade: 1, icon: MoodCryIcon, description: 'Bad' },
  { grade: 2, icon: MoodSadIcon, description: 'Okay' },
  { grade: 3, icon: MoodEmptyIcon, description: 'Good' },
  { grade: 4, icon: MoodSmileIcon, description: 'Very Good' },
  { grade: 5, icon: MoodHappyIcon, description: 'Excellent' }
]

watch(() => props.isOpen, () => {
  if (props.isOpen) {
    selectedGrade.value = 0
    content.value = ''
    errorMessage.value = ''
    isLoading.value = false
  }
})

const submitReview = async () => {
  if (selectedGrade.value === 0 || !content.value) {
    errorMessage.value = 'You must leave a grade and a comment!'
    return
  }
  errorMessage.value = ''
  isLoading.value = true
  
  const [, error] = await api.reservations.review(
    props.id,
    content.value,
    selectedGrade.value,
    props.type /* PROPERTY or HOST */
  )

  if (error) {
    if (error.errors) errorMessage.value = Object.values(error.errors)[0].join(', ')
    else errorMessage.value = error.detail
    isLoading.value = false
    return
  }
  
  setTimeout(() => {
    emit('success')
    isLoading.value = false
  }, 1000)
  
  // setTimeout(async () => {
  //   await router.push({
  //     name: 'property',
  //     params: { id: props.propertyId }
  //   })
  //   isLoading.value = false
  // }, 1000)
}
</script>
