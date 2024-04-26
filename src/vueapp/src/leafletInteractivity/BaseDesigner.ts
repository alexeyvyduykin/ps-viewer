import * as L from 'leaflet'
import type { DesignerOptions, Shape } from '.'
import type { Editable } from './Editable'

export interface DesignerCreateEvent extends L.LeafletEvent {
  feature: Shape
}

export type DesignerCreateEventHandlerFn = (event: DesignerCreateEvent) => void

export abstract class BaseDesigner extends L.Evented {
  options: DesignerOptions
  interactiveLayer!: L.LayerGroup
  tools!: Editable
  map!: L.Map
  feature!: Shape
  protected _drawing: boolean = false

  constructor(map: L.Map, options?: DesignerOptions | undefined) {
    //super(map);
    super()

    this.map = map

    this.options = options || ({} as DesignerOptions)
  }
}
