/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, reactive, watch, computed, watchEffect } from 'vue'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import type { FeatureCollection } from 'geojson'
import { whenever } from '@vueuse/core'
import type { Dictionary, NodeFeatureMap } from '@/types'
import { usePlannedScheduleStore } from '@/stores/plannedSchedule'

const urlTracks = 'api/feature/gettracks/'
const urlSwaths = 'api/feature/getswaths/'

export interface TrackFeature {
  node: number
  features: FeatureCollection
}

export interface SwathFeature {
  node: number
  features: FeatureCollection[]
}

export enum SatelliteFeatureType {
  Track = 1,
  IntervalTrack = 2,
  Left = 4,
  Right = 8
}

export interface SatelliteState {
  name: string
  visible: boolean
  isTrack: boolean
  isLeftSwath: boolean
  isRightSwath: boolean
  node: number
}

export interface TrackDictionary {
  [key: string]: NodeFeatureMap
}

export interface SwathDictionary {
  [key: string]: NodeFeatureMap
}

export const useTrackStore = defineStore('tracks', () => {
  const psStore = usePlannedScheduleStore()
  const { minNode, maxNode, satelliteNames } = storeToRefs(psStore)
  const states = reactive<Dictionary<SatelliteState>>({})
  const data = ref<FeatureCollection[]>([])
  const cache = reactive<TrackDictionary>({})
  const swathCache = reactive<SwathDictionary>({})

  const allNodesFormat = computed(() => `${minNode.value}+${maxNode.value}`)

  watchEffect(() => initState(satelliteNames.value))

  function initState(names: string[] | undefined) {
    for (const key in states) {
      delete states[key]
    }

    if (names) {
      for (const item of names) {
        states[item] = {
          name: item,
          visible: false,
          isTrack: true,
          isLeftSwath: false,
          isRightSwath: false,
          node: 1
        }
      }
    }
  }

  async function getTracks(name: string) {
    const path = urlTracks + name + '/' + allNodesFormat.value + '/' + '?hasIntervals=true'
    await myFetch(path).then((res) => {
      cache[name] = res.response.value as NodeFeatureMap
    })
  }

  async function getSwaths(name: string) {
    const path = urlSwaths + name + '/' + allNodesFormat.value
    await myFetch(path).then((res) => {
      swathCache[name] = res.response.value as NodeFeatureMap
    })
  }

  watch(states, async () => {
    await update()
  })

  async function update() {
    const res: FeatureCollection[] = []

    for (const key in states) {
      const item = states[key]
      if (item.visible === true && item.isTrack === true) {
        if (cache[key] === undefined) {
          await getTracks(key)
        }
        res.push(cache[key][item.node][SatelliteFeatureType[SatelliteFeatureType.Track]])
      }
      if (item.visible === true && (item.isLeftSwath === true || item.isRightSwath === true)) {
        if (swathCache[key] === undefined) {
          await getSwaths(key)
        }

        if (item.isLeftSwath === true) {
          res.push(swathCache[key][item.node][SatelliteFeatureType[SatelliteFeatureType.Left]])
        }

        if (item.isRightSwath === true) {
          res.push(swathCache[key][item.node][SatelliteFeatureType[SatelliteFeatureType.Right]])
        }
      }
    }

    if ((data.value.length === 0 && res.length === 0) === false) {
      data.value = res
    }
  }

  async function getTrack(name: string, node: number): Promise<FeatureCollection> {
    if (cache[name] === undefined) {
      await getTracks(name)
    }

    return cache[name][node][SatelliteFeatureType[SatelliteFeatureType.Track]]
  }

  async function getIntervalTrack(name: string, node: number): Promise<FeatureCollection> {
    if (cache[name] === undefined) {
      await getTracks(name)
    }

    return cache[name][node][SatelliteFeatureType[SatelliteFeatureType.IntervalTrack]]
  }

  return {
    cache,
    swathCache,
    states,
    data,
    getTrack,
    getIntervalTrack
  }
})
