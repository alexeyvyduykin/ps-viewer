/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, watch } from 'vue'
import type { GroundStation } from '@/types'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import { usePlannedScheduleStore } from './plannedSchedule'

const url = 'api/ps/getgss'

export const useGroundStationStore = defineStore('groundStation', () => {
  const psStore = usePlannedScheduleStore()

  const { ps } = storeToRefs(psStore)

  const gss = ref<GroundStation[] | undefined>()
  const isLoading = ref(true)

  // async function getGroundStations() {
  //   isLoading.value = true;
  //   myFetch(url).then((res) => {
  //     const data = res.response.value as GroundStation[];
  //     gss.value = data;
  //     isLoading.value = false;
  //   });
  // }

  async function getGroundStations() {
    isLoading.value = true
    await new Promise((resolve) => setTimeout(resolve, 200))
    gss.value = ps.value?.groundStations
    isLoading.value = false
  }

  watch(ps, (ps) => {
    isLoading.value = true
    if (ps) {
      gss.value = ps.groundStations
    }
    isLoading.value = false
  })

  return {
    gss,
    isLoading,
    getGroundStations
  }
})
