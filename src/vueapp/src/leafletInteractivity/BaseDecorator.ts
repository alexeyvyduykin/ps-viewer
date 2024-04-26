import * as L from 'leaflet'
import type { EditableOptions, Shape, EditableShape } from '.'
import type { Editable } from './Editable'

export interface DecoratorEditEvent extends L.LeafletEvent {
  feature: EditableShape
  vertex: L.Marker
}

export interface DecoratorTranslateEvent extends L.LeafletEvent {
  feature: Shape
}

export type DecoratorEditEventHandlerFn = (event: DecoratorEditEvent) => void
export type DecoratorTranslateEventHandlerFn = (event: DecoratorTranslateEvent) => void

export abstract class BaseDecorator extends L.Handler {
  options: EditableOptions
  interactiveLayer!: L.LayerGroup
  tools!: Editable
  map!: L.Map
  feature!: Shape

  constructor(map: L.Map, feature: Shape, options?: EditableOptions | undefined) {
    super(map)

    this.options = options || {}
  }
}
