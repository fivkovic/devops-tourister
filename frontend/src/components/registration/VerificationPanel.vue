<template>
  <div class="space-y-7">
    <CheckIcon class="mx-auto h-28 w-28 rounded-full ring-4 ring-black stroke-black stroke-[2.5] p-4"/>
    <h1 class="text-center text-lg">
      {{ successText }}
    </h1>
  </div>
</template>

<script setup>
import { CheckIcon } from 'vue-tabler-icons'

defineProps(['successText'])
</script>
