/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, computed, reactive } from 'vue'
import type { Coordinate, GroundTarget } from '@/types'
import { GroundTargetType } from '@/types'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import { usePlannedScheduleStore } from './plannedSchedule'
import type { LatLng, ControlOptions, LatLngExpression } from 'leaflet'
import * as L from 'leaflet'

export const useMapStore = defineStore('map', () => {
  //const psStore = usePlannedScheduleStore();
  const coordinatesControl = ref<ControlOptions>({})

  const currentLocation = ref<LatLng>()

  const currentZoom = ref<number>()

  const flyLocation = ref<LatLngExpression>()

  function setCurrentLocation(coord: Coordinate) {}
  function setLonLat(coord: LatLng) {
    currentLocation.value = coord
  }

  function setCurrentZoom(zoom: number) {
    currentZoom.value = zoom
  }

  function flyToLocation(location: LatLng) {
    flyLocation.value = location
  }

  return {
    currentZoom,
    currentLocation,
    coordinatesControl,
    flyLocation,
    setCurrentZoom,
    setCurrentLocation,
    setLonLat,
    flyToLocation
  }
})
