import * as L from 'leaflet'
import { Editable } from '../Editable'
import { BaseDesigner, type DesignerCreateEvent } from '../BaseDesigner'
import type { DesignerOptions } from '..'
import { getTranslation } from '../helpers'

export class PolygonDesigner extends BaseDesigner {
  declare map: L.Map
  declare feature: L.Polyline | L.Polygon
  declare options: DesignerOptions
  declare tools: Editable
  declare interactiveLayer: L.LayerGroup
  _vertices: L.LatLng[]
  extraLine: L.Polyline
  extraArea: L.Polygon
  _hintMarker: L.Marker

  constructor(map: L.Map, options?: DesignerOptions | undefined) {
    super(map)
    L.setOptions(this, options)
    this.map = map
    this.feature = L.polyline([]) as L.Polyline
    this.interactiveLayer = new L.LayerGroup()
    this.tools = map.editTools
    this._vertices = this.feature.getLatLngs() as L.LatLng[]

    const extraLineOptions = {
      dashArray: '5,5',
      color: '#3388ff',
      interactive: false
    } as L.CircleMarkerOptions

    const extraFeatureOptions = { weight: 0, interactive: false } as L.PolylineOptions

    this.extraLine = L.polyline([], extraLineOptions)
    this.extraArea = L.polygon([], extraFeatureOptions)

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

    if (this._vertices.length === 3) {
      this._hintMarker.setTooltipContent(getTranslation('tooltips.finishPolygon'))
    }

    this.interactiveLayer.addLayer(this.extraLine)
    this.interactiveLayer.addLayer(this.extraArea)

    this.extraArea.setLatLngs(this._vertices)
  }

  onMarkerClick(this: PolygonDesigner, e: L.LeafletMouseEvent) {
    const marker = e.sourceTarget as L.Marker
    const index = marker.index

    if (this._vertices.length > 2 && index === 0) {
      this.endDrawing()
    }
  }

  onMarkerMouseOver(this: PolygonDesigner, e: L.LeafletMouseEvent) {
    const marker = e.sourceTarget as L.Marker
    const index = marker.index

    if (this._vertices.length > 2 && index === 0) {
      L.DomUtil.removeClass(this._hintMarker.getElement()!, 'visible')
      L.DomUtil.addClass(marker.getElement()!, 'lastClick')
    }
  }

  onMarkerMouseOut(this: PolygonDesigner, e: L.LeafletMouseEvent) {
    const marker = e.sourceTarget as L.Marker
    L.DomUtil.addClass(this._hintMarker.getElement()!, 'visible')
    L.DomUtil.removeClass(marker.getElement()!, 'lastClick')
  }

  onDrawingMouseMove(e: L.LeafletMouseEvent) {
    this._hintMarker.setLatLng(e.latlng)

    if (this.extraLine.getLatLngs().length > 0) {
      this.extraLine.setLatLngs([this._vertices.slice(-1)[0], e.latlng])
    }
  }

  endDrawing() {
    const latlngs = this.feature.getLatLngs() as L.LatLng[]

    const feature = L.polygon(latlngs)

    this.map.fireEvent('designer:create', { feature: feature } as DesignerCreateEvent)
    this.fireEvent('designer:create', { feature: feature } as DesignerCreateEvent)

    this.tools.featuresLayer.addLayer(feature)

    this._drawing = false

    this.map.getContainer().classList.remove('geoman-draw-cursor')

    this.map.off('mousemove', this.onDrawingMouseMove, this)
    this.map.off('click', this.onDrawingClick, this)

    this.map.removeLayer(this.interactiveLayer)
  }
}
