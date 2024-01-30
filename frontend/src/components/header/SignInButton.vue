<template>
  <RouterLink v-if="!isAuthenticated" to="/signin">
    <Button class="border border-gray-300">Sign in</Button>
  </RouterLink>
  <Button v-else @click="signOut()" class="border border-gray-300"
    >Sign out</Button
  >
</template>

<script setup>
import { RouterLink } from 'vue-router'
import Button from '../ui/Button.vue'
import { isAuthenticated } from '@/stores/userStore'
import api from '@/api/api.js'

const signOut = () => api.auth.signOut()
</script>
