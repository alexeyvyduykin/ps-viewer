/* eslint-disable @typescript-eslint/no-unused-vars */
import * as L from 'leaflet'
import { AoiControl } from './AoiControl'
import { getTranslation } from '@/utils/translation'

export type AoiButtonOptions = L.ControlOptions & {
  name: string
  title: string
  icon: string
  hidden?: boolean
  disable?: boolean
  toggleStatus: boolean
  actions: string[]
  tool: string
}

export type AoiAction = {
  name: string
  text: string
}

export type AoiButtonEvent = L.LeafletEvent & {
  name: string
}

export type AoiActionEvent = L.LeafletEvent & {
  name: string
  buttonName: string
}

export class AoiButton extends L.Control {
  declare options: AoiButtonOptions
  private _container: HTMLDivElement
  private _aoi: AoiControl
  private _btnContainer?: HTMLDivElement
  _map?: L.Map
  _actions: AoiAction[]

  constructor(aoi: AoiControl, container: HTMLDivElement, options: AoiButtonOptions) {
    super({ position: 'topleft' })
    this._aoi = aoi
    this._container = container
    this.options = options
    this._actions = []

    const actions: AoiAction[] = [
      {
        name: 'remove',
        text: getTranslation('actions.remove')
      },
      {
        name: 'cancel',
        text: getTranslation('actions.cancel')
      },
      {
        name: 'finish',
        text: getTranslation('actions.finish')
      }
    ]

    for (let i = 0; i < this.options.actions.length; i++) {
      const item = this.options.actions[i]

      const res = actions.filter((s) => s.name === item)[0]
      this._actions.push(res)
    }
  }

  onAdd(map: L.Map) {
    this._map = map

    let className = 'aoi-buttons-container'
    if (this.options.tool === 'draw') {
      className += ' leaflet-bar h'
    }

    this._btnContainer = L.DomUtil.create('div', className, this._container)

    const newButton = L.DomUtil.create('a', 'aoi-button', this._btnContainer)

    if (this.options.title) {
      newButton.setAttribute('title', this.options.title)
    }

    newButton.setAttribute('role', 'button')
    newButton.setAttribute('tabindex', '0')

    if (this.options.hidden) {
      this._hide()
    }

    const image = L.DomUtil.create('div', 'aoi-button-icon', newButton)

    L.DomUtil.addClass(image, this.options.icon)

    L.DomEvent.disableClickPropagation(newButton)
    L.DomEvent.on(newButton, 'click', L.DomEvent.stop)

    L.DomEvent.addListener(newButton, 'click', this._onBtnClick, this)

    const actionContainer = L.DomUtil.create('div', 'aoi-actions-container', this._btnContainer)

    this._actions.forEach((s) => {
      const name = s.name

      const actionNode = L.DomUtil.create('a', `aoi-action action-${name}`, actionContainer)
      actionNode.setAttribute('role', 'button')
      actionNode.setAttribute('tabindex', '0')

      actionNode.innerHTML = s.text

      L.DomEvent.disableClickPropagation(actionNode)
      L.DomEvent.on(actionNode, 'click', L.DomEvent.stop)

      const actionClick = () => {
        // is needed to prevent scrolling when clicking on a-element with href="a"
        //e.preventDefault();
        this._aoi.clickActionButton(this, name)
      }

      L.DomEvent.addListener(actionNode, 'click', actionClick, this)
    })

    if (this.options.toggleStatus) {
      L.DomUtil.addClass(this._btnContainer, 'select')
    }

    this._container.appendChild(this._btnContainer)

    return this._container
  }

  toggle(e?: any | boolean | undefined) {
    this.options.toggleStatus = !this.options.toggleStatus

    if (this.options.toggleStatus === false) {
      L.DomUtil.removeClass(this._btnContainer!, 'select')
    } else {
      L.DomUtil.addClass(this._btnContainer!, 'select')
    }
  }

  unselect() {
    if (this.options.toggleStatus === true) {
      this.options.toggleStatus = false
      L.DomUtil.removeClass(this._btnContainer!, 'select')
    }
  }

  disable() {
    this.options.disable = true
    L.DomUtil.addClass(this._btnContainer!, 'disable')
  }

  undisable() {
    this.options.disable = false
    L.DomUtil.removeClass(this._btnContainer!, 'disable')
  }

  _onBtnClick(e?: any | undefined) {
    if (e) {
      // is needed to prevent scrolling when clicking on a-element with href="a"
      //e.preventDefault();
    }

    if (this.options.disable !== true) {
      this.toggle()

      this._aoi.clickButton(this)
    }
  }

  _hide() {
    L.DomUtil.addClass(this._btnContainer!, 'hidden')
  }

  _show() {
    L.DomUtil.removeClass(this._btnContainer!, 'hidden')
  }

  _selectOne() {
    L.DomUtil.addClass(this._btnContainer!, 'select')
    L.DomUtil.removeClass(this._btnContainer!, 'hidden')
  }
}

export const aoiButton = function (
  aoi: AoiControl,
  container: HTMLDivElement,
  options: AoiButtonOptions
) {
  return new AoiButton(aoi, container, options)
}
