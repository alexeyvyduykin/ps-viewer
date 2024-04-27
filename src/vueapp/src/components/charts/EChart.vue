<template>
  <div class="chart" ref="chartElementRef"></div>
</template>

<script setup lang="ts">
import * as echarts from 'echarts'
import { type ECharts, type EChartsOption } from 'echarts'
import { onMounted, ref } from 'vue'
import { getDemo_tasks_2 } from '@/composables/echarts'
import type {
  CustomSeriesRenderItem,
  CustomSeriesRenderItemAPI,
  CustomSeriesRenderItemParams
} from 'echarts'
import { usePlannedScheduleStore } from '@/stores/plannedSchedule'
import { storeToRefs } from 'pinia'
import { useDialogStore } from '@/stores/dialog'

const dialogStore = useDialogStore()
const psStore = usePlannedScheduleStore()
const { ps } = storeToRefs(psStore)

const data = getDemo_tasks_2(ps.value!)

const chartElementRef = ref<HTMLDivElement>()
let chart: ECharts | undefined
let isCross = false

onMounted(() => {
  if (chartElementRef.value) {
    chart = echarts.init(chartElementRef.value, 'dark', { renderer: 'canvas' })

    chart.resize({
      width: 1450,
      height: 750
    })

    const options2 = {} as EChartsOption
    options2.tooltip = {
      confine: true
    }
    options2.toolbox = {
      left: 20,
      top: 0,
      itemSize: 20,
      feature: {
        myCross: {
          show: true,
          title: 'Enable cross pointer',
          iconStatus: 'normal',
          icon: 'path://M990.55 380.08 q11.69 0 19.88 8.19 q7.02 7.01 7.02 18.71 l0 480.65 q-1.17 43.27 -29.83 71.93 q-28.65 28.65 -71.92 29.82 l-813.96 0 q-43.27 -1.17 -72.5 -30.41 q-28.07 -28.07 -29.24 -71.34 l0 -785.89 q1.17 -43.27 29.24 -72.5 q29.23 -29.24 72.5 -29.24 l522.76 0 q11.7 0 18.71 7.02 q8.19 8.18 8.19 18.71 q0 11.69 -7.6 19.29 q-7.6 7.61 -19.3 7.61 l-518.08 0 q-22.22 1.17 -37.42 16.37 q-15.2 15.2 -15.2 37.42 l0 775.37 q0 23.39 15.2 38.59 q15.2 15.2 37.42 15.2 l804.6 0 q22.22 0 37.43 -15.2 q15.2 -15.2 16.37 -38.59 l0 -474.81 q0 -11.7 7.02 -18.71 q8.18 -8.19 18.71 -8.19 l0 0 ZM493.52 723.91 l-170.74 -170.75 l509.89 -509.89 q23.39 -23.39 56.13 -21.05 q32.75 1.17 59.65 26.9 l47.94 47.95 q25.73 26.89 27.49 59.64 q1.75 32.75 -21.64 57.3 l-508.72 509.9 l0 0 ZM870.09 80.69 l-56.13 56.14 l94.72 95.9 l56.14 -57.31 q8.19 -9.35 8.19 -21.05 q-1.17 -12.86 -10.53 -22.22 l-47.95 -49.12 q-10.52 -9.35 -23.39 -9.35 q-11.69 -1.17 -21.05 7.01 l0 0 ZM867.75 272.49 l-93.56 -95.9 l-380.08 380.08 l94.73 94.73 l378.91 -378.91 l0 0 ZM322.78 553.16 l38.59 39.77 l-33.92 125.13 l125.14 -33.92 l38.59 38.6 l-191.79 52.62 q-5.85 1.17 -12.28 0 q-6.44 -1.17 -11.11 -5.84 q-4.68 -4.68 -5.85 -11.7 q-2.34 -5.85 0 -11.69 l52.63 -192.97 l0 0 Z',
          onclick: onDragSwitchClick,
          emphasis: {
            iconStyle: {
              borderColor: 'green'
            }
          }
        }
      }
    }

    options2.legend = {
      data: ['Observations', 'Communications'],
      orient: 'horizontal',
      left: 'center'
    }

    options2.xAxis = {
      type: 'time',
      position: 'top',
      splitLine: {
        lineStyle: {
          color: ['#E9EDFF']
        }
      },
      axisLine: {
        show: true
      },
      axisTick: {
        lineStyle: {
          color: '#929ABA'
        }
      },
      axisLabel: {
        color: '#929ABA',
        inside: false,
        align: 'center'
      }
    }

    options2.yAxis = {
      type: 'category'
    }

    options2.series = [
      {
        name: 'Observations',
        type: 'custom',
        renderItem: (p, api) => renderItem(p, api),
        dimensions: [
          { name: 'satellite' },
          { name: 'task' },
          { name: 'begin', type: 'time' },
          { name: 'end', type: 'time' }
        ],
        itemStyle: {
          color: 'red'
        },
        encode: {
          x: [2, 3],
          y: 0,
          tooltip: [2, 3],
          itemName: 1
        },
        data: data.observations
      } as echarts.CustomSeriesOption,
      {
        name: 'Communications',
        type: 'custom',
        renderItem: (p, api) => renderItem(p, api),
        dimensions: [
          { name: 'satellite' },
          { name: 'task' },
          { name: 'begin', type: 'time' },
          { name: 'end', type: 'time' }
        ],
        itemStyle: {
          color: 'green'
        },
        encode: {
          x: [2, 3],
          y: 0,
          tooltip: [2, 3],
          itemName: 1
        },
        data: data.communications
      } as echarts.CustomSeriesOption
    ]

    options2.dataZoom = [
      {
        type: 'slider',
        filterMode: 'weakFilter',
        showDataShadow: false,
        labelFormatter: ''
      },
      {
        type: 'inside',
        filterMode: 'weakFilter'
      }
    ]

    chart.setOption(options2)
  }
})

const renderItem: CustomSeriesRenderItem = (
  params: CustomSeriesRenderItemParams,
  api: CustomSeriesRenderItemAPI
) => {
  const categoryIndex = api.value(0)
  const start = api.coord([api.value(2), categoryIndex])
  const end = api.coord([api.value(3), categoryIndex])

  const sz = api.size!([0, 1]) as number[]
  const height = sz[1] * 0.6

  return {
    type: 'rect',
    transition: ['shape'],
    shape: {
      x: start[0],
      y: start[1] - height / 2,
      width: end[0] - start[0],
      height: height
    },
    style: {
      fill: api.visual('color')
      // fill: type === 'observations' ? 'red' : 'green'
    }
  }
}

const onDragSwitchClick = () => {
  isCross = !isCross
  chart!.setOption({
    tooltip: {
      axisPointer: {
        type: isCross ? 'cross' : 'none'
      }
    },
    toolbox: {
      feature: {
        myCross: {
          title: isCross ? 'Disable cross pointer' : 'Enable cross pointer',
          iconStyle: {
            borderColor: isCross ? 'green' : ''
          }
        }
      }
    }
  })
}

dialogStore.$onAction((s) => {
  if (s.name === 'next') {
    dialogStore.close()
  }
})
</script>

<style scoped>
.chart {
  background-color: azure;
  border: 0px solid blueviolet;
  border-radius: 8px;
}
</style>
