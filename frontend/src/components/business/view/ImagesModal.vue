<template>
  <Modal
    @modalClosed="$emit('modalClosed')"
    :isOpen="props.isOpen"
    :hasCloseButton="false"
    :light="true"
    class="relative h-[700px] w-[1000px] bg-transparent border-none"
  >
    <XIcon
      @click="$emit('modalClosed')"
      class="absolute top-3 right-3 h-6 w-6 cursor-pointer text-black"
    />
    <img
      :src="api.properties.image(props.images[selectedImageIndex])"
      alt=""
      class="mx-auto h-full w-full select-none object-cover"
    />
    <div
      class="absolute left-1/2 bottom-3 -translate-x-1/2 rounded-full bg-black/[0.5] py-2 px-8 text-sm text-white"
    >
      {{ selectedImageIndex + 1 }} / {{ props.images.length }}
    </div>
    <div
      @click="previousImage()"
      class="absolute -left-16 top-0 flex h-full w-20 cursor-pointer items-center select-none"
    >
      <ChevronLeftIcon class="h-16 w-16 cursor-pointer text-black" />
    </div>
    <div
      @click="nextImage()"
      class="absolute -right-20 top-0 flex h-full w-20 cursor-pointer items-center select-none"
    >
      <ChevronRightIcon class="h-16 w-16 cursor-pointer text-black" />
    </div>
  </Modal>
</template>
<script setup>
import {ref, watch} from 'vue'
import { ChevronLeftIcon, ChevronRightIcon, XIcon } from 'vue-tabler-icons'
import Modal from '../../ui/Modal.vue'
import api from "@/api/api";
const props = defineProps(['images', 'isOpen'])
const selectedImageIndex = ref(0)

watch(
    () => props.isOpen, 
    () => {
      const handler = (e) => {
        if (e.key === 'ArrowRight') {
          nextImage()
        } else if (e.key === 'ArrowLeft') {
          previousImage()
        }
      }
      if (props.isOpen) {
        document.addEventListener('keydown', handler)
      } else {
        document.removeEventListener('keydown', handler)
      }
  }
)

const nextImage = () => {
  selectedImageIndex.value =
    (selectedImageIndex.value + 1) % props.images.length
}
const previousImage = () =>
  (selectedImageIndex.value =
    (selectedImageIndex.value + props.images.length - 1) % props.images.length)
</script>
