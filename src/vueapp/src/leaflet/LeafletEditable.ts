import CustomMap from '@/leaflet/CustomMap'
import L from 'leaflet'
import { editable } from '@/leafletInteractivity/Editable'
import { type EditableShape } from '@/leafletInteractivity'
import type { AoiControl } from './controls/aoi/AoiControl'
import { includes } from 'lodash'

type Action = () => void

export enum AOIType {
  None,
  Rectangle,
  Circle,
  Polygon
}

export class LeafletEditable {
  private _map: CustomMap
  private _aoiControl: AoiControl
  private _aoiType: AOIType = AOIType.None
  private _shape: EditableShape | undefined
  private _tempShape: EditableShape | undefined
  private _endCommand?: Action | undefined

  constructor(aoiControl: AoiControl, map: CustomMap) {
    this._aoiControl = aoiControl
    this._map = map
    const fsm = this._aoiControl.getFsm()

    this._aoiControl.addEventListener('aoi:buttonClick', (e) => {
      if (includes(['rect', 'circle', 'poly'], e.name)) {
        const shape = e.name
        const createCommand = function () {
          fsm.create()
        }

        if (shape === 'rect') {
          this.createAOI(AOIType.Rectangle, createCommand)
        } else if (shape === 'circle') {
          this.createAOI(AOIType.Circle, createCommand)
        } else if (shape === 'poly') {
          this.createAOI(AOIType.Polygon, createCommand)
        }
      } else if (e.name === 'edit') {
        this.editShape()
      } else if (e.name === 'translate') {
        this.translateShape()
      }
    })

    this._aoiControl.addEventListener('aoi:actionClick', (e) => {
      if (e.name === 'remove') {
        this.removeShape()
      }
    })

    this._aoiControl.addEventListener('aoi:actionClick', (e) => {
      if (e.name === 'cancel') {
        this.cancel()
      }
    })

    this._aoiControl.addEventListener('aoi:actionClick', (e) => {
      if (e.name === 'finish') {
        this.finish()
      }
    })
  }

  createAOI(type: AOIType, createCommand?: Action) {
    this._aoiType = type
    this._endCommand = createCommand

    switch (type) {
      case AOIType.Rectangle: {
        const designer = editable(this._map).startRectangle()
        designer.addEventListener('designer:create', (e) => {
          const rect = e.feature as L.Rectangle
          this._shape = rect
          createCommand?.()
        })
        break
      }
      case AOIType.Circle: {
        const designer = editable(this._map).startCircle()
        designer.addEventListener('designer:create', (e) => {
          const circle = e.feature as L.Circle
          this._shape = circle
          createCommand?.()
        })
        break
      }
      case AOIType.Polygon: {
        const designer = editable(this._map).startPolygon()
        designer.addEventListener('designer:create', (e) => {
          const poly = e.feature as L.Polygon
          this._shape = poly
          createCommand?.()
        })
        break
      }
      default:
        break
    }
  }

  removeShape() {
    this._shape?.disableEdit()
    this._shape!.remove()
  }

  editShape() {
    this._tempShape = this._clone(this._shape!)
    this._shape?.enableEdit()
  }

  translateShape() {
    this._tempShape = this._clone(this._shape!)
    this._shape?.enableTranslation(this._map)
  }

  finish() {
    this._shape?.disableEdit()
    this._shape?.disableTranslation()
  }

  cancel() {
    this._shape?.disableEdit()
    this._shape?.disableTranslation()
    this._shape?.remove()
    this._shape = this._tempShape?.addTo(this._map)
  }

  _clone = (shape: EditableShape): EditableShape => {
    if (shape instanceof L.Rectangle) {
      return L.rectangle(shape.getBounds())
    }

    if (shape instanceof L.Circle) {
      const center = shape.getLatLng()
      const radius = shape.getRadius()
      return L.circle(center, { radius })
    }

    if (shape instanceof L.Polygon) {
      const arr = (shape.getLatLngs() as L.LatLng[][])[0].map(
        (s) => [s.lat, s.lng] as L.LatLngExpression
      )
      return L.polygon(arr)
    }

    throw new Error()
  }
}
