import type { Point, LineString, Geometry, FeatureCollection } from 'geojson'

export interface PlannedScheduleObject {
  name: string
  dateTime: string
  satellites: Satellite[]
  groundTargets: GroundTarget[]
  groundStations: GroundStation[]
  observationTasks: ObservationTask[]
  communicationTasks: CommunicationTask[]
  observationWindows: TimeWindow[]
  communicationWindows: TimeWindow[]
  observationTaskResults: ObservationTaskResult[]
  communicationTaskResults: CommunicationTaskResult[]
}

export interface Satellite {
  name: string
  semiaxis: number
  eccentricity: number
  inclinationDeg: number
  argumentOfPerigeeDeg: number
  longitudeAscendingNodeDeg: number
  rightAscensionAscendingNodeDeg: number
  period: number
  epoch: string
  lookAngleDeg: number
  radarAngleDeg: number
}

export enum GroundTargetType {
  Point,
  Route,
  Area
}

export interface GroundTarget {
  name: string
  type: GroundTargetType
  center: Point
  points: Geometry
}

export interface GroundStation {
  name: string
  center: Point
  angles: number[]
}

export interface Interval {
  begin: string
  duration: number
}

export interface TimeWindow {
  taskName: string
  satelliteName: string
  windows: Interval[]
}

export interface TaskResult {
  name: string
  taskName: string
  satelliteName: string
  interval: Interval
  node: number
  transition: Interval | undefined
}

export enum CommunicationType {
  Uplink,
  Downlink
}

export interface CommunicationTaskResult extends TaskResult {
  type: CommunicationType
}

export enum SwathDirection {
  Left,
  Right
}

export interface FootprintGeometry {
  center: Point
  border: LineString
}

export interface ObservationTaskResult extends TaskResult {
  targetName: string
  geometry: FootprintGeometry
  direction: SwathDirection
}

export interface Task {
  name: string
}

export interface CommunicationTask extends Task {
  groundStationName: string
}

export interface ObservationTask extends Task {
  groundTargetName: string
}

export interface Dictionary<T> {
  [key: string]: T
}

export interface NestedDictionary<T> {
  [key: string | number]: T | NestedDictionary<T>
}

export interface Coordinate {
  x: number
  y: number
  z: number
}

export interface FeatureMap {
  [key: string]: FeatureCollection
}

export interface NodeFeatureMap {
  [key: number]: FeatureMap
}
