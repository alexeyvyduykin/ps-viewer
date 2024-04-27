import { ref, watch } from 'vue'
import { useTaskResultStore } from '@/stores/taskResults'
import { useSatelliteStore } from '@/stores/satellites'
import { storeToRefs } from 'pinia'
import { whenever } from '@vueuse/core'
import { type Group, type Line, type Segment } from 'timelines-chart'
import { getFromInterval } from '@/utils'

export function useChartTimelines() {
  const taskResultStore = useTaskResultStore()
  const { taskResults } = storeToRefs(taskResultStore)
  const satelliteStore = useSatelliteStore()
  const { satellites: sats } = storeToRefs(satelliteStore)
  const satellites = ref<string[] | undefined>()
  const data = ref<Group[] | undefined>()
  const isLoading = ref(true)
  const selectedSatellites = ref<string[]>([])

  whenever(satellites, (sats) => {
    selectedSatellites.value = sats
  })

  whenever(selectedSatellites, (sats) => {
    data.value = getData(sats)
  })

  async function update() {
    if (taskResults.value === undefined) {
      await taskResultStore.getObservationTaskResults()
    }

    if (sats.value === undefined) {
      await satelliteStore.getSatellites()
    }

    isLoading.value = false
    satellites.value = [...(sats.value?.map((s) => s.name) ?? [])]

    data.value = getData()
  }

  function getData(selectedSatellites?: string[]): Group[] {
    return satellites.value
      .filter(function (item) {
        return selectedSatellites?.includes(item) ?? true
      })
      .map((s) => {
        return {
          group: s,
          data: getGroupData(s)
        } as Group
      })
  }

  function getSegmentsData(satelliteName: string): Segment[] {
    return (
      taskResults.value
        ?.filter((s) => s.satelliteName === satelliteName)
        .map((item) => {
          //const taskName = item.taskName;
          const { begin, end } = getFromInterval(item.interval)
          return {
            timeRange: [begin, end],
            val: 'Observation' // taskName,
            //labelVal: is optional - only displayed in the labels
          } as Segment
        }) ?? []
    )
  }

  function getGroupData(satelliteName: string): Line[] {
    const line = {
      label: 'Label1', // "Observation",
      data: getSegmentsData(satelliteName)
    } as Line

    return [line]
  }

  return {
    data,
    isLoading,
    satellites,
    selectedSatellites,
    update
  }
}
