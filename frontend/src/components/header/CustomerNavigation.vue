<template>
  <Logo />
  <div v-if="isNonSearchRoute && isAuthenticated" class="flex space-x-8 pl-8">
    <RouterLink
      :to="{ name: 'profile' }"
      v-slot="{ isActive }"
      @mouseenter="() => import('../../views/user/ProfileView.vue')"
      class="flex items-center"
    >
      <div class="relative" :class="{ 'ml-4 pr-1': unreadNotifications }">
        <div :class="{ 'font-medium': isActive }">Profile</div>
        <div
          v-if="isActive"
          class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-black"
        />
      </div>
      <div
        v-if="unreadNotifications"
        class="rounded-full leading-none h-4 w-4 bg-red-500 flex items-center justify-center text-xs font-bold text-white"
      >
        {{unreadNotifications}}
      </div>
    </RouterLink>
    <RouterLink
      :to="{ name: 'reservations' }"
      v-slot="{ isActive }"
      @mouseenter="() => import('../../views/user/Reservations.vue')"
      class="relative w-24 cursor-pointer text-center"
    >
      <div :class="{ 'font-medium': isActive }">
        Reservations
      </div>
      <div
        v-if="isActive"
        class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-black"
      />
    </RouterLink>
  </div>
  <div class="flex items-center space-x-5">
    <div v-if="isAuthenticated" class="flex space-x-2">
      <CustomerProfileOptions />
    </div>
    <RouterLink v-else :to="'/signin'">
      <Button class="bg-black text-white"> List your property </Button>
    </RouterLink>
    <SignInButton />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, RouterLink } from 'vue-router'
import Logo from './Logo.vue'
import Button from '../ui/Button.vue'
import SignInButton from './SignInButton.vue'
import {isAuthenticated, unreadNotifications} from '@/stores/userStore'
import CustomerProfileOptions from './CustomerProfileOptions.vue'
import { nonSearchRoutes } from '@/router/index.js'

const currentRouteName = computed(() => useRoute().name)

const isNonSearchRoute = computed(() =>
  nonSearchRoutes.includes(currentRouteName.value)
)

</script>
