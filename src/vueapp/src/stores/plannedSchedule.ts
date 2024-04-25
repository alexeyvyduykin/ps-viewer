import { ref } from 'vue'
import type { ObservationTaskResult, PlannedScheduleObject } from '@/types'
import { defineStore } from 'pinia'
import myFetch from '@/utils/fetch'

//import mydata from "/public/demo.json";
//import { useQuery } from '@tanstack/vue-query'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const defaultUrl = 'api/ps/getps'

export const usePlannedScheduleStore = defineStore('plannedSchedule', () => {
  const ps = ref<PlannedScheduleObject | undefined>()
  const isLoading = ref(true)
  const isValid = ref(false)
  const url = ref()

  const minNode = ref(1)
  const maxNode = ref(2)
  const satelliteNames = ref<string[] | undefined>()
  const gsNames = ref<string[] | undefined>()

  //const enabled = computed(() => !!url.value)

  async function getPlannedSchedule(url?: string) {
    isLoading.value = true
    myFetch(url ?? defaultUrl).then((res) => {
      const data = res.response.value as PlannedScheduleObject
      ps.value = data

      const arr = data.observationTaskResults
      minNode.value = getMinNode(arr)
      maxNode.value = getMaxNode(arr)

      satelliteNames.value = data.satellites.map((s) => s.name)
      gsNames.value = data.groundStations.map((s) => s.name)

      isLoading.value = false
    })
  }

  function getMinNode(arr: ObservationTaskResult[]): number {
    return arr.reduce((prev, curr) => (prev.node < curr.node ? prev : curr)).node
  }

  function getMaxNode(arr: ObservationTaskResult[]): number {
    return arr.reduce((prev, curr) => (prev.node > curr.node ? prev : curr)).node
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  //   const { isPending, isFetching, isError, data, error } = useQuery({
  //     queryKey: ['ps', url],
  //     queryFn: fetchPs,
  //     enabled: enabled,
  //     gcTime: 1000 * 10 // 10sec
  //   })

  //   async function fetchPs({ queryKey }) {
  //     const response = await fetch(queryKey[1])
  //     if (!response.ok) {
  //       throw new Error('Network response was not ok')
  //     }
  //     return response.json().then((res) => (ps.value = res))
  //   }

  // async function getPlannedSchedule(url?: string) {
  //   const path = url ?? defaultUrl;

  //   const { isPending, isFetching, isError, data, error } = useQuery({
  //     queryKey: ["ps", path],
  //     queryFn: fetchPs,
  //   });

  //   async function fetchPs({ queryKey }) {
  //     const response = await fetch(queryKey[1]);
  //     if (!response.ok) {
  //       throw new Error("Network response was not ok");
  //     }
  //     return response.json();
  //   }
  //   watch(data, (raw) => {
  //     ps.value = raw as PlannedScheduleObject;
  //     init();
  //   });
  // }

  async function validation(url: string) {
    isValid.value = false
    myFetch(url).then((res) => {
      const data = res.response.value as boolean
      console.log(data)
      isValid.value = data
      console.log(isValid.value)
    })
  }

  function getPlannedScheduleFromJson(json: string) {
    isLoading.value = true

    const parsed = JSON.parse(json)

    if (isType(parsed)) {
      ps.value = parsed

      const arr = parsed.observationTaskResults
      minNode.value = getMinNode(arr)
      maxNode.value = getMaxNode(arr)

      satelliteNames.value = parsed.satellites.map((s) => s.name)
      gsNames.value = parsed.groundStations.map((s) => s.name)

      //init();
    } else {
      // error handling; invalid JSON format
    }

    isLoading.value = false
  }

  function clear() {
    ps.value = undefined
  }

  function isType(obj: any): obj is PlannedScheduleObject {
    return (
      'name' in obj &&
      'dateTime' in obj &&
      'satellites' in obj &&
      'groundTargets' in obj &&
      'groundStations' in obj &&
      'tasks' in obj &&
      'taskResults' in obj
    )
  }

  return {
    ps,
    isLoading,
    isValid,
    url,
    minNode,
    maxNode,
    satelliteNames,
    gsNames,
    getPlannedSchedule,
    getPlannedScheduleFromJson,
    validation,
    clear
  }
})
