<template>
  <form
    id="add-images-form"
    name="add-images-form"
    @submit.prevent="onSubmit()"
    class="w-96"
  >
    <h3 class="text-lg font-medium">Show us your property</h3>
    <h5 class="text-neutral-500">Your customers will value images the most</h5>
    <div
      @dragover="isDragging = true"
      @dragleave="isDragging = false"
      @drop="isDragging = false"
      class="relative mt-5 flex max-w-lg justify-center rounded-md border-2 border-neutral-300 px-6 pt-5 pb-6"
      :class="[
        isDragging ? 'border-black' : 'border-dashed border-neutral-300'
      ]"
    >
      <div class="space-y-1 text-center">
        <CameraPlusIcon class="mx-auto mb-2 h-6 w-6 text-neutral-600" />
        <div class="flex text-sm text-neutral-600">
          <label
            for="file-upload"
            class="cursor-pointer rounded-md bg-white text-black font-medium focus-within:outline-none focus-within:ring-2 focus-within:ring-black focus-within:ring-offset-2 hover:text-gray-800"
          >
            <span>Upload a file</span>
            <input
              :required="images.length === 0"
              id="file-upload"
              name="file-upload"
              type="file"
              accept=".jpg,.jpeg,.png"
              multiple
              @change="e => handleImages(e)"
              class="absolute inset-0 block w-full cursor-pointer bg-red-400 opacity-0"
            />
          </label>
          <p class="pl-1">or drag and drop</p>
        </div>
        <p class="text-xs text-gray-500">PNG, JPG, GIF up to 4MB</p>
      </div>
    </div>
    <div class="mx-auto mt-6 flex flex-wrap gap-3">
      <div
        v-for="image in images"
        :key="image.name"
        class="group relative overflow-clip rounded"
      >
        <img
          :src="imageUrl(image)"
          alt=""
          class="h-36 w-[11.6rem] object-cover"
        />
        <div
          class="absolute inset-0 bg-black/40 p-2.5 opacity-0 transition-all group-hover:opacity-100"
        >
          <div class="text-xs text-white">
            {{ image.name }}
          </div>
          <TrashIcon
            @click="removeImage(image)"
            stroke-width="1.5"
            class="absolute right-2 bottom-3 h-5 w-5 cursor-pointer text-white"
          />
        </div>
      </div>
    </div>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { TrashIcon, CameraPlusIcon } from 'vue-tabler-icons'

const props = defineProps(['images'])
const emit = defineEmits(['change'])

const isDragging = ref(false)
const images = ref([])

const imageUrlToFile = async imgUrl => {
  const fileName = imgUrl.split('/').pop()
  const response = await fetch(
    imgUrl + '?nonce=' + Math.random().toString(36).slice(2)
  )
  const blob = await response.blob()
  return new File([blob], fileName, {
    type: blob.type
  })
}

const handleImages = event => {
  for (const image of event.target.files) {
    images.value.push(image)
  }
}

const imageUrl = image => URL.createObjectURL(image)

const removeImage = image => {
  images.value = images.value.filter(i => i.name !== image.name)
}

const onSubmit = () => emit('change', { images: images.value })

const getValues = () => document.getElementById('add-images-form').requestSubmit()

defineExpose({ getValues })

if (props.images) {
  for (const url of props.images) {
    images.value.push(await imageUrlToFile(url))
  }
}
</script>

<script>
export default {
  inheritAttrs: false
}
</script>
