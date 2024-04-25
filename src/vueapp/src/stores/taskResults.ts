/* eslint-disable @typescript-eslint/no-unused-vars */
import { computed, ref, watch } from 'vue'
import { markRaw } from 'vue'
import type { ObservationTaskResult } from '@/types'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import { usePlannedScheduleStore } from './plannedSchedule'
import { useSatelliteStore } from './satellites'
import { usePsHelperStore } from './psHelper'

//const url = 'api/ps/getobservationtaskresults'

export type SatelliteItem = {
  name: string
  key: string
}

export const useTaskResultStore = defineStore('taskResults', () => {
  const psStore = usePlannedScheduleStore()
  const satelliteStore = useSatelliteStore()
  const { satellites: sats } = storeToRefs(satelliteStore)
  const { ps, minNode, maxNode } = storeToRefs(psStore)
  const taskResults = ref<ObservationTaskResult[] | undefined>()
  const filteringTaskResults = ref<ObservationTaskResult[] | undefined>()
  const isLoading = ref(true)
  const searchString = ref('')

  const nodes = ref([1, 1])

  const isLeft = ref(true)
  const isRight = ref(true)

  const satellites = computed(
    () =>
      sats.value?.map(
        (s, i) =>
          ({
            name: s.name,
            key: `${i}`
          }) as SatelliteItem
      ) ?? []
  )

  const selectedSatellites = ref<string[]>([])

  watch(ps, async (ps) => {
    isLoading.value = true
    if (ps) {
      nodes.value = [minNode.value, maxNode.value]
      selectedSatellites.value = [...ps.satellites.map((s) => s.name)]
    }
    isLoading.value = false
  })

  const isDirty = computed(() => {
    return !(
      nodes.value[0] === minNode.value &&
      nodes.value[1] === maxNode.value &&
      isLeft.value === true &&
      isRight.value === true &&
      selectedSatellites.value.length === satellites.value.length
    )
  })

  function filtering(
    taskResult: ObservationTaskResult,
    search: string,
    satellites: string[],
    left: boolean,
    right: boolean,
    min: number,
    max: number
  ) {
    return (
      taskResult.taskName.includes(search) &&
      satellites.includes(taskResult.satelliteName) &&
      (taskResult.direction === 0 ? left : right) &&
      taskResult.node >= min &&
      taskResult.node <= max
    )
  }

  async function getObservationTaskResults() {
    isLoading.value = true
    await new Promise((resolve) => setTimeout(resolve, 200))
    taskResults.value = psStore.ps?.observationTaskResults

    const search = searchString.value
    const satellites = selectedSatellites.value
    const left = isLeft.value
    const right = isRight.value
    const min = nodes.value[0]
    const max = nodes.value[1]

    if (search === '') {
      filteringTaskResults.value = taskResults.value
    }

    filteringTaskResults.value = taskResults.value?.filter((s) =>
      filtering(s, search, satellites, left, right, min, max)
    )
    isLoading.value = false
  }

  async function reset() {
    if (isLoading.value == false) {
      nodes.value = [minNode.value, maxNode.value]
      isLeft.value = true
      isRight.value = true
      selectedSatellites.value = [...satellites.value.map((s) => s.name)]

      getObservationTaskResults()
    }
  }

  return {
    taskResults,
    filteringTaskResults,
    isLoading,
    isDirty,
    searchString,
    getObservationTaskResults,
    // filter
    reset,
    nodes,
    isLeft,
    isRight,
    satellites,
    selectedSatellites
  }
})
