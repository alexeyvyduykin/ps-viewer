/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, computed, watch } from 'vue'
import type { Satellite } from '@/types'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import { usePlannedScheduleStore } from './plannedSchedule'

const url = 'api/ps/getsatellites'

export const useSatelliteStore = defineStore('satellites', () => {
  const psStore = usePlannedScheduleStore()
  const satellites = ref<Satellite[] | undefined>()
  const isLoading = ref(true)
  const { ps } = storeToRefs(psStore)

  // async function getSatellites() {
  //   isLoading.value = true;
  //   myFetch(url).then((res) => {
  //     const data = res.response.value as Satellite[];
  //     satellites.value = data;
  //     isLoading.value = false;
  //   });
  // }

  async function getSatellites() {
    isLoading.value = true
    await new Promise((resolve) => setTimeout(resolve, 200))
    satellites.value = psStore.ps?.satellites
    isLoading.value = false
  }

  watch(ps, (ps) => {
    isLoading.value = true
    if (ps) {
      satellites.value = ps.satellites
    }
    isLoading.value = false
  })

  return {
    satellites,
    isLoading,
    getSatellites
  }
})
