import * as L from 'leaflet'
import { Editable } from '../Editable'
import { BaseDesigner, type DesignerCreateEvent } from '../BaseDesigner'
import type { DesignerOptions } from '..'
import { getTranslation } from '../helpers'

export class RectangleDesigner extends BaseDesigner {
  declare map: L.Map
  declare feature: L.Rectangle
  declare options: DesignerOptions
  declare tools: Editable
  declare interactiveLayer: L.LayerGroup
  extraFeature: L.Rectangle
  extraVertex0: L.Marker
  extraVertex1: L.Marker
  extraVertex2: L.Marker
  originClick?: L.LatLng | undefined
  _hintMarker: L.Marker

  constructor(map: L.Map, options?: DesignerOptions | undefined) {
    super(map)
    L.setOptions(this, options)
    this.map = map
    this.tools = map.editTools
    this.interactiveLayer = new L.LayerGroup()

    this.feature = L.rectangle([
      [0, 0],
      [0, 0]
    ])

    const extraOptions = {
      dashArray: '5,5',
      color: '#3388ff',
      interactive: false
    } as L.PolylineOptions

    this.extraFeature = L.rectangle(
      [
        [0, 0],
        [0, 0]
      ],
      extraOptions
    )

    this.extraVertex0 = L.marker([0, 0], {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' })
    })

    this.extraVertex1 = L.marker([0, 0], {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' })
    })

    this.extraVertex2 = L.marker([0, 0], {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' })
    })

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
    this.map.on('click', this._firstClick, this)

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

  _firstClick(e: L.LeafletMouseEvent) {
    this.interactiveLayer.addLayer(this.extraFeature)

    this.originClick = e.latlng.clone()

    this.extraVertex0.addTo(this.interactiveLayer)
    this.extraVertex1.addTo(this.interactiveLayer)
    this.extraVertex2.addTo(this.interactiveLayer)

    this.extraVertex0.setLatLng(e.latlng)
    this.extraVertex1.setLatLng(e.latlng)
    this.extraVertex2.setLatLng(e.latlng)

    this.map.off('click', this._firstClick, this)
    this.map.on('click', this._secondClick, this)

    this._hintMarker.setTooltipContent(getTranslation('tooltips.finishRectangle'))
  }

  _secondClick(e: L.LeafletMouseEvent) {
    this.tools.featuresLayer.addLayer(this.feature)

    const bounds = L.latLngBounds(this.originClick!, e.latlng)
    this.feature.setBounds(bounds)

    this.map.fireEvent('designer:create', { feature: this.feature } as DesignerCreateEvent)
    this.fireEvent('designer:create', { feature: this.feature } as DesignerCreateEvent)

    this.endDrawing()
  }

  endDrawing() {
    this._drawing = false

    this.map.getContainer().classList.remove('geoman-draw-cursor')

    this.map.off('mousemove', this.onDrawingMouseMove, this)
    this.map.off('click', this._firstClick, this)
    this.map.off('click', this._secondClick, this)

    this.map.removeLayer(this.interactiveLayer)
  }

  onDrawingMouseMove(e: L.LeafletMouseEvent) {
    this._hintMarker.setLatLng(e.latlng)

    if (this.originClick) {
      const p1 = this.originClick
      const p2 = L.latLng(this.originClick.lat, e.latlng.lng)
      const p3 = e.latlng
      const p4 = L.latLng(e.latlng.lat, this.originClick.lng)
      const arr = [p1, p2, p3, p4]
      this.extraFeature.setLatLngs(arr)
      this.extraVertex1.setLatLng(p2)
      this.extraVertex2.setLatLng(p4)
    }
  }
}
