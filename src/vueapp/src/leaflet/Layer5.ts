/* eslint-disable @typescript-eslint/no-unused-vars */
import L from 'leaflet'
import { watch } from 'vue'
import { useGroundTargetLeafletStore } from '@/stores/layers/groundTargets'
import { storeToRefs } from 'pinia'
import { useMapStore } from '@/stores/map'
import { usePreviewStore } from '@/stores/layers/preview'
import { usePaletteStore } from '@/stores/palette'
//import 'leaflet-arrowheads'

export default class Layer5 extends L.FeatureGroup {
  private _layerFootprint: L.GeoJSON
  private _layerPreviewSwath: L.GeoJSON
  private _layerPreviewTrack: L.GeoJSON
  private _layerPreviewIntervalTrack: L.GeoJSON

  constructor(layers?: L.Layer[], options?: L.LayerOptions) {
    super(layers, options)

    this._layerFootprint = L.geoJSON(undefined, {
      style: (f) => this._getStyleFootprint(f)
    } as L.GeoJSONOptions)

    this._layerPreviewSwath = L.geoJSON(undefined, {
      style: (f) => this._getStylePreviewSwath(f, pickColor),
      interactive: false
    } as L.GeoJSONOptions)

    this._layerPreviewTrack = L.geoJSON(undefined, {
      style: (f) => this._getStylePreviewTrack(f, pickColor),
      interactive: false,
      arrowheads: {
        yawn: 40,
        size: '20px',
        fill: true,
        frequency: 'endonly',
        proportionalToTotal: true
      }
    } as L.GeoJSONOptions)

    this._layerPreviewIntervalTrack = L.geoJSON(undefined, {
      style: (f) => this._getStylePreviewIntervalTrack(f, pickColor),
      interactive: false
    } as L.GeoJSONOptions)

    this.addLayer(this._layerPreviewSwath)
    this.addLayer(this._layerPreviewIntervalTrack)
    this.addLayer(this._layerPreviewTrack)
    this.addLayer(this._layerFootprint)

    const store = usePreviewStore()
    const paletteStore = usePaletteStore()
    const { pickColor } = paletteStore
    const { dataFootprint, dataPreviewTrack, dataPreviewIntervalTrack, dataPreviewSwath } =
      storeToRefs(store)

    watch(dataFootprint, (data) => {
      this._layerFootprint.clearLayers()
      if (data) {
        this._layerFootprint.addData(data)
      }
      this._layerFootprint.resetStyle()
    })

    watch(dataPreviewSwath, (data) => {
      this._layerPreviewSwath.clearLayers()
      if (data) {
        this._layerPreviewSwath.addData(data)
      }
      this._layerPreviewSwath.resetStyle()
    })

    watch(dataPreviewTrack, (data) => {
      this._layerPreviewTrack.clearLayers()
      if (data) {
        this._layerPreviewTrack.addData(data)
      }
      this._layerPreviewTrack.resetStyle()
    })

    watch(dataPreviewIntervalTrack, (data) => {
      this._layerPreviewIntervalTrack.clearLayers()
      if (data) {
        this._layerPreviewIntervalTrack.addData(data)
      }
      this._layerPreviewIntervalTrack.resetStyle()
    })
  }

  _getStylePreviewTrack(feature: any, pick: (key: string) => string): any {
    if (feature.properties.Satellite) {
      const sat = feature.properties.Satellite
      return {
        weight: 12,
        opacity: 0.35,
        color: pick(sat),
        lineCap: 'square'
      }
    }

    return {
      fillColor: 'red',
      weight: 1,
      opacity: 1,
      color: 'red',
      fillOpacity: 0.65
    }
  }

  _getStyleFootprint(feature: any): any {
    if (feature.properties.Feature) {
      if (feature.properties.Feature === 'Footprint') {
        return {
          fillColor: 'green',
          weight: 1,
          opacity: 1,
          color: 'green',
          fillOpacity: 0.35
        }
      }
    }

    return {
      fillColor: 'red',
      weight: 1,
      opacity: 1,
      color: 'red',
      fillOpacity: 0.65
    }
  }

  _getStylePreviewSwath(feature: any, pick: (key: string) => string): any {
    if (feature.properties.Feature) {
      const sat = feature.properties.Satellite
      if (feature.properties.Feature === 'SegmentSwath') {
        return {
          weight: 2,
          opacity: 1,
          color: pick(sat)
        }
      }

      if (feature.properties.Feature === 'SegmentAreaSwath') {
        return {
          fillColor: pick(sat),
          weight: 0,
          fillOpacity: 0.2
        }
      }
    }

    return {
      fillColor: 'red',
      weight: 1,
      opacity: 1,
      color: 'red',
      fillOpacity: 0.65
    }
  }

  _getStylePreviewIntervalTrack(feature: any, pick: (key: string) => string): any {
    if (feature.properties.Satellite) {
      const sat = feature.properties.Satellite
      return {
        weight: 12,
        opacity: 0.85,
        color: pick(sat),
        lineCap: 'square'
      }
    }
    return {
      weight: 12,
      opacity: 0.85,
      color: 'green',
      lineCap: 'square'
    }
  }
}
