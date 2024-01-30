<template>
  <div
    :class="{ [props.horizonal]: 'flex space-x-3 space-y-0' }"
    class="space-y-3"
  >
    <div v-for="item in items" :key="item">
      <Checkbox
        @change="selectItem(item)"
        :checked="item.id === selectedItem?.id"
        :label="item.label"
        :id="item.id"
      />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import Checkbox from './Checkbox.vue'
const emit = defineEmits(['change'])
const props = defineProps({
  items: {
    type: Array,
    required: true
  },
  horizontal: {
    type: Boolean,
    required: false,
    default: false
  },
  selectedValue: {
    type: Object,
    required: false
  }
})
const selectedItem = ref(props.selectedValue ?? {})
const selectItem = item => {
  item.id !== selectedItem.value.id
    ? (selectedItem.value = item)
    : (selectedItem.value = '')
  emit('change', selectedItem.value.value)
}
</script>
