/* eslint-disable @typescript-eslint/no-unused-vars */
import L from 'leaflet'
import type { GeoJSONOptions } from 'leaflet'
import type { GeometryObject, GeoJsonObject, MultiPolygon } from 'geojson'
import { toValue, isProxy, toRaw, type Ref } from 'vue'
import { useGroundStationLeafletStore } from '@/stores/layers/groundStations'
import { storeToRefs } from 'pinia'
import { whenever } from '@vueuse/core'
import { type ColorRect, createAngles } from '@/composables/groundStationHelper'
import { useFromColorHuePalette } from '@/composables/palettes'

let _geojson: L.GeoJSON | undefined

export default class Layer2<P = any, G extends GeometryObject = GeometryObject> extends L.GeoJSON<
  P,
  G
> {
  private _dataset: string | undefined
  private _data: any | undefined
  private _property: string | null = null
  //private _geojson: L.GeoJSON | undefined;

  constructor(geojson?: GeoJsonObject, options?: GeoJSONOptions<P, G>) {
    super(geojson, options)

    this.options = {
      style: (f) => this._getStyle(f)
    }

    const store = useGroundStationLeafletStore()

    const { data } = storeToRefs(store)

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

  _getStyle(feature?: any) {
    if (feature.properties.Count && feature.properties.Index) {
      const count = feature.properties.Count
      const index = feature.properties.Index
      const color = useFromColorHuePalette(index, count)

      return {
        fillColor: color,
        weight: 0,
        fillOpacity: 0.45
        //color: color,
      }
    }
    if (feature.properties.Count && feature.properties.InnerBorder) {
      const count = feature.properties.Count

      return {
        weight: 2,
        opacity: 1,
        color: useFromColorHuePalette(0, count)
      }
    }

    if (feature.properties.Count && feature.properties.OuterBorder) {
      const count = feature.properties.Count

      return {
        weight: 2,
        opacity: 1,
        color: useFromColorHuePalette(count - 1, count)
      }
    }

    return {
      fillColor: 'red',
      weight: 3,
      opacity: 1,
      color: 'red',
      fillOpacity: 1
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
