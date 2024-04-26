/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, computed } from 'vue'
import type { GroundTarget } from '@/types'
import { GroundTargetType } from '@/types'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import { usePlannedScheduleStore } from './plannedSchedule'

const url = 'api/ps/getgts'

export const useGroundTargetStore = defineStore('groundTargets', () => {
  const psStore = usePlannedScheduleStore()
  const gts = ref<GroundTarget[] | undefined>()
  const isLoading = ref(true)
  const filteringGts = ref<GroundTarget[] | undefined>()
  const searchString = ref('')

  function update(signal: AbortSignal): void {
    fetch(url, { signal: signal })
      .then((response) => response.json())
      .then((data) => (gts.value = data))
  }

  type GroundTargetItem = {
    name: string
    key: string
  }

  const types = computed(
    () =>
      Object.values(GroundTargetType)
        .filter((v) => isNaN(Number(v)))
        .map(
          (s, i) =>
            ({
              name: `${s}`,
              key: `${i}`
            }) as GroundTargetItem
        ) ?? []
  )

  const selectedTypes = ref([...types.value.map((s) => s.name)])

  const isDirty = computed(() => {
    return !(selectedTypes.value.length === types.value.length)
  })

  function filtering(gt: GroundTarget, str: string, types: string[]) {
    return gt.name.includes(str) && types.includes(`${GroundTargetType[gt.type]}`)
  }

  async function getGroundTargets() {
    isLoading.value = true
    await new Promise((resolve) => setTimeout(resolve, 200))
    gts.value = psStore.ps?.groundTargets

    const search = searchString.value
    const selTypes = selectedTypes.value

    if (search === '') {
      filteringGts.value = gts.value
    }

    filteringGts.value = gts.value?.filter((s) => filtering(s, search, selTypes))
    isLoading.value = false
  }

  async function reset() {
    if (isLoading.value == false) {
      selectedTypes.value = [...types.value.map((s) => s.name)]
      getGroundTargets()
    }
  }

  return {
    gts,
    filteringGts,
    isLoading,
    isDirty,
    getGroundTargets,
    reset,
    searchString,
    types,
    selectedTypes
  }
})
