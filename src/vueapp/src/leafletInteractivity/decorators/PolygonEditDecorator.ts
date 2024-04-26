import * as L from 'leaflet'
import type { EditableOptions } from '..'
import { BaseDecorator, type DecoratorEditEvent } from '../BaseDecorator'

export class PolygonEditDecorator extends BaseDecorator {
  declare map: L.Map
  declare feature: L.Polygon
  declare options: EditableOptions
  declare interactiveLayer: L.LayerGroup
  _markers: L.Marker[]

  constructor(map: L.Map, feature: L.Polygon, options?: any) {
    super(map, feature, options)
    L.setOptions(this, options)
    this.map = map
    this.feature = feature
    this.feature.editor = this
    this.interactiveLayer = new L.LayerGroup()
    this._markers = []
  }

  addHooks() {
    this.interactiveLayer.addTo(this.map)
    this.initVertexMarkers()
    return this
  }

  removeHooks() {
    this.interactiveLayer.clearLayers()
    this.map.removeLayer(this.interactiveLayer)
  }

  initVertexMarkers() {
    const latlngs = this.feature.getLatLngs()[0] as L.LatLng[]

    for (let i = 0; i < latlngs.length; i++) {
      const marker = L.marker(latlngs[i], {
        icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' }),
        draggable: true
      })

      marker.on('drag', this.vertexDrag, this)
      marker.on('dragend', () => this.vertexDragEnd(marker), this)

      this.interactiveLayer.addLayer(marker)

      this._markers.push(marker)
    }
  }

  vertexDrag(this: PolygonEditDecorator) {
    this.feature.setLatLngs(this._markers.map((s) => s.getLatLng()))
  }

  vertexDragEnd(this: PolygonEditDecorator, marker: L.Marker) {
    this.map.fireEvent('edit:vertexDragEnd', {
      feature: this.feature,
      vertex: marker
    } as DecoratorEditEvent)

    this.feature.fireEvent('edit:vertexDragEnd', {
      feature: this.feature,
      vertex: marker
    } as DecoratorEditEvent)
  }
}
