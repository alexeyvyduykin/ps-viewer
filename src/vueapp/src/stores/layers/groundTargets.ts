/* eslint-disable @typescript-eslint/no-unused-vars */
import { ref, watchEffect } from 'vue'
import { defineStore, storeToRefs } from 'pinia'
import myFetch from '@/utils/fetch'
import type { FeatureCollection, GeoJsonObject } from 'geojson'
import type { FeatureMap, GroundTarget } from '@/types'
import { useGroundTargetStore } from '@/stores/groundTargets'

const url = 'api/feature/getgts'

export const useGroundTargetLeafletStore = defineStore('graundTargetLeaflet', () => {
  const gtStore = useGroundTargetStore()
  const { gts } = storeToRefs(gtStore)
  const data = ref<FeatureCollection>()
  const data0 = ref<FeatureCollection | undefined>()
  const cache = ref<FeatureMap | undefined>()

  watchEffect(async () => await initState(gts.value))

  async function initState(gts: GroundTarget[] | undefined) {
    if (cache.value === undefined) {
      await getGts()
      data.value = cache.value!['GT']
    }

    if (gts) {
      data0.value = {
        type: 'FeatureCollection',
        features: gts.map(
          (s) =>
            ({
              type: 'Point',
              coordinates: s.center.coordinates
            }) as GeoJsonObject
        )
      } as FeatureCollection
    }
  }

  async function getGts() {
    await myFetch(url).then((res) => {
      cache.value = res.response.value as FeatureMap
    })
  }

  async function update() {
    if (cache.value === undefined) {
      await getGts()
      data.value = cache.value!['GT']
    }
  }

  return {
    cache,
    data,
    data0,
    update
  }
})
