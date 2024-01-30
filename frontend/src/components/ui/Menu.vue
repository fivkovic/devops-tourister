<template>
  <div class="w-56">
    <Menu as="div" class="relative inline-block text-left">
      <div>
        <MenuButton
          class="inline-flex w-full justify-center rounded-md border border-neutral-300 bg-white px-4 py-2 text-sm font-medium hover:border-neutral-300 hover:bg-opacity-30 focus:outline-none focus-visible:ring-2 focus-visible:ring-white focus-visible:ring-opacity-75"
        >
          Options
          <ChevronDownIcon class="ml-2 -mr-1 h-5 w-5" aria-hidden="true" />
        </MenuButton>
      </div>

      <transition
        enter-active-class="transition duration-100 ease-out"
        enter-from-class="transform scale-95 opacity-0"
        enter-to-class="transform scale-100 opacity-100"
        leave-active-class="transition duration-75 ease-in"
        leave-from-class="transform scale-100 opacity-100"
        leave-to-class="transform scale-95 opacity-0"
      >
        <MenuItems
          class="absolute right-0 mt-2 w-44 origin-top-right rounded-md bg-white py-1 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none"
        >
          <div v-for="value in values" :key="value.value" class="px-1">
            <MenuItem v-slot="{ active }">
              <button
                @click="$emit('selected', value.value)"
                :class="[
                  active && 'bg-neutral-100',
                  'group flex w-full items-center rounded-md px-2 py-2 text-sm'
                ]"
              >
                <Component
                  :is="value.icon"
                  :active="active"
                  class="mr-2 h-5 w-5 text-neutral-900"
                  aria-hidden="true"
                />

                {{ value.name }}
              </button>
            </MenuItem>
          </div>
        </MenuItems>
      </transition>
    </Menu>
  </div>
</template>

<script setup>
import { Menu, MenuButton, MenuItems, MenuItem } from '@headlessui/vue'
import { ChevronDownIcon } from 'vue-tabler-icons'

defineProps({ values: Array })
defineEmits(['selected'])
</script>
