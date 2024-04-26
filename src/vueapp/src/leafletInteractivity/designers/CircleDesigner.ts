import * as L from 'leaflet'
import { Editable } from '../Editable'
import { BaseDesigner, type DesignerCreateEvent } from '../BaseDesigner'
import { type DesignerOptions } from '..'
import { getTranslation } from '../helpers'

export class CircleDesigner extends BaseDesigner {
  declare map: L.Map
  declare feature: L.Circle
  declare options: DesignerOptions
  declare tools: Editable
  declare interactiveLayer: L.LayerGroup
  extraFeature: L.Circle
  extraLine: L.Polyline
  originClick: L.LatLng = L.latLng([0, 0])
  _hintMarker: L.Marker
  extraVertex0: L.Marker

  constructor(map: L.Map, options?: DesignerOptions | undefined) {
    super(map, options)
    L.setOptions(this, options)
    this.map = map
    this.feature = L.circle([0, 0], { radius: 0 })
    this.interactiveLayer = new L.LayerGroup()
    this.tools = map.editTools

    const extraOptions = {
      dashArray: '5,5',
      color: '#3388ff',
      interactive: false
    } as L.CircleMarkerOptions

    const extraLineOptions = {
      dashArray: '5,5',
      color: '#3388ff',
      interactive: false
    } as L.PolylineOptions

    this.extraFeature = L.circle([0, 0], 0, extraOptions)
    this.extraLine = L.polyline([], extraLineOptions)

    this._hintMarker = L.marker([-200, 200], {
      icon: L.divIcon({ className: 'marker-icon cursor-marker' }),
      draggable: false,
      zIndexOffset: 150, // -100
      interactive: false,
      opacity: 1 //this.options.isMarkerHint === true ? 1 : 0
    })

    this.extraVertex0 = L.marker([0, 0], {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' })
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
        .bindTooltip(getTranslation('tooltips.startCircle'), {
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

    this.extraFeature.setLatLng(e.latlng)
    this.extraFeature.setRadius(0)

    this.extraVertex0.addTo(this.interactiveLayer)
    this.extraVertex0.setLatLng(e.latlng)

    this.extraLine.addTo(this.interactiveLayer)
    this.extraLine.setLatLngs([e.latlng, e.latlng])

    this.map.off('click', this._firstClick, this)
    this.map.on('click', this._secondClick, this)

    this._hintMarker.setTooltipContent(getTranslation('tooltips.finishCircle'))
  }

  _secondClick(e: L.LeafletMouseEvent) {
    this.tools.featuresLayer.addLayer(this.feature)

    const radius = this.originClick.distanceTo(e.latlng)

    this.feature.setLatLng(this.originClick)
    this.feature.setRadius(radius)

    this.map.fireEvent('designer:create', { feature: this.feature } as DesignerCreateEvent)
    this.fireEvent('designer:create', { feature: this.feature } as DesignerCreateEvent)

    this.endDrawing()
  }

  onDrawingMouseMove(e: L.LeafletMouseEvent) {
    this._hintMarker.setLatLng(e.latlng)

    if (this.originClick) {
      const radius = this.extraFeature.getLatLng().distanceTo(e.latlng)
      this.extraFeature.setRadius(radius)
      this.extraLine.setLatLngs([this.originClick, e.latlng])
    }
  }

  endDrawing() {
    this._drawing = false

    this.map.getContainer().classList.remove('geoman-draw-cursor')

    this.map.off('mousemove', this.onDrawingMouseMove, this)
    this.map.off('click', this._firstClick, this)
    this.map.off('click', this._secondClick, this)

    this.map.removeLayer(this.interactiveLayer)
  }
}
