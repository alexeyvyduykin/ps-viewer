import * as L from 'leaflet'
import { MarkerDesigner } from './designers/MarkerDesigner'
import type { DesignerOptions } from '.'
import { PolylineDesigner } from './designers/PolylineDesigner'
import { CircleDesigner } from './designers/CircleDesigner'
import { RectangleDesigner } from './designers/RectangleDesigner'
import { PolygonDesigner } from './designers/PolygonDesigner'

export class Editable extends L.Evented {
  map: L.Map
  editLayer: L.LayerGroup
  featuresLayer: L.LayerGroup
  _lastZIndex: number

  constructor(map: L.Map, options?: any) {
    super()
    L.setOptions(this, options)

    this._lastZIndex = 1000

    this.map = map
    this.map.editTools = this

    this.editLayer = new L.LayerGroup().addTo(this.map)
    this.featuresLayer = new L.LayerGroup().addTo(this.map)
  }

  static makeCancellable(e: L.LeafletEvent) {
    e.cancel = function (this: L.LeafletEvent) {
      this._cancelled = true
    }
  }

  startMarker() {
    const options = { isHint: true, isMarkerHint: true } as DesignerOptions
    const designer = new MarkerDesigner(this.map, options)
    designer.startDrawing()
    return designer
  }

  startPolyline() {
    const options = { isHint: true, isMarkerHint: true } as DesignerOptions
    const designer = new PolylineDesigner(this.map, options)
    designer.startDrawing()
    return designer
  }

  startRectangle() {
    const options = { isHint: true, isMarkerHint: true } as DesignerOptions
    const designer = new RectangleDesigner(this.map, options)
    designer.startDrawing()
    return designer
  }

  startCircle() {
    const options = { isHint: true, isMarkerHint: true } as DesignerOptions
    const designer = new CircleDesigner(this.map, options)
    designer.startDrawing()
    return designer
  }

  startPolygon() {
    const options = { isHint: true, isMarkerHint: true } as DesignerOptions
    const designer = new PolygonDesigner(this.map, options)
    designer.startDrawing()
    return designer
  }
}

export function editable(map: L.Map, options?: any): Editable {
  return new Editable(map, options)
}
