//import { LeafletEditable } from '@/leaflet/LeafletEditable'
import CustomMap from '@/leaflet/CustomMap'
import { scaleBar } from '@/leaflet/controls/ScaleBarControl'
//import { AoiControl, aoiControl } from '@/leaflet/controls/aoi/AoiControl'
import L from 'leaflet'

class LeafletService {
  _map: CustomMap | undefined
  _document!: HTMLElement
  _editable!: LeafletEditable
  _aoiControl!: AoiControl

  createMap(mapElementId: HTMLElement): CustomMap {
    if (!this._map) {
      this._document = mapElementId
      this._map = new CustomMap(mapElementId, {
        // editable: true,
        // editOptions: {
        //   skipMiddleMarkers: false,
        //   rectangleEditorClass: MyRectangleEditor,
        // },
        preferCanvas: true,
        worldCopyJump: true,
        maxBounds: new L.LatLngBounds(new L.LatLng(-90, -180), new L.LatLng(90, 180))
      })

      scaleBar({ isCompact: false }).addTo(this._map)

      //this._aoiControl = aoiControl(this._map, {})
      //this._aoiControl.addControls()

      //this._editable = new LeafletEditable(this._aoiControl, this._map)

      //this._editable.init();
    }
    return this._map
  }

  getAoi(): AoiControl {
    return this._aoiControl!
  }

  getMap(): CustomMap {
    return this._map!
  }

  getMapId(): HTMLElement {
    return this._document!
  }

  getEditable(): LeafletEditable {
    return this._editable!
  }
}

export default new LeafletService()
