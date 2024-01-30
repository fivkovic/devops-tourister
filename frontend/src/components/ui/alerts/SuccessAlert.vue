<template>
  <div
    class="fixed right-0 left-0 z-20 mx-auto mt-5 max-w-sm rounded-md bg-green-50 p-4 transition-all"
  >
    <div class="flex">
      <div class="flex-shrink-0">
        <CircleCheckIcon class="h-5 w-5 text-green-400" aria-hidden="true" />
      </div>
      <div class="ml-3">
        <p class="text-sm font-medium text-green-800">
          <slot></slot>
        </p>
      </div>
      <div class="ml-auto pl-3">
        <div class="-mx-1.5 -my-1.5">
          <button
            type="button"
            class="inline-flex rounded-md bg-green-50 p-1.5 text-green-500 hover:bg-green-100 focus:outline-none focus:ring-2 focus:ring-green-600 focus:ring-offset-2 focus:ring-offset-green-50"
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
import { CircleCheckIcon, XIcon } from 'vue-tabler-icons'
</script>
