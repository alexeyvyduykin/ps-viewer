import type { Point } from 'geojson'
import type { Interval } from '@/types'

export function toStringFormat(point: Point) {
  const lon = point.coordinates[0]
  const lat = point.coordinates[1]
  return lon + '° ' + lat + '°'
}

export function getRandomInt(min: number, max: number) {
  min = Math.ceil(min)
  max = Math.floor(max)
  return Math.floor(Math.random() * (max - min) + min) // The maximum is exclusive and the minimum is inclusive
}

export function getFromInterval(interval: Interval) {
  const begin = interval.begin
  const duration = interval.duration
  const dateBegin = new Date(begin)
  const dateEnd = new Date(begin)
  dateEnd.setSeconds(dateBegin.getSeconds() + duration)

  return {
    begin: dateBegin,
    end: dateEnd
  }
}
