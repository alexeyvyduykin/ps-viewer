import * as L from 'leaflet'
import { Editable } from '../Editable'
import { BaseDesigner, type DesignerCreateEvent } from '../BaseDesigner'
import type { DesignerOptions } from '..'
import { getTranslation } from '../helpers'

export class PolylineDesigner extends BaseDesigner {
  declare map: L.Map
  declare feature: L.Polyline
  declare options: DesignerOptions
  declare tools: Editable
  declare interactiveLayer: L.LayerGroup
  _vertices: L.LatLng[]
  extraLine: L.Polyline
  _hintMarker: L.Marker

  constructor(map: L.Map, options?: DesignerOptions | undefined) {
    super(map, options)
    L.setOptions(this, options)
    this.map = map
    this.feature = new L.Polyline([])
    this.tools = map.editTools
    this.interactiveLayer = new L.LayerGroup()
    this._vertices = []

    const extraLineOptions = {
      dashArray: '5,5',
      color: '#3388ff',
      interactive: false
    } as L.CircleMarkerOptions

    this.extraLine = L.polyline([], extraLineOptions)

    this._hintMarker = L.marker([-200, 200], {
      icon: L.divIcon({ className: 'marker-icon cursor-marker' }),
      draggable: false,
      zIndexOffset: 150, // -100
      interactive: false,
      opacity: 1 //this.options.isMarkerHint === true ? 1 : 0
    })
  }

  startDrawing() {
    this._drawing = true

    this.interactiveLayer.addTo(this.map)
    this.interactiveLayer.addLayer(this._hintMarker)

    this.map.on('mousemove', this.onDrawingMouseMove, this)
    this.map.on('click', this.onDrawingClick, this)

    this.map.getContainer().classList.add('geoman-draw-cursor')

    if (this.options.isMarkerHint === true) {
      L.DomUtil.addClass(this._hintMarker.getElement()!, 'visible')
    }

    if (this.options.isHint === true) {
      this._hintMarker
        .bindTooltip(getTranslation('tooltips.firstVertex'), {
          permanent: true,
          offset: L.point(0, 10),
          direction: 'bottom',
          opacity: 0.8
        })
        .openTooltip()
    }
  }

  onDrawingClick(e: L.LeafletMouseEvent) {
    if (this._vertices.length === 0) {
      this.interactiveLayer.addLayer(this.feature)
      this.interactiveLayer.addLayer(this.extraLine)
    }

    this._vertices.push(e.latlng)

    this.feature.setLatLngs(this._vertices)

    const marker = L.marker(e.latlng, {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' })
    })

    marker.index = this._vertices.length - 1
    marker.on('click', this.onMarkerClick, this)
    marker.on('mouseover', this.onMarkerMouseOver, this)
    marker.on('mouseout', this.onMarkerMouseOut, this)

    this.interactiveLayer.addLayer(marker)

    this.extraLine.setLatLngs([e.latlng, e.latlng])

    if (this._vertices.length === 1) {
      this._hintMarker.setTooltipContent(getTranslation('tooltips.nextVertex'))
    }

    if (this._vertices.length === 2) {
      this._hintMarker.setTooltipContent(getTranslation('tooltips.finishLine'))
    }
  }

  onMarkerClick(this: PolylineDesigner, e: L.LeafletMouseEvent): void {
    const marker = e.sourceTarget as L.Marker
    const index = marker.index

    if (this._vertices.length > 1 && index === this._vertices.length - 1) {
      this.endDrawing()
    }
  }

  onDrawingMouseMove(e: L.LeafletMouseEvent) {
    this._hintMarker.setLatLng(e.latlng)

    if (this.extraLine.getLatLngs().length) {
      this.extraLine.setLatLngs([this._vertices.slice(-1)[0], e.latlng])
    }
  }

  onMarkerMouseOver(this: PolylineDesigner, e: L.LeafletMouseEvent): void {
    const marker = e.sourceTarget as L.Marker
    const index = marker.index

    if (this._vertices.length > 1 && index === this._vertices.length - 1) {
      L.DomUtil.removeClass(this._hintMarker.getElement()!, 'visible')
      L.DomUtil.addClass(marker.getElement()!, 'lastClick')
    }
  }

  onMarkerMouseOut(this: PolylineDesigner, e: L.LeafletMouseEvent): void {
    const marker = e.sourceTarget as L.Marker
    L.DomUtil.addClass(this._hintMarker.getElement()!, 'visible')
    L.DomUtil.removeClass(marker.getElement()!, 'lastClick')
  }

  endDrawing() {
    this._drawing = false

    const latlngs = this.feature.getLatLngs() as L.LatLng[]

    const feature = L.polyline(latlngs)

    this.map.fireEvent('designer:create', { feature: feature } as DesignerCreateEvent)
    this.fireEvent('designer:create', { feature: feature } as DesignerCreateEvent)

    this.tools.featuresLayer.addLayer(feature)

    this.map.getContainer().classList.remove('geoman-draw-cursor')

    this.map.off('mousemove', this.onDrawingMouseMove, this)
    this.map.off('click', this.onDrawingClick, this)

    this.map.removeLayer(this.interactiveLayer)
  }
}
