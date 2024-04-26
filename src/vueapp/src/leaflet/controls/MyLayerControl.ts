import { Control, Layer, Map as LeafletMap, Util } from 'leaflet'

//import { useStore } from "@/store";
//import { MutationTypes } from "@/store/mutation-types";
//import { handleKeyboardEvent } from "@/util/events";
//import LayersObject = Control.LayersObject
//import LayersOptions = Control.LayersOptions

export class MyLayerControl extends Control.Layers {
  //   declare _map?: LeafletMap;
  //   declare _overlaysList?: HTMLElement;
  //   declare _baseLayersList?: HTMLElement;
  //   declare _layerControlInputs?: HTMLElement[];
  //   declare _container?: HTMLElement;
  //   declare _section?: HTMLElement;
  //   declare _separator?: HTMLElement;

  //   private _layersButton?: HTMLElement;
  //   private _layerPositions: Map<Layer, number>;
  //   private visible: boolean = false;

  constructor(
    baseLayers?: Control.LayersObject,
    overlays?: Control.LayersObject,
    options?: Control.LayersOptions
  ) {
    super(baseLayers, overlays, options)
  }

  //   constructor(baseLayers?: LayersObject, overlays?: LayersObject, options?: LayersOptions) {
  //     // noinspection JSUnusedGlobalSymbols
  //     super(
  //       baseLayers,
  //       overlays,
  //       Object.assign(options || {}, {
  //         sortLayers: true,
  //         sortFunction: (layer1: Layer, layer2: Layer, name1: string, name2: string) => {
  //           const priority1 = this._layerPositions.get(layer1) || 0,
  //             priority2 = this._layerPositions.get(layer2) || 0;

  //           if (priority1 !== priority2) {
  //             return priority1 - priority2;
  //           }

  //           return name1 < name2 ? -1 : name1 > name2 ? 1 : 0;
  //         },
  //       }),
  //     );
  //     this._layerPositions = new Map<Layer, number>();
  //   }

  hasLayer(layer: Layer): boolean {
    // @ts-ignore
    return !!super._getLayer(Util.stamp(layer))
  }

  //   addOverlayAtPosition(layer: Layer, name: string, position: number): this {
  //     //this._layerPositions.set(layer, position);
  //     return super.addOverlay(layer, name);
  //   }

  //   addOverlay(layer: Layer, name: string): this {
  //     //this._layerPositions.set(layer, 0);
  //     return super.addOverlay(layer, name);
  //   }

  //   removeLayer(layer: Layer): this {
  //     //this._layerPositions.delete(layer);
  //     return super.removeLayer(layer);
  //   }

  onRemove(map: LeafletMap) {
    //this._layerControlInputs = [];

    ;(super.onRemove as Function)(map)
  }
}
