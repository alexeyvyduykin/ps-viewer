/* eslint-disable @typescript-eslint/no-unused-vars */
import L from 'leaflet'
import type { LeafletMouseEvent, GeoJSONOptions } from 'leaflet'
import type { GeometryObject, GeoJsonObject, FeatureCollection, Feature, Geometry } from 'geojson'
import {
  computed,
  ref,
  watch,
  onMounted,
  onUnmounted,
  toValue,
  isProxy,
  toRaw,
  type Ref
} from 'vue'
import { useTrackStore, type TrackFeature } from '@/stores/layers/tracks'
import { storeToRefs } from 'pinia'
import { whenever } from '@vueuse/core'
import { usePaletteStore } from '@/stores/palette'

let _geojson: L.GeoJSON | undefined

export default class Layer1<P = any, G extends GeometryObject = GeometryObject> extends L.GeoJSON<
  P,
  G
> {
  private _dataset: string | undefined
  private _data: any | undefined
  private _property: string | null = null
  //private _geojson: L.GeoJSON | undefined;

  constructor(geojson?: GeoJsonObject, options?: GeoJSONOptions<P, G>) {
    super(geojson, options)

    const store = useTrackStore()
    const paletteStore = usePaletteStore()
    const { pickColor } = paletteStore
    const { data } = storeToRefs(store)

    this.options = {
      style: (f) => this._getStyle(f, pickColor),
      interactive: false
    }

    whenever(data, (data) => {
      this.clearLayers()
      for (const item of data) {
        this.addData(item)
      }
      this.resetStyle()
      //console.log(data);
    })

    // _geojson = L.geoJson(this._data, {
    //   style: (f) => this._getStyle(f),
    // });
  }

  _getStyle(feature: any, pick: (key: string) => string) {
    let color = 'red'

    if (feature.properties.Satellite !== undefined) {
      const key = feature.properties.Satellite
      color = pick(key)
    }

    return {
      fillColor: color,
      weight: 1,
      opacity: 1,
      color: color,
      fillOpacity: 0.4
    }
  }

  private toData<T>(value: Ref<T> | T): T {
    let val = toValue(value)
    if (isProxy(val) == true) {
      val = toRaw(val)
    }
    return val
  }
}
