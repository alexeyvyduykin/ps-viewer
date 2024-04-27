<template>
  <div class="dialog-timelinechart-wrapper">
    <div class="flex flex-column">
      <div v-for="item of satellites" :key="item" class="flex align-items-center">
        <PrimeCheckbox v-model="selectedSatellites" :inputId="item" name="category" :value="item" />
        <label class="ml-2" :for="item">{{ item }}</label>
      </div>

      <PrimeButton @click="btnClick">BtnClick</PrimeButton>
      <PrimeButton @click="sortClick">Sort</PrimeButton>
      <PrimeButton @click="zoomClick">Zoom</PrimeButton>
      <PrimeButton @click="checkedClick">enableOverview: {{ checked }}</PrimeButton>
      <div>{{ resDate }}</div>
      <div>{{ selectedSegment }}</div>
    </div>

    <div v-if="isLoading">Loading...</div>
    <div ref="timelineId" style="width: 75rem; max-height: 50rem"></div>
  </div>
</template>

<script setup lang="ts">
import TimelinesChart from 'timelines-chart'
import { type Group, type Line, type Segment } from 'timelines-chart'

//import TimelinesChart from 'node_modules/timelines-chart/dist/timelines-chart'
// import {
//   type Group,
//   type Line,
//   type Segment,
//   type TS,
//   type Val,
//   type TimelinesChartInstance,
// } from "node_modules/timelines-chart/dist/timelines-chart";

import { onMounted, ref } from 'vue'
import { useChartTimelines } from '@/composables/chartTimelines'
import { whenever } from '@vueuse/core'
import { getRandomInt } from '@/utils'
import { useDialogStore } from '@/stores/dialog'

const dialogStore = useDialogStore()
const { data, isLoading, satellites, selectedSatellites, update } = useChartTimelines()

const timelineId = ref<HTMLElement | undefined>()
const selectedSegment = ref()

let chart = TimelinesChart()

whenever(data, (data) => {
  chart.data(data)
})

const resDate = ref()
const checked = ref(false)

const checkedClick = () => {
  checked.value = !checked.value
  chart.enableOverview(checked.value)
}

const btnClick = () => {
  if (data.value) {
    const amax = data.value.length
    const a = getRandomInt(0, amax)

    const group = data.value[a] as Group

    const bmax = group.data.length
    const b = getRandomInt(0, bmax)
    const line = group.data[b] as Line

    const cmax = line.data.length
    const c = getRandomInt(0, cmax)
    const segment = line.data[c] as Segment
    const res = segment.timeRange[0]

    resDate.value = res

    chart.dateMarker(res) //.data(data.value);
    chart.refresh()
  }
}

const sortClick = () => {
  chart.sortAlpha(checked.value)
}

const zoomClick = () => {
  chart.zoomY([1, 2])
}

dialogStore.$onAction((s) => {
  if (s.name === 'next') {
    dialogStore.close()
  }
})

onMounted(() => {
  chart(timelineId.value!)
    .width(1000)
    .maxLineHeight(60)
    .leftMargin(150)
    //.zScaleLabel("My Scale Units")
    .zQualitative(true)
  chart.onSegmentClick(f1)
  update()
})

function f1(segment: Segment) {
  selectedSegment.value = segment
}
</script>

<style scoped></style>
