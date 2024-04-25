import type { Point } from 'geojson'

export function toStringFormat(point: Point) {
  const lon = point.coordinates[0]
  const lat = point.coordinates[1]
  return lon + '° ' + lat + '°'
}
