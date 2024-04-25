<template>
  <div class="px-4 py-3">
    <div class="pr-4">
      <div class="flex">
        <div class="text-xl font-bold">{{ item.taskName }}</div>
        <PrimeButton @click="previewClick">Preview</PrimeButton>
        <PrimeButton @click="previewSatelliteClick">Satellite</PrimeButton>
      </div>
      <div class="flex flex-column align-items-end text-sm">
        <div>{{ satStr }}</div>
        <div>{{ item.interval.begin }}</div>
        <div>{{ item.interval.duration }} sec</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ObservationTaskResult } from '@/types'
//import { usePreviewStore } from '@/stores/layers/preview'
//import { useMapStore } from '@/stores/map'
//import { LatLng } from 'leaflet'
import { computed, onBeforeMount } from 'vue'

const { item } = defineProps({
  item: {
    type: Object as () => ObservationTaskResult,
    required: true
  }
})

const satStr = computed(() => {
  return `${item.satelliteName} (${item.node} - ${item.direction === 0 ? 'Left' : 'Right'})`
})

//const previewStore = usePreviewStore()
//const mapStore = useMapStore()
//const { preview, previewSatellite } = previewStore

const previewClick = async () => {
  //await preview(item.taskName)
  //   const latLng = new LatLng(
  //     item.geometry.center.coordinates[1],
  //     item.geometry.center.coordinates[0]
  //   )
  //   mapStore.flyToLocation(latLng)
}

const previewSatelliteClick = async () => {
  //await previewSatellite(item.satelliteName, item.node)
}

onBeforeMount(() => {
  //epoch.value = props.item.epoch.toISOString();
})
</script>

<style scoped></style>
