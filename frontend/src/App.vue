<script setup>
import { RouterView } from 'vue-router'
import TheHeader from './components/header/TheHeader.vue'
</script>

<template>
  <div class="flex h-screen flex-col">
    <Suspense>
      <TheHeader />
    </Suspense>
    <RouterView v-slot="{ Component }">
      <Suspense>
        <div class="flex-1">
          <Component :is="Component" />
        </div>
      </Suspense>
    </RouterView>

    <RouterView name="modal-router" v-slot="{ Component }">
      <Suspense>
        <div>
          <Component :is="Component" />
        </div>
      </Suspense>
    </RouterView>
  </div>
</template>

<style>
@font-face {
  font-family: 'Circular Std';
  src: url('/src/assets/fonts/CircularStd-Book.woff2') format('woff2');
  font-weight: 400;
  font-style: normal;
}

@font-face {
  font-family: 'Circular Std';
  src: url('/src/assets/fonts/CircularStd-Medium.woff2') format('woff2');
  font-weight: 500;
  font-style: normal;
}

@font-face {
  font-family: 'Circular Std';
  src: url('/src/assets/fonts/CircularStd-Bold.woff2') format('woff2');
  font-weight: 700;
  font-style: normal;
}

body {
  text-rendering: optimizeLegibility;
  -webkit-font-smoothing: antialiased;
  font-family: 'Circular Std', sans-serif;
}

.pac-container {
  height: 150px;
}

::-webkit-scrollbar {
  background-color: #fafafa;
  width: 15px;
}

::-webkit-scrollbar-track {
  background-color: transparent;
}

::-webkit-scrollbar-thumb:hover {
  background-color: #b0b0b0;
}

::-webkit-scrollbar-thumb {
  background-color: #c2c2c2;
  border-radius: 16px;
  border: 4px solid transparent;
  background-clip: padding-box;
}

.gmnoprint a,
.gmnoprint span,
.gm-style-cc {
  display: none;
}

.gm-svpc {
  display: none;
}

.gmnoprint div {
  background: white !important;
}

.pac-container {
  text-rendering: optimizeLegibility;
  -webkit-font-smoothing: antialiased;
  font-family: 'Circular Std', sans-serif;
  box-shadow: none;
  @apply ml-px rounded-md border py-1 shadow-md;
}

.pac-container::after {
  content: none;
}

.pac-item {
  @apply cursor-pointer border-none px-3 py-2 text-[13px];
}

.pac-item-selected {
  @apply bg-emerald-50 text-emerald-700;
}

.pac-matched {
  @apply text-sm;
}
</style>
