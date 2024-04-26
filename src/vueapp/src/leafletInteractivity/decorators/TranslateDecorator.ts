import * as L from 'leaflet'
import type { Shape } from '..'
import type { DecoratorTranslateEvent } from '../BaseDecorator'

type rec = { [k: string]: any }

export class TranslateDecorator extends L.Handler {
  feature: Shape
  _options: L.PathOptions
  _saveDict: rec = {}
  declare map: L.Map

  constructor(map: L.Map, feature: Shape, options?: L.PathOptions) {
    super(map)
    this.map = map
    this._options = options ?? { weight: 5, fillOpacity: 0.65 }
    this.feature = feature
  }

  addHooks() {
    this.feature.options.draggable = true

    Object.entries(this._options).forEach(([key]) => {
      this._saveDict[key] = (this.feature.options as rec)[key]
    })

    if (this.feature instanceof L.Marker) {
      //
    } else {
      this.feature.setStyle(this._options)
    }

    if (this.feature.dragging) {
      this.feature.on('dragend', this.onDragEnd, this)
      this.feature.dragging.enable()
    }

    return this
  }

  removeHooks() {
    if (this.feature.dragging) {
      this.feature.off('dragend', this.onDragEnd, this)
      this.feature.dragging.disable()
    }

    if (this.feature instanceof L.Marker) {
      //
    } else {
      this.feature.setStyle(this._saveDict)
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onDragEnd(e?: any) {
    this.map.fireEvent('translate:dragEnd', { feature: this.feature } as DecoratorTranslateEvent)
  }
}
