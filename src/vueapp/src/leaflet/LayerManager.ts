import * as L from 'leaflet'
import { MyLayerControl } from './controls/MyLayerControl'

export class LayerManager {
  private readonly layerControl: MyLayerControl
  private readonly map: L.Map

  constructor(map: L.Map) {
    this.map = map
    this.layerControl = new MyLayerControl(
      {},
      {},
      {
        //position: "topleft",
      }
    )

    const base = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '� OpenStreetMap'
    }).addTo(map)

    this.layerControl.addBaseLayer(base, 'OpenStreetMap')

    this.map.addControl(this.layerControl)
  }

  addLayer(layer: L.Layer, showInControl: boolean, name: string) {
    this.map.addLayer(layer)

    if (showInControl) {
      if (this.layerControl.hasLayer(layer)) {
        this.layerControl.removeLayer(layer)
      }

      this.layerControl.addOverlay(layer, name)
    }
  }

  addHiddenLayer(layer: L.Layer, name: string) {
    if (this.layerControl.hasLayer(layer)) {
      this.layerControl.removeLayer(layer)
    }

    this.layerControl.addOverlay(layer, name)
  }

  removeLayer(layer: L.Layer) {
    this.map.removeLayer(layer)
    this.layerControl.removeLayer(layer)
  }
}
