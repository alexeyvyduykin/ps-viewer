/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, reactive, watch, computed } from 'vue'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import type { FeatureCollection } from 'geojson'
import { usePsHelperStore } from '@/stores/psHelper'
import type { FeatureMap, NodeFeatureMap } from '@/types'
import { useTrackStore } from './tracks'
import { usePlannedScheduleStore } from '@/stores/plannedSchedule'

const url = 'api/feature/getpreview/'

export interface TrackFeature {
  node: number
  features: FeatureCollection
}

export enum SatelliteFeatureType {
  Track = 1,
  IntervalTrack = 2,
  Left = 4,
  Right = 8
}

export enum PreviewFeatureType {
  Footprint = 1,
  PreviewTrack = 2,
  PreviewIntervalTrack = 4,
  PreviewSwath = 8
}

export interface SatelliteState {
  name: string
  visible: boolean
  isTrack: boolean
  node: number
}

export interface TrackDictionary {
  [key: string]: NodeFeatureMap
}

export interface PreviewDictionary {
  [key: string]: FeatureMap
}

export const usePreviewStore = defineStore('preview', () => {
  const psStore = usePlannedScheduleStore()
  const pshStore = usePsHelperStore()
  const trackStore = useTrackStore()
  const { minNode, maxNode } = storeToRefs(psStore)
  const { getTrack, getIntervalTrack } = trackStore
  const { getObservationTaskInfo } = pshStore
  const dataFootprint = ref<FeatureCollection | undefined>()
  const dataPreviewTrack = ref<FeatureCollection | undefined>()
  const dataPreviewIntervalTrack = ref<FeatureCollection | undefined>()
  const dataPreviewSwath = ref<FeatureCollection | undefined>()
  const previewCache = reactive<PreviewDictionary>({})

  const allNodesFormat = computed(() => `${minNode.value}+${maxNode.value}`)

  const states = ref(['Footprint', 'Track', 'Swath'])
  const trackState = ref('Segment')

  const hasFootprint = computed(() => states.value.some((s) => s === 'Footprint'))
  const hasTrack = computed(() => states.value.some((s) => s === 'Track'))
  const hasSwath = computed(() => states.value.some((s) => s === 'Swath'))
  const isFullTrack = computed(() => trackState.value === 'Full')

  const currentObservationTask = ref()

  async function getPreview(observationTaskName: string) {
    const path =
      url +
      observationTaskName +
      '/' +
      '?hasFootprint=true' +
      '&hasPreviewTrack=true' +
      '&hasPreviewSwath=true'
    await myFetch(path).then((res) => {
      previewCache[observationTaskName] = res.response.value as FeatureMap
    })
  }

  watch(hasFootprint, async () => {
    if (currentObservationTask.value) {
      preview(currentObservationTask.value)
    }
  })

  watch(hasTrack, async () => {
    if (currentObservationTask.value) {
      preview(currentObservationTask.value)
    }
  })

  watch(hasSwath, async () => {
    if (currentObservationTask.value) {
      preview(currentObservationTask.value)
    }
  })

  watch(isFullTrack, async () => {
    if (currentObservationTask.value) {
      preview(currentObservationTask.value)
    }
  })

  async function preview(observationTaskName: string) {
    currentObservationTask.value = observationTaskName

    if (previewCache[observationTaskName] === undefined) {
      await getPreview(observationTaskName)
    }

    if (hasFootprint.value) {
      dataFootprint.value =
        previewCache[observationTaskName][PreviewFeatureType[PreviewFeatureType.Footprint]]
    } else {
      dataFootprint.value = undefined
    }

    if (hasTrack.value) {
      if (trackState.value === 'Segment') {
        dataPreviewTrack.value =
          previewCache[observationTaskName][PreviewFeatureType[PreviewFeatureType.PreviewTrack]]
        dataPreviewIntervalTrack.value =
          previewCache[observationTaskName][
            PreviewFeatureType[PreviewFeatureType.PreviewIntervalTrack]
          ]
      } else if (trackState.value === 'Full') {
        const { satelliteName, node } = getObservationTaskInfo(observationTaskName)
        const data1 = await getTrack(satelliteName, node)
        const data2 = await getIntervalTrack(satelliteName, node)
        dataPreviewTrack.value = data1
        dataPreviewIntervalTrack.value = data2
      }
    } else {
      dataPreviewTrack.value = undefined
      dataPreviewIntervalTrack.value = undefined
    }

    if (hasSwath.value) {
      dataPreviewSwath.value =
        previewCache[observationTaskName][PreviewFeatureType[PreviewFeatureType.PreviewSwath]]
    } else {
      dataPreviewSwath.value = undefined
    }
  }

  return {
    allNodesFormat,
    states,
    trackState,
    previewCache,
    dataFootprint,
    dataPreviewTrack,
    dataPreviewIntervalTrack,
    dataPreviewSwath,
    preview
  }
})
