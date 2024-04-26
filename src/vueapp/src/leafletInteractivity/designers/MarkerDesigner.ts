import * as L from 'leaflet'
import { Editable } from '../Editable'
import { BaseDesigner, type DesignerCreateEvent } from '../BaseDesigner'
import type { DesignerOptions } from '..'
import { getTranslation } from '../helpers'

export class MarkerDesigner extends BaseDesigner {
  declare interactiveLayer: L.LayerGroup
  declare map: L.Map
  declare feature: L.Marker
  declare options: DesignerOptions
  declare tools: Editable
  _hintMarker: L.Marker

  constructor(map: L.Map, options?: DesignerOptions | undefined) {
    super(map)
    L.Util.setOptions(this, options)
    this.map = map
    this.tools = map.editTools

    this.interactiveLayer = L.layerGroup()

    this._hintMarker = L.marker([-200, -200], {
      icon: L.divIcon({ className: 'marker-icon cursor-marker' }),
      draggable: false,
      zIndexOffset: 150, // -100
      interactive: false,
      opacity: this.options.isMarkerHint === true ? 1 : 0
    })
  }

  startDrawing() {
    this._drawing = true

    this.map.getContainer().classList.add('geoman-draw-cursor')

    this.interactiveLayer.addTo(this.map)
    this.interactiveLayer.addLayer(this._hintMarker)

    if (this.options.isMarkerHint === true) {
      L.DomUtil.addClass(this._hintMarker.getElement()!, 'visible')
    }

    if (this.options.isHint === true) {
      this._hintMarker
        .bindTooltip(getTranslation('tooltips.placeMarker'), {
          permanent: true,
          offset: L.point(0, 10),
          direction: 'bottom',
          opacity: 0.8
        })
        .openTooltip()
    }

    this.map.on('click', this._createMarker, this)
    this.map.on('mousemove', this._moveHintMarker, this)
  }

  endDrawing() {
    this._drawing = false

    this.map.getContainer().classList.remove('geoman-draw-cursor')

    this.map.off('click', this._createMarker, this)
    this.map.off('mousemove', this._moveHintMarker, this)

    this.map.removeLayer(this.interactiveLayer)

    this.tools.featuresLayer.addLayer(this.feature)
  }

  _createMarker(e: L.LeafletMouseEvent) {
    if (!e.latlng) {
      return
    }

    this.feature = L.marker(e.latlng, {
      icon: L.divIcon({ className: 'marker-icon cursor-marker.visible' })
    })

    this.map.fireEvent('designer:create', { feature: this.feature } as DesignerCreateEvent)
    this.fireEvent('designer:create', { feature: this.feature } as DesignerCreateEvent)

    this.endDrawing()
  }

  _moveHintMarker(e: L.LeafletMouseEvent) {
    this._hintMarker.setLatLng(e.latlng)
  }
}
