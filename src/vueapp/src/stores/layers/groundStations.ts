/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, reactive, watch, watchEffect } from 'vue'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import type { FeatureCollection } from 'geojson'
import { useGroundStationStore } from '@/stores/groundStations'
import type { FeatureMap, GroundStation } from '@/types'
import { type ColorRect, createAngles } from '@/composables/groundStationHelper'

const url = 'api/feature/getgs/'
//const url2 = "api/features/gss/v2/";

export interface GroundStationState {
  name: string
  visible: boolean
  rangeAngle: number[]
  areaCount: number
  countMode: GroundStationAreaMode
  defaultAngles: number[]
}

export interface Dictionary<T> {
  [key: string]: T
}

export enum GroundStationAreaMode {
  None = 'None',
  Equal = 'Equal',
  Geometric = 'Geometric'
}

export const useGroundStationLeafletStore = defineStore('graundStationLeaflet', () => {
  const gsStore = useGroundStationStore()
  const { gss } = storeToRefs(gsStore)
  const states = reactive<Dictionary<GroundStationState>>({})
  const data = ref<FeatureCollection[]>([])
  const cache = reactive<Dictionary<FeatureMap>>({})
  const test = ref()

  watchEffect(() => initState(gss.value))

  function initState(gss: GroundStation[] | undefined) {
    for (const key in states) {
      delete states[key]
    }

    if (gss) {
      for (const item of gss) {
        const angles = item.angles
        const len = angles.length
        states[item.name] = {
          name: item.name,
          visible: false,
          rangeAngle: [angles[0], angles[len - 1]],
          areaCount: len - 1,
          countMode: GroundStationAreaMode.None,
          defaultAngles: angles
        }
      }
    }
  }

  async function getGss(name: string) {
    const path = url + name
    await myFetch(path).then((res) => {
      cache[name] = res.response.value as FeatureMap
    })
  }

  async function getGss2(name: string, angles: number[]) {
    const params = new URLSearchParams(angles.map((s) => ['angles', s.toString()]))
    const str = params.toString()
    const path = url + name + '?' + str

    await myFetch(path).then((res) => {
      cache[name] = res.response.value as FeatureMap
    })
  }

  watch(states, async () => {
    await update()
  })

  async function update() {
    const res: FeatureCollection[] = []

    for (const key in states) {
      const item = states[key]
      if (item.visible === true) {
        //if (cache[key] === undefined) {
        const inner = item.rangeAngle[0]
        const outer = item.rangeAngle[1]
        const count = item.areaCount
        const mode = item.countMode

        const angles =
          mode === GroundStationAreaMode.None
            ? item.defaultAngles
            : createAngles(inner, outer, count, mode)

        if (mode !== GroundStationAreaMode.None) {
          angles.unshift(inner)
        }

        await getGss2(key, angles)
        //}
        res.push(cache[key]['GS'])
      }
    }

    if ((data.value.length === 0 && res.length === 0) === false) {
      data.value = res
    }
  }

  function resetToDefault(name: string) {
    const oldState = states[name]
    const angles = oldState.defaultAngles
    const len = angles.length
    // states[name] = {
    //   name: oldState.name,
    //   visible: oldState.visible,
    //   rangeAngle: [angles[0], angles[len - 1]],
    //   areaCount: len - 1,
    //   countMode: GroundStationAreaMode.None,
    //   defaultAngles: oldState.defaultAngles,
    // };
  }

  return {
    test,
    cache,
    states,
    data,
    resetToDefault
  }
})
