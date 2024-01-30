<template>
  <Logo class="" />
  <div v-if="isAuthenticated" class="flex space-x-8">
    <div class="relative cursor-pointer">
      <RouterLink 
        :to="{ name: 'profile' }" 
        v-slot="{ isActive }" 
        @mouseenter="() => import('../../views/user/ProfileView.vue')"
        class="flex items-center"
      >
        <div :class="{ 'ml-4 pr-1': unreadNotifications }">
          <div :class="{ 'font-medium': isActive }">Profile</div>
          <div
              v-if="isActive"
              class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-black"
          />
        </div>
        <div 
          v-if="unreadNotifications" 
          class="rounded-full mb-0.5 leading-none h-4 w-4 bg-red-500 flex items-center justify-center text-xs font-bold text-white"
        >
          {{unreadNotifications}}
        </div>
      </RouterLink>
    </div>
    <div class="relative cursor-pointer">
      <RouterLink
        to="/properties"
        v-slot="{ isActive }"
        @mouseenter="() => import('../../views/OwnersBusinesses.vue')"
      >
        <div :class="{ 'font-medium': isActive }">
          Properties
        </div>
        <div
          v-if="isActive"
          class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-black"
        ></div>
      </RouterLink>
    </div>
    <div class="relative cursor-pointer">
      <RouterLink
        to="/reservations"
        v-slot="{ isActive }"
        @mouseenter="() => import('../../views/user/Reservations.vue')"
      >
        <div :class="{ 'font-medium': isActive }">
          Resevations
        </div>
        <div
          v-if="isActive"
          class="absolute left-1/2 h-1 w-14 -translate-x-1/2 rounded-full bg-black"
        />
      </RouterLink>
    </div>
  </div>
  <div class="flex space-x-3">
    <RouterLink to="/property/create">
      <Button class="bg-black text-white"> List your property </Button>
    </RouterLink>
    <SignInButton />
  </div>
</template>

<script setup>
import { RouterLink } from 'vue-router'
import Logo from './Logo.vue'
import SignInButton from './SignInButton.vue'
import Button from '../ui/Button.vue'
import {isAuthenticated, unreadNotifications} from '@/stores/userStore'
</script>
