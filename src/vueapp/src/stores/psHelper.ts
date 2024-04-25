import { defineStore, storeToRefs } from 'pinia'
import { usePlannedScheduleStore } from './plannedSchedule'

export const usePsHelperStore = defineStore('psHelper', () => {
  const psStore = usePlannedScheduleStore()
  const { ps } = storeToRefs(psStore)

  function getObservationTaskInfo(name: string): { satelliteName: string; node: number } {
    if (ps.value) {
      const { satelliteName, node } = ps.value.observationTaskResults.filter(
        (s) => s.taskName === name
      )[0]!
      return {
        satelliteName: satelliteName,
        node: node
      }
    }
    throw 'Not handled error -> psHelperStore -> getObservationTaskInfo'
  }

  return { getObservationTaskInfo }
})
