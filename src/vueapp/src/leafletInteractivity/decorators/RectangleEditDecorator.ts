import * as L from 'leaflet'
import type { EditableOptions } from '..'
import { BaseDecorator, type DecoratorEditEvent } from '../BaseDecorator'

export class RectangleEditDecorator extends BaseDecorator {
  declare map: L.Map
  declare feature: L.Rectangle
  declare options: EditableOptions
  declare interactiveLayer: L.LayerGroup
  _markers: L.Marker[]

  constructor(map: L.Map, feature: L.Rectangle, options: any) {
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

      marker.index = i

      marker.on('drag', this.vertexDrag, this)
      marker.on('dragend', () => this.vertexDragEnd(marker), this)

      this.interactiveLayer.addLayer(marker)

      this._markers.push(marker)
    }
  }

  vertexDrag(this: RectangleEditDecorator, e: L.LeafletEvent) {
    const marker = e.target as L.Marker
    const i = marker.index!
    const iPrev = this.getPrev(i)
    const iNext = this.getNext(i)
    const iRev = (i + 2) % 4
    const latlngRev = this._markers[iRev].getLatLng()

    this._markers[iPrev].setLatLng([marker.getLatLng().lat, latlngRev.lng])
    this._markers[iNext].setLatLng([latlngRev.lat, marker.getLatLng().lng])

    const bounds = L.latLngBounds(marker.getLatLng(), latlngRev)
    this.feature.setBounds(bounds)
  }

  vertexDragEnd(this: RectangleEditDecorator, marker: L.Marker) {
    this.map.fireEvent('edit:vertexDragEnd', {
      feature: this.feature,
      vertex: marker
    } as DecoratorEditEvent)

    this.feature.fireEvent('edit:vertexDragEnd', {
      feature: this.feature,
      vertex: marker
    } as DecoratorEditEvent)
  }

  getNext(i: number): number {
    if (i >= 0 && i < 3) {
      return i + 1
    }
    if (i === 3) {
      return 0
    }

    throw new Error('rectangleEditDecorator -> getNext')
  }

  getPrev(i: number): number {
    if (i > 0 && i <= 3) {
      return i - 1
    }
    if (i === 0) {
      return 3
    }

    throw new Error('rectangleEditDecorator -> getPrev')
  }
}
