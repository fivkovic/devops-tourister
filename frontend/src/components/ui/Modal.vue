<template>
  <TransitionRoot as="template" :show="isOpen" appear>
    <Dialog
      :initialFocus="{ focus: () => {} }"
      as="div"
      static
      class="fixed inset-0 z-20 overflow-y-auto"
      @close="emitModalClosed()"
    >
      <div
        class="flex min-h-screen items-end justify-center px-4 pt-4 pb-20 text-center sm:block sm:p-0"
      >
        <TransitionChild
          as="template"
          enter="ease-out duration-150"
          enter-from="opacity-0"
          enter-to="opacity-100"
          leave="ease-in duration-100"
          leave-from="opacity-100"
          leave-to="opacity-0"
        >
          <DialogOverlay
            class="fixed inset-0 bg-black/[85%] transition-opacity"
            :class="{ 'bg-white/[65%]': light }"
          />
        </TransitionChild>
        <TransitionChild
          as="template"
          enter="ease-out duration-50"
          enter-from="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
          enter-to="opacity-100 translate-y-0 sm:scale-100"
          leave="ease-in duration-100"
          leave-from="opacity-100 translate-y-0 sm:scale-100"
          leave-to="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
        >
          <div class="flex h-screen items-center justify-center">
            <div
              class="relative transform rounded-lg transition-all duration-500 border border-neutral-200"
              style="box-shadow: 0 6px 20px -4px rgb(0 0 0 / 25%)"
              v-bind="$attrs"
            >
              <slot></slot>
              <div
                v-if="hasCloseButton"
                @click="emitModalClosed()"
                class="absolute top-0 right-0 cursor-pointer p-2"
              >
                <XIcon class="h-5 w-5" />
              </div>
            </div>
          </div>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script>
import {
  Dialog,
  DialogOverlay,
  TransitionChild,
  TransitionRoot
} from '@headlessui/vue'

import { XIcon } from 'vue-tabler-icons'
export default {
  inheritAttrs: false,
  components: {
    Dialog,
    DialogOverlay,
    TransitionChild,
    TransitionRoot,
    XIcon
  },
  props: {
    isOpen: Boolean,
    classes: String,
    light: Boolean,
    hasCloseButton: { type: Boolean, default: true }
  },
  emits: ['modalClosed'],
  setup: (props, { emit }) => {
    const emitModalClosed = () => emit('modalClosed')
    return {
      emitModalClosed
    }
  }
}
</script>
