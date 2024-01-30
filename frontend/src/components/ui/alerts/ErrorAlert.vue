<template>
  <div
    class="fixed right-0 left-0 z-20 mx-auto mt-5 max-w-sm rounded-md bg-red-50 p-4 transition-all"
  >
    <div class="flex">
      <div class="flex-shrink-0">
        <CircleXIcon class="h-5 w-5 text-red-400" aria-hidden="true" />
      </div>
      <div class="ml-3">
        <p class="text-sm font-medium text-red-800">
          <slot></slot>
        </p>
      </div>
      <div class="ml-auto pl-3">
        <div class="-mx-1.5 -my-1.5">
          <button
            @click="$emit('close')"
            type="button"
            class="inline-flex rounded-md bg-red-50 p-1.5 text-red-500 hover:bg-red-100 focus:outline-none focus:ring-2 focus:ring-red-600 focus:ring-offset-2 focus:ring-offset-red-50"
          >
            <span class="sr-only">Dismiss</span>
            <XIcon class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { CircleXIcon, XIcon } from 'vue-tabler-icons'
</script>
