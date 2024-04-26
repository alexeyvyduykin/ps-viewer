<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<template>
  <div class="map-wrapper">
    <div id="map" ref="mapElement" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import * as L from 'leaflet'
import CustomMap from '@/leaflet/CustomMap'
import { useMapStore } from '@/stores/map'
import { storeToRefs } from 'pinia'
import LeafletService from '@/services/LeafletService'

const mapStore = useMapStore()
const { flyLocation } = storeToRefs(mapStore)
const mapElement = ref<HTMLElement | null>(null)
const visible = ref(false)
let leaflet: CustomMap | undefined

watch(flyLocation, (loc) => {
  leaflet?.flyTo(loc as L.LatLngExpression, 6, {
    duration: 1.6
  } as L.ZoomPanOptions)
})

onMounted(() => {
  if (mapElement.value) {
    leaflet = LeafletService.createMap(mapElement.value!)!
    visible.value = true
  }

  leaflet?.on('moveend', () => {
    mapStore.setLonLat(leaflet!.getCenter())
  })

  leaflet?.on('zoomend', () => {
    mapStore.setCurrentZoom(leaflet!.getZoom())
  })
})

onUnmounted(() => {
  leaflet?.remove()
})
</script>

<style lang="scss" scoped>
#map {
  height: 100%;
  width: auto;
}
</style>
