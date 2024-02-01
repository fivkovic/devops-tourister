<template>
  <div @mouseenter="() => import('../../views/business/PropertyProfileView.vue')">
    <div class="w-fit group cursor-pointer">
      <div class="relative h-36 w-48">
        <h4
          v-if="props.result.rating > 0"
          class="absolute top-2 right-1.5 rounded-xl bg-emerald-50 px-2.5 py-0.5 text-[13px] font-semibold tracking-tight text-[#05a876]"
        >
          {{ props.result.rating.toPrecision(2) }}
        </h4>
        <img
          alt=""
          :src="api.properties.image(props.result.images[0])"
          class="h-full w-full rounded object-cover ring-2 ring-black ring-opacity-0 ring-offset-2 transition-all group-hover:ring-opacity-100"
          :class="{
            'ring-opacity-100': props.result.selected
          }"
        />
      </div>
      <div>
        <div>
          <h2 class="mt-2 font-semibold transition-all underline group-hover:decoration-black decoration-transparent">{{ props.result.name }}</h2>
          <h3 class="text-[13px] text-gray-500">{{ props.result.location }}</h3>
          <div v-if="props.hasDetails" class="flex items-center space-x-2 pt-1.5">
            <h3 class="text-lg font-semibold tracking-tight text-emerald-600">
              ${{ props.result.totalPrice }}
            </h3>
<!--            <div class="flex items-center space-x-1 text-neutral-500">-->
<!--              <UserIcon stroke-width="2" class="h-4 w-4 pb-px" />-->
<!--              <h4 class="font- text-[13px]">{{ props.result.maxPeople }}</h4>-->
<!--            </div>-->
          </div>
        </div>
      </div>
    </div>
    <Menu 
      @click.stop
      v-if="props.hasMenu"
      @selected="e => emit('optionSelected', e)"
      :values="result.values"
      class="mt-2"
    />
  </div>
</template>

<script setup>
import api from '@/api/api.js'
import {BedIcon, HotelServiceIcon, UserIcon} from "vue-tabler-icons"
import Menu from "@/components/ui/Menu.vue";
const props = defineProps(['result', 'priceUnit', 'hasMenu', 'hasDetails'])
const emit = defineEmits(['optionSelected'])
</script>
