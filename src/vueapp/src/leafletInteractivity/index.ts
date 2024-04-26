import * as L from 'leaflet'
import { PolygonEditDecorator } from './decorators/PolygonEditDecorator'
import { RectangleEditDecorator } from './decorators/RectangleEditDecorator'
import { CircleEditDecorator } from './decorators/CircleEditDecorator'
import { PolylineEditDecorator } from './decorators/PolylineEditDecorator'
import { TranslateDecorator } from './decorators/TranslateDecorator'
import type { BaseDecorator } from './BaseDecorator'

export interface EditableOptions {}

export interface DesignerOptions {
  isHint: boolean
  isMarkerHint: boolean
  continueDrawing: boolean | undefined
}

export interface TranslateDecoratorOptions extends L.PathOptions {}

export type Shape = L.Marker | L.Rectangle | L.Circle | L.Polyline | L.Polygon
export type EditableShape = L.Rectangle | L.Circle | L.Polyline | L.Polygon

function initMixins() {
  const classes = [L.Polyline, L.Circle]
  for (let i = 0; i < classes.length; i++) {
    const cl = classes[i]
    cl.prototype.createEditor = createEditorImpl
    cl.prototype.enableEdit = enableEditImpl
    cl.prototype.editEnabled = editEnabledImpl
    cl.prototype.disableEdit = disableEditImpl
    cl.prototype.toggleEdit = toggleEditImpl
    cl.prototype.enableTranslation = enableTranslationImpl
    cl.prototype.disableTranslation = disableTranslationImpl
  }

  const classes2 = [L.Marker]
  for (let i = 0; i < classes2.length; i++) {
    const cl = classes2[i]
    cl.prototype.enableTranslation = enableTranslationImpl
    cl.prototype.disableTranslation = disableTranslationImpl
  }
}

initMixins()

L.Polygon.prototype.getEditorClass = createPolygonEditor
L.Rectangle.prototype.getEditorClass = createRectangleEditor
L.Circle.prototype.getEditorClass = createCircleEditor
L.Polyline.prototype.getEditorClass = createPolylineEditor

function createPolygonEditor(
  this: L.Polygon,
  map: L.Map,
  options?: EditableOptions | undefined
): BaseDecorator {
  return new PolygonEditDecorator(map, this, options)
}

function createRectangleEditor(
  this: L.Rectangle,
  map: L.Map,
  options?: EditableOptions | undefined
): BaseDecorator {
  return new RectangleEditDecorator(map, this, options)
}

function createCircleEditor(
  this: L.Circle,
  map: L.Map,
  options?: EditableOptions | undefined
): BaseDecorator {
  return new CircleEditDecorator(map, this, options)
}

function createPolylineEditor(
  this: L.Polyline,
  map: L.Map,
  options?: EditableOptions | undefined
): BaseDecorator {
  return new PolylineEditDecorator(map, this, options)
}

function createEditorImpl(this: EditableShape, map?: L.Map) {
  map = map || (this as L.Layer)['_map']
  return this.getEditorClass(map)
}

function enableEditImpl(this: EditableShape, map?: L.Map): BaseDecorator {
  if (this.editor === undefined) {
    this.createEditor(map)
  }
  this.editor!.enable()
  return this.editor!
}

function editEnabledImpl(this: EditableShape): boolean {
  return this.editor !== undefined && this.editor.enabled()
}

function disableEditImpl(this: EditableShape) {
  if (this.editor) {
    this.editor.disable()
    delete this.editor
  }
}

function toggleEditImpl(this: EditableShape) {
  if (this.editEnabled()) {
    this.disableEdit()
  } else {
    this.enableEdit()
  }
}

function enableTranslationImpl(this: Shape, map: L.Map, options?: L.PathOptions | undefined) {
  if (this.translateDecorator === undefined) {
    this.translateDecorator = new TranslateDecorator(map, this, options)
    this.translateDecorator.enable()
  }
}

function disableTranslationImpl(this: Shape) {
  if (this.translateDecorator) {
    this.translateDecorator.disable()
    delete this.translateDecorator
  }
}

Math.toRadians = function (degrees: number) {
  return (degrees * Math.PI) / 180
}

Math.toDegrees = function (radians: number) {
  return (radians * 180) / Math.PI
}
