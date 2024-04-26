/* eslint-disable @typescript-eslint/no-unused-vars */
import * as L from 'leaflet'
import { aoiButton, AoiButton } from './AoiButton'
import type { AoiButtonOptions, AoiButtonEvent, AoiActionEvent } from './AoiButton'
import { Aoi } from './aoi'
import { getTranslation } from '@/utils/translation'

export type AoiControlOptions = L.ControlOptions & {}

export class AoiControl extends L.Evented {
  options: AoiControlOptions
  _map: L.Map
  _drawContainer: HTMLDivElement
  _editContainer: HTMLDivElement
  buttons: AoiButton[]
  _fsm: Aoi

  constructor(map: L.Map, options: AoiControlOptions) {
    super()

    this._map = map
    this.options = options
    this.options.position = 'topleft'

    this.buttons = []

    this._drawContainer = L.DomUtil.create('div', 'leaflet-aoi-control')

    this._editContainer = L.DomUtil.create('div', 'leaflet-aoi-control leaflet-bar leaflet-control')

    this._hideEditContainer()

    Aoi.prototype.onCreating = function (context: AoiControl, shape: string) {
      context._selectDrawButton(shape)
      context._getButton(shape).disable()
    }

    Aoi.prototype.onCreate = function (context: AoiControl, shape: string) {
      context._showEditContainer()
    }

    Aoi.prototype.onRemove = function (context: AoiControl, shape: string) {
      context._unselectDrawButton()
      context._hideEditContainer()

      context._getButton(shape).undisable()
    }

    Aoi.prototype.onCancel = function (context: AoiControl, btn: string) {
      context._getButton(btn).unselect()
    }

    this._fsm = new Aoi(this)

    this._fsm.log()
  }

  getFsm() {
    return this._fsm
  }

  addControls() {
    const btnRect = {
      name: 'rect',
      title: getTranslation('buttonTitles.drawRectangleButton'),
      icon: 'pi pi-stop',
      position: 'topleft',
      actions: ['remove'],
      tool: 'draw'
    } as AoiButtonOptions

    const btnCircle = {
      name: 'circle',
      title: getTranslation('buttonTitles.drawCircleButton'),
      icon: 'pi pi-circle',
      position: 'topleft',
      actions: ['remove'],
      hidden: true,
      tool: 'draw'
    } as AoiButtonOptions

    const btnPoly = {
      name: 'poly',
      title: getTranslation('buttonTitles.drawPolygonButton'),
      icon: 'pi pi-caret-up',
      position: 'topleft',
      actions: ['remove'],
      hidden: true,
      tool: 'draw'
    } as AoiButtonOptions

    const btnTranslate = {
      name: 'translate',
      title: getTranslation('buttonTitles.translateButton'),
      position: 'topleft',
      icon: 'pi pi-arrows-h',
      actions: ['cancel', 'finish'],
      tool: 'edit'
    } as AoiButtonOptions

    const btnEdit = {
      name: 'edit',
      title: getTranslation('buttonTitles.editButton'),
      icon: 'pi pi-window-maximize',
      position: 'topleft',
      actions: ['cancel', 'finish'],
      tool: 'edit'
    } as AoiButtonOptions

    const button1 = aoiButton(this, this._drawContainer, btnRect)
    const button2 = aoiButton(this, this._drawContainer, btnCircle)
    const button3 = aoiButton(this, this._drawContainer, btnPoly)

    const button4 = aoiButton(this, this._editContainer, btnEdit)
    const button5 = aoiButton(this, this._editContainer, btnTranslate)

    this.buttons = [button1, button2, button3, button4, button5]

    for (let i = 0; i < this.buttons.length; i++) {
      this.buttons[i].addTo(this._map)
    }

    this._drawContainer.addEventListener('pointerover', () => this._showDrawContainer(this))
    this._drawContainer.addEventListener('pointerout', () => this._hideDrawContainer(this))

    L.DomEvent.disableClickPropagation(this._drawContainer)
  }

  _getDrawButtons(): AoiButton[] {
    return this.buttons.filter(function (s) {
      return s.options.tool === 'draw'
    })
  }

  _getButton(name: string): AoiButton {
    return this.buttons.filter(function (s) {
      return s.options.name === name
    })[0]
  }

  _selectDrawButton(shape: string) {
    const drawButtons = this._getDrawButtons()

    for (let i = 0; i < drawButtons.length; i++) {
      const item = drawButtons[i]
      if (item.options.name !== shape) {
        item._hide()
      } else {
        item._selectOne()
      }
    }
  }

  _unselectDrawButton() {
    const drawButtons = this._getDrawButtons()

    for (let i = 0; i < drawButtons.length; i++) {
      drawButtons[i]._show()
    }
  }

  clickButton(btn: AoiButton) {
    if (btn.options.toggleStatus === true) {
      if (btn.options.tool === 'draw') {
        this._fsm.creating(btn.options.name)
      }
      if (btn.options.name === 'edit') {
        this._fsm.edit(btn.options.name)
      }
      if (btn.options.name === 'translate') {
        this._fsm.translate(btn.options.name)
      }
    }
    if (btn.options.toggleStatus === false) {
      if (btn.options.tool === 'draw') {
        this._fsm.remove()
      }
      if (btn.options.tool === 'edit') {
        //  btn.unselect();
        this._fsm.cancel()
      }
    }

    this.fireEvent('aoi:buttonClick', {
      name: btn.options.name
    } as AoiButtonEvent)
  }

  clickActionButton(btn: AoiButton, action: string) {
    btn.unselect()

    if (action === 'remove') {
      this._fsm.remove()
    }

    if (action === 'cancel') {
      this._fsm.cancel()
    }
    if (action === 'finish') {
      this._fsm.finish()
    }

    this.fireEvent('aoi:actionClick', {
      name: action,
      buttonName: btn.options.name
    } as AoiActionEvent)
  }

  _showDrawContainer(aoi: AoiControl) {
    const drawButtons = aoi._getDrawButtons()

    for (let i = 0; i < drawButtons.length; i++) {
      if (this._fsm.isNone() === true) {
        if (i !== 0) {
          drawButtons[i]._show()
        }
      }
    }
  }

  _hideDrawContainer(aoi: AoiControl) {
    const drawButtons = aoi._getDrawButtons()

    for (let i = 0; i < drawButtons.length; i++) {
      if (this._fsm.isNone() === true) {
        if (i !== 0) {
          drawButtons[i]._hide()
        }
      }
    }
  }

  _showEditContainer() {
    L.DomUtil.removeClass(this._editContainer, 'hidden')
  }

  _hideEditContainer() {
    L.DomUtil.addClass(this._editContainer, 'hidden')
  }
}

export const aoiControl = function (map: L.Map, options: AoiControlOptions) {
  return new AoiControl(map, options)
}
