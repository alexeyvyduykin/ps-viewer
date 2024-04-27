import type { PlannedScheduleObject } from '@/types'
import { random } from 'lodash'

export type TasksRes = {
  observations: any[]
  communications: any[]
}

export function getDemo_tasks_2(ps_demo: PlannedScheduleObject, dt?: number): TasksRes {
  const ps = ps_demo as PlannedScheduleObject
  const tasks = ps.observationTaskResults //.slice(0, 20)

  //console.log('len: ' + `${tasks.length}`)

  const data1: any[] = []
  const data2: any[] = []

  //data.push(['satellite', 'task', 'type', 'begin', 'end'])

  for (let i = 0; i < tasks.length; i++) {
    const task = tasks[i]

    const satellite = task.satelliteName
    const name = task.taskName

    const begin = new Date(task.interval.begin)

    const end = new Date(begin.getTime() + task.interval.duration * 1000 + (dt ?? 0) * 1000)

    const res = random(1, 10)

    if (res > 7) {
      data2.push([satellite, name, begin, end])
    } else {
      data1.push([satellite, name, begin, end])
    }
  }

  return {
    observations: data1,
    communications: data2
  }
}
