import * as L from 'leaflet'

export interface ScaleBarOptions extends L.ControlOptions {
  isCompact?: boolean | undefined
  maxWidth?: number | undefined
}

export class ScaleBarControl extends L.Control {
  declare _options: ScaleBarOptions
  declare _map: L.Map | undefined
  private _location: L.LatLng | undefined
  private _locationChanged: boolean = false
  private _coordsContainer: HTMLDivElement | undefined
  private _scaleContainer: HTMLSpanElement | undefined
  private _scaleRect: HTMLDivElement | undefined

  constructor(options: ScaleBarOptions | undefined) {
    options = {
      isCompact: options?.isCompact ?? true,
      position: options?.position ?? 'bottomleft',
      maxWidth: options?.maxWidth ?? 100
    }

    super(options)

    this._options = options
  }

  onAdd(map: L.Map) {
    const className = 'leaflet-control-scaleBar'
    const container = L.DomUtil.create('div', className)

    this._coordsContainer = L.DomUtil.create('div', `${className}-coords`, container)
    this._coordsContainer.textContent = this._convert(new L.LatLng(0, 0))

    if (this._options.isCompact === false) {
      this._scaleContainer = L.DomUtil.create('span', `${className}-scale`, container)
      const rectContainer = L.DomUtil.create('div', `${className}-rect-container`, container)
      this._scaleRect = L.DomUtil.create('div', `${className}-rect`, rectContainer)
    }

    map.on('mousemove', this._onMouseMove, this)
    map.on('mouseout', this._onMouseOut, this)
    map.on('move', this._onMove, this)

    map.whenReady(this._onMove, this)

    return container
  }

  onRemove(map: L.Map) {
    map.off('mousemove', this._onMouseMove, this)
    map.off('mouseout', this._onMouseOut, this)
    map.off('move', this._update2, this)
  }

  _onMouseMove(event: L.LeafletMouseEvent) {
    this._location = event.latlng

    if (this._locationChanged === false) {
      this._locationChanged = true
      requestAnimationFrame(() => this._update())
    }
  }

  _onMouseOut() {
    this._location = undefined

    if (this._locationChanged === false) {
      this._locationChanged = true
      requestAnimationFrame(() => this._update())
    }
  }

  _onMove() {
    if (this._options.isCompact === false) {
      requestAnimationFrame(() => this._update2())
    }
  }

  _update() {
    if (this._locationChanged === false) {
      return
    }

    this._locationChanged = false

    if (this._coordsContainer && this._location) {
      this._coordsContainer.textContent = this._convert(this._location)
    }
  }

  _update2() {
    if (this._map) {
      const map = this._map
      const y = map.getSize().y / 2

      const maxMeters = map.distance(
        map.containerPointToLatLng([0, y]),
        map.containerPointToLatLng([this._options.maxWidth!, y])
      )

      this._updateScales(maxMeters)
    }
  }

  _updateScales(maxMeters: number) {
    if (maxMeters) {
      this._updateMetric(maxMeters)
    }
  }

  _updateMetric(maxMeters: number) {
    if (this._scaleContainer && this._scaleRect) {
      const meters = this._getRoundNum(maxMeters),
        label = meters < 1000 ? `${meters} m` : `${meters / 1000} km`

      const ratio = meters / maxMeters
      this._scaleRect.style.width = `${Math.round(this._options.maxWidth! * ratio)}px`
      this._scaleContainer.textContent = label
    }
  }

  _getRoundNum(num: number) {
    const pow10 = Math.pow(10, `${Math.floor(num)}`.length - 1)
    let d = num / pow10

    d = d >= 10 ? 10 : d >= 5 ? 5 : d >= 3 ? 3 : d >= 2 ? 2 : 1

    return pow10 * d
  }

  _convert(location: L.LatLng): string {
    const { lng, lat } = location
    const lonStr =
      lng >= 0.0
        ? `${this._pad(lng.toFixed(3), 7)}°E`
        : `${this._pad(Math.abs(lng).toFixed(3), 7)}°W`
    const latStr =
      lat >= 0.0
        ? `${this._pad(lat.toFixed(3), 6)}°N`
        : `${this._pad(Math.abs(lat).toFixed(3), 6)}°S`
    return `${lonStr} ${latStr}`
  }

  _pad(num: string, size: number) {
    let numStr = num
    while (numStr.length < size) {
      numStr = ' ' + numStr
    }
    return numStr
  }
}

export const scaleBar = function (options: ScaleBarOptions | undefined) {
  return new ScaleBarControl(options)
}
