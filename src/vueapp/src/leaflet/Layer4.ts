/* eslint-disable @typescript-eslint/no-unused-vars */
import L, { LatLng } from 'leaflet'
import type { GeoJSONOptions, Polyline } from 'leaflet'
import type {
  GeometryObject,
  GeoJsonObject,
  MultiPolygon,
  FeatureCollection,
  LineString,
  Feature
} from 'geojson'
import { ref, toValue, isProxy, watch, toRaw, watchEffect, type Ref, computed } from 'vue'
import { useGroundTargetLeafletStore } from '@/stores/layers/groundTargets'
import { storeToRefs } from 'pinia'
import { whenever } from '@vueuse/core'
import { type ColorRect, createAngles } from '@/composables/groundStationHelper'
import { useFromColorHuePalette } from '@/composables/palettes'
import { useMapStore } from '@/stores/map'
import { usePreviewStore } from '@/stores/layers/preview'
import { usePaletteStore } from '@/stores/palette'
import 'leaflet-arrowheads'

export default class Layer4<P = any, G extends GeometryObject = GeometryObject> extends L.GeoJSON<
  P,
  G
> {
  constructor(geojson?: GeoJsonObject, options?: GeoJSONOptions<P, G>) {
    super(geojson, options)

    this.options = {
      style: (f) => this._getStyle(f, pickColor)
      //onEachFeature: this.ff1,
      //arrowheads: { yawn: 40, fill: true },
    }

    const store = usePreviewStore()
    const paletteStore = usePaletteStore()
    const { pickColor } = paletteStore
    const { /*data,*/ dataFootprint /*dataTrack */ } = storeToRefs(store)

    // whenever(data, (data) => {
    //   this.clearLayers();
    //   for (const item of data) {
    //     this.addData(item);
    //   }
    //   this.resetStyle();
    // });

    whenever(dataFootprint, (data) => {
      this.clearLayers()

      //   const layer0 = L.geoJSON(dataTrack.value, {
      //     arrowheads: { yawn: 40, fill: true }
      //   } as L.GeoJSONOptions)

      //   const fg = L.featureGroup([layer0], {
      //     arrowheads: { yawn: 40, fill: true }
      //   } as L.GeoJSONOptions)

      //      for (const item of data) {
      //        this.addData(item)
      //     }

      //   this.addData(fg.toGeoJSON())

      this.resetStyle()
    })
  }

  //   whenever(dataFootprint, (data) => {
  //     this.clearLayers();
  //     for (const item of data) {
  //       this.addData(item);
  //     }

  //     const line = dataTrack.value?.features[0].geometry as LineString;
  //     const arrow = L.polyline(
  //       line.coordinates.slice(-2).map((s) => new LatLng(s[1], s[0])),
  //     ).arrowheads({
  //       yawn: 40,
  //       fill: true,
  //     });
  //     const f2 = arrow.toGeoJSON();
  //     f2.properties["Feature"] = "Arrow";
  //     this.addData(f2);

  //     this.resetStyle();
  //   });
  // }

  ff1(feature: Feature, layer: any) {
    if (feature.properties?.Feature) {
      if (feature.properties.Feature === 'SegmentTrack') {
        if (feature.geometry.type == 'LineString') {
          const arrow = L.polyline(layer.getLatLngs()) //.arrowheads();
          layer.addData(arrow)
          //(feature.geometry as Polyline).arrowheads().addTo(layer);
        }

        //const polyline = L.polyline(layer.getLatLngs()).arrowheads().addTo(this);
        // const arrowHead = L.polylineDecorator(feature, {
        //   patterns: [
        //     {
        //       offset: "100%",
        //       repeat: 0,
        //       symbol: L.Symbol.arrowHead({
        //         pixelSize: 60,
        //         polygon: false,
        //         pathOptions: { stroke: true },
        //       }),
        //     },
        //   ],
        // }).addTo(this);
      }
    }
  }

  _getStyle(feature: any, pick: (key: string) => string): any {
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

      if (feature.properties.Feature === 'Footprint') {
        return {
          fillColor: 'green',
          weight: 1,
          opacity: 1,
          color: 'green',
          fillOpacity: 0.35
        }
      }

      if (feature.properties.Feature === 'SegmentTrack') {
        return {
          weight: 12,
          opacity: 0.35,
          color: pick(sat),
          lineCap: 'square'
        }
      }
      if (feature.properties.Feature === 'Arrow') {
        return {}
      }
      return {
        fillColor: 'blue',
        weight: 2,
        opacity: 1,
        color: 'blue',
        fillOpacity: 0.35
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
}
