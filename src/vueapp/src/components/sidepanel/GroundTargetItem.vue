<template>
  <div class="flex align-items-center px-4 py-3">
    <div>
      <div class="text-xl font-bold">{{ item.name }}</div>
      <div class="text-sm">{{ toType(item) }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onBeforeMount } from 'vue'
import { type GroundTarget } from '@/types'

defineProps({
  item: {
    type: Object as () => GroundTarget,
    required: true
  }
})

onBeforeMount(() => {})

const toType = (item: GroundTarget): string => {
  switch (item.type) {
    case 0:
      return 'Point'
    case 1:
      return 'Route'
    case 2:
      return 'Area'
    default:
      return 'Unknown'
  }
}
</script>

<style scoped></style>
