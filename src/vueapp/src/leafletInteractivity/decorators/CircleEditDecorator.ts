import * as L from 'leaflet'
import type { EditableOptions } from '..'
import { BaseDecorator, type DecoratorEditEvent } from '../BaseDecorator'

export class CircleEditDecorator extends BaseDecorator {
  declare map: L.Map
  declare feature: L.Circle
  declare options: EditableOptions
  declare interactiveLayer: L.LayerGroup
  _marker0?: L.Marker
  _marker?: L.Marker
  extraLine: L.Polyline

  constructor(map: L.Map, feature: L.Circle, options?: EditableOptions | undefined) {
    super(map, feature, options)
    L.setOptions(this, options)
    this.map = map
    this.feature = feature
    this.feature.editor = this
    this.interactiveLayer = L.layerGroup()

    const extraLineOptions = {
      dashArray: '5,5',
      color: '#3388ff',
      interactive: false
    } as L.PolylineOptions

    this.extraLine = L.polyline([], extraLineOptions)
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
    const latlng0 = this.feature.getLatLng()
    const marker0 = L.marker(latlng0, {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' })
    })

    this.interactiveLayer.addLayer(marker0)

    const latlng = this.computeResizeLatLng()
    const marker = L.marker(latlng, {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' }),
      draggable: true
    })

    marker.on('drag', this.vertexDrag, this)
    marker.on('dragend', () => this.vertexDragEnd(marker), this)

    this.interactiveLayer.addLayer(marker)

    this._marker0 = marker0
    this._marker = marker

    this.extraLine.setLatLngs([latlng0, latlng])
    this.interactiveLayer.addLayer(this.extraLine)
  }

  vertexDrag(this: CircleEditDecorator) {
    const latlng0 = this._marker0!.getLatLng()
    const latlng = this._marker!.getLatLng()
    const radius = latlng0.distanceTo(latlng)
    this.feature.setRadius(radius)
    this.extraLine.setLatLngs([latlng0, latlng])
  }

  vertexDragEnd(this: CircleEditDecorator, marker: L.Marker) {
    this.map.fireEvent('edit:vertexDragEnd', {
      feature: this.feature,
      vertex: marker
    } as DecoratorEditEvent)

    this.feature.fireEvent('edit:vertexDragEnd', {
      feature: this.feature,
      vertex: marker
    } as DecoratorEditEvent)
  }

  computeResizeLatLng() {
    return this.pointOnCircle(this.feature.getLatLng(), this.feature.getRadius(), 45)
  }

  pointOnCircle(origin: L.LatLng, distance: number, bearing: number): L.LatLng {
    const radius = 6371e3
    const Ad = distance / radius
    const br = Math.toRadians(bearing)

    const lat1 = Math.toRadians(origin.lat)
    const lon1 = Math.toRadians(origin.lng)

    const sinlat2 = Math.sin(lat1) * Math.cos(Ad) + Math.cos(lat1) * Math.sin(Ad) * Math.cos(br)
    const lat2 = Math.asin(sinlat2)
    const y = Math.sin(br) * Math.sin(Ad) * Math.cos(lat1)
    const x = Math.cos(Ad) - Math.sin(lat1) * sinlat2
    const lon2 = lon1 + Math.atan2(y, x)

    const lat = Math.toDegrees(lat2)
    const lon = Math.toDegrees(lon2)

    return L.latLng(lat, lon)
  }
}
