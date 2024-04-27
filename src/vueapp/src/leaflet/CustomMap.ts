import L from 'leaflet'
import Layer1 from './Layer1'
import Layer2 from './Layer2'
import Layer3 from './Layer3'
import { LayerManager } from './LayerManager'
import Layer5 from './Layer5'

let _layer1: L.GeoJSON | Layer1 | undefined
let _layer2: L.GeoJSON | Layer2 | undefined
let _layer3: L.Layer
let _layer5: L.Layer

export default class CustomMap extends L.Map {
  private readonly _layerManager: LayerManager

  constructor(element: string | HTMLElement, options?: L.MapOptions) {
    super(element, options)

    this._layerManager = new LayerManager(this)

    this.fitWorld()

    _layer1 = new Layer1()
    _layer2 = new Layer2()
    _layer3 = new Layer3()
    _layer5 = new Layer5()

    this._layerManager.addLayer(_layer1, true, 'Tracks')
    this._layerManager.addLayer(_layer2, true, 'GroundStations')
    this._layerManager.addHiddenLayer(_layer3, 'GroundTargets')
    //this._layerManager.addLayer(_layer3, true, 'GroundTargets')
    //this._layerManager.addHiddenLayer(_layer5, 'Preview')
    this._layerManager.addLayer(_layer5, true, 'Preview')
  }
}
