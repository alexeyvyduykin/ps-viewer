/* eslint-disable @typescript-eslint/no-unused-vars */
import L from 'leaflet'
import type { GeoJSONOptions } from 'leaflet'
import type { GeometryObject, GeoJsonObject, MultiPolygon, FeatureCollection } from 'geojson'
import { ref, toValue, isProxy, watch, toRaw, watchEffect, type Ref, computed } from 'vue'
import { useGroundTargetLeafletStore } from '@/stores/layers/groundTargets'
import { storeToRefs } from 'pinia'
import { whenever } from '@vueuse/core'
import { type ColorRect, createAngles } from '@/composables/groundStationHelper'
import { useFromColorHuePalette } from '@/composables/palettes'
import { useMapStore } from '@/stores/map'

export default class Layer3<P = any, G extends GeometryObject = GeometryObject> extends L.GeoJSON<
  P,
  G
> {
  constructor(geojson?: GeoJsonObject, options?: GeoJSONOptions<P, G>) {
    super(geojson, options)

    this.options = {
      style: (f) => this._getStyle(f)
    }

    this.options.pointToLayer = (feature, latlng) => {
      return L.circleMarker(latlng, {
        radius: 4,
        fillColor: '#ff7800',
        color: '#000',
        weight: 1,
        opacity: 1,
        fillOpacity: 0.8
      })
    }

    const mapStore = useMapStore()
    const store = useGroundTargetLeafletStore()

    const { currentZoom } = storeToRefs(mapStore)
    const { data, data0 } = storeToRefs(store)

    const compact = computed(() => (currentZoom.value && currentZoom.value < 6) ?? undefined)

    watch(compact, (compact) => {
      if (compact !== undefined) {
        const res = compact === true ? data0.value : data.value
        this.clearLayers()
        this.addData(res as FeatureCollection)
        this.resetStyle()
      }
    })
  }

  _getStyle(feature?: any) {
    return {
      fillColor: 'red',
      weight: 2,
      opacity: 1,
      color: 'red',
      fillOpacity: 0.45
    }
  }
}
