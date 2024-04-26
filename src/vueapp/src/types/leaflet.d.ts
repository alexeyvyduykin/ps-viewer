declare module 'leaflet' {
  type Editable = import('@/leafletInteractivity/Editable').Editable
  type BaseDesigner = import('@/leafletInteractivity/BaseDesigner').BaseDesigner
  type DesignerCreateEventHandlerFn =
    import('@/leafletInteractivity/BaseDesigner').DesignerCreateEventHandlerFn
  type BaseDecorator = import('@/leafletInteractivity/BaseDecorator').BaseDecorator
  type DecoratorEditEventHandlerFn =
    import('@/leafletInteractivity/BaseDecorator').DecoratorEditEventHandlerFn
  type DecoratorTranslateEventHandlerFn =
    import('@/leafletInteractivity/BaseDecorator').DecoratorTranslateEventHandlerFn
  type TranslateDecorator =
    import('@/leafletInteractivity/decorators/TranslateDecorator').TranslateDecorator
  type AoiButtonEvent = import('@/leaflet/controls/aoi/AoiButton').AoiButtonEvent
  type AoiActionEvent = import('@/leaflet/controls/aoi/AoiButton').AoiActionEvent

  interface PathDrag {
    enable: () => void
    disable: () => void
  }

  interface EditableOptions {}

  interface CircleMarkerOptions {
    draggable?: boolean
  }

  interface PolylineOptions {
    draggable?: boolean
  }

  interface LeafletEvent {
    _cancelled: boolean
    cancel: () => void
  }

  interface EditableMixin {
    editor?: BaseDecorator | undefined
    getEditorClass: (map: L.Map, options?: EditableOptions | undefined) => BaseDecorator

    createEditor(map?: L.Map): BaseDecorator
    enableEdit(map?: L.Map): BaseDecorator
    editEnabled(): boolean
    disableEdit(): void
    toggleEdit(): void
  }

  interface TranslateDecoratorMixin {
    enableTranslation(map: L.Map, options?: PathOptions | undefined): void
    disableTranslation(): void
    translateDecorator?: TranslateDecorator | undefined

    dragging?: PathDrag
  }

  interface Map {
    editTools: Editable
  }

  interface Evented {
    on(type: 'designer:create', fn: DesignerCreateEventHandlerFn, context?: any): this
    on(type: 'edit:vertexDragEnd', fn: DecoratorEditEventHandlerFn, context?: any): this
    on(type: 'translate:dragEnd', fn: DecoratorTranslateEventHandlerFn, context?: any): this

    on(type: 'aoi:buttonClick', fn: (e: AoiButtonEvent) => void, context?: any): this
    on(type: 'aoi:actionClick', fn: (e: AoiActionEvent) => void, context?: any): this

    off(type: 'designer:create', fn: DesignerCreateEventHandlerFn, context?: any): this
    off(type: 'edit:vertexDragEnd', fn: DecoratorEditEventHandlerFn, context?: any): this
    off(type: 'translate:dragEnd', fn: DecoratorTranslateEventHandlerFn, context?: any): this

    of(type: 'aoi:buttonClick', fn: (e: AoiButtonEvent) => void, context?: any): this
    of(type: 'aoi:actionClick', fn: (e: AoiActionEvent) => void, context?: any): this

    addEventListener(type: 'designer:create', fn: DesignerCreateEventHandlerFn, context?: any): this
    addEventListener(
      type: 'edit:vertexDragEnd',
      fn: DecoratorEditEventHandlerFn,
      context?: any
    ): this
    addEventListener(
      type: 'translate:dragEnd',
      fn: DecoratorTranslateEventHandlerFn,
      context?: any
    ): this

    addEventListener(type: 'aoi:buttonClick', fn: (e: AoiButtonEvent) => void, context?: any): this
    addEventListener(type: 'aoi:actionClick', fn: (e: AoiActionEvent) => void, context?: any): this

    removeEventListener(
      type: 'designer:create',
      fn: DesignerCreateEventHandlerFn,
      context?: any
    ): this
    removeEventListener(
      type: 'edit:vertexDragEnd',
      fn: DecoratorEditEventHandlerFn,
      context?: any
    ): this
    removeEventListener(
      type: 'translate:dragEnd',
      fn: DecoratorTranslateEventHandlerFn,
      context?: any
    ): this

    removeEventListener(
      type: 'aoi:buttonClick',
      fn: (e: AoiButtonEvent) => void,
      context?: any
    ): this
    removeEventListener(
      type: 'aoi:actionClick',
      fn: (e: AoiActionEvent) => void,
      context?: any
    ): this
  }

  interface Marker extends TranslateDecoratorMixin {
    handler?: BaseDesigner | BaseDecorator | undefined
    update(): () => void
    index?: number | undefined
  }

  interface Polyline extends EditableMixin, TranslateDecoratorMixin {}

  interface Rectangle extends EditableMixin, TranslateDecoratorMixin {}

  interface Circle extends EditableMixin, TranslateDecoratorMixin {}

  interface Polygon extends EditableMixin, TranslateDecoratorMixin {}
}

export {}
