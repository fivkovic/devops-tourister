<template>
  <div class="h-full">
    <div
      :key="selectedImage"
      class="bg-image relative mx-auto h-[80%] overflow-clip rounded-2xl lg:w-4/5"
      :style="{ backgroundImage: `url(${selectedImage})` }"
    >
      <div
        v-once
        class="mx-auto flex h-full max-w-4xl flex-col justify-center pb-24 2xl:max-w-5xl 2xl:pb-52"
      >
        <h1 class="w-full text-8xl font-bold text-white">
          Air, sleep, <br />dream
        </h1>
        <h2 class="mt-2 max-w-[320px] text-lg text-white">
          Choose one of many great experiences to go on with your friends.
        </h2>
        <Button
          @click="startSearch()"
          class="mt-5 flex w-fit items-center space-x-2 bg-black pl-6 pr-4 text-white"
        >
          <div>Start your search</div>
          <ArrowRightIcon stroke-width="1.25" />
        </Button>
      </div>
    </div>
    <SearchBar class="relative -mt-20" />
  </div>
</template>

<script setup>
import Button from '@/components/ui/Button.vue'
import SearchBar from '@/components/homepage/SearchBar.vue'
import { ArrowRightIcon } from 'vue-tabler-icons'
import image from '@/assets/images/cabins.png'

const selectedImage = image
const startSearch = () => {
  document.getElementById('Location').focus()
}
</script>

<style>
.bg-image {
  background-size: cover;
  background-position: left;
}
</style>
