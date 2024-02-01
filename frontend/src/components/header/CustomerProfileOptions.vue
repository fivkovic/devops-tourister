<template>
  <Menu as="div" class="relative z-10 block text-left">
    <MenuButton class="mt-0.5 flex space-x-2 rounded-full">
      <img
        :src="userDefaultImg"
        alt=""
        class="h-10 w-10 rounded-full border border-gray-300"
      />
      <div class="text-sm">
        <p class="font-bold">Your account</p>
        <p>{{ user.firstName }} {{user.lastName}}</p>
      </div>
    </MenuButton>

    <transition
      enter-active-class="transition duration-100 ease-out"
      enter-from-class="transform scale-95 opacity-0"
      enter-to-class="transform scale-100 opacity-100"
      leave-active-class="transition duration-75 ease-in"
      leave-from-class="transform scale-100 opacity-100"
      leave-to-class="transform scale-95 opacity-0"
    >
      <MenuItems
        class="absolute right-0 mt-3 w-56 overflow-clip origin-top-right rounded-xl border border-gray-300 bg-white shadow-lg"
      >
        <MenuItem as="div" v-slot="{ active }">
          <RouterLink
            :to="'/profile'"
            class="z-20 space-x-0.5"
            :class="[
              active ? 'bg-gray-50' : 'text-gray-900',
              'group mt-1 flex w-full items-center px-3.5 py-3 text-sm'
            ]"
          >
            <div class="flex space-x-2">
              <UserIcon class="h-5 w-5 items-center stroke-2" />
              <span>Manage account</span>
            </div>
          </RouterLink>
        </MenuItem>
        <MenuItem v-if="!props.isAdmin" as="div" v-slot="{ active }">
          <RouterLink
            :to="'/reservations'"
            class="z-20 space-x-0.5"
            :class="[
              active ? 'bg-gray-50' : 'text-gray-900',
              'group mt-1 flex w-full items-center px-3.5 py-3 text-sm'
            ]"
          >
            <div class="flex space-x-2">
              <Map2Icon class="h-5 w-5 items-center stroke-2" />
              <span>My reservations</span>
            </div>
          </RouterLink>
        </MenuItem>
      </MenuItems>
    </transition>
  </Menu>
</template>

<script setup>
import { Menu, MenuButton, MenuItems, MenuItem } from '@headlessui/vue'
import userDefaultImg from '@/assets/images/user-default.png'
import { user } from '@/stores/userStore.js'
import { UserIcon, Map2Icon } from 'vue-tabler-icons'

const props = defineProps({
  isAdmin: {
    type: Boolean,
    required: false,
    default: false
  }
})
</script>
