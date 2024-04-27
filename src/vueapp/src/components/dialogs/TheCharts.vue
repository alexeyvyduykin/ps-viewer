<template>
  <div class="dialog-charts-wrapper">
    <PrimeButton class="btn" @click="showApexChart">
      <div>Apex Chart</div>
    </PrimeButton>
    <PrimeButton class="btn" @click="showGoogleChart">
      <div>Google Chart</div>
    </PrimeButton>
    <PrimeButton class="btn" @click="showTimelineChart">
      <div>Timelines Chart</div>
    </PrimeButton>
  </div>
</template>

<script setup lang="ts">
import { useDialogStore } from '@/stores/dialog'
//import ApexChart from '@/components/charts/ApexChart.vue'
//import GoogleChart from '@/components/charts/GoogleChart.vue'
import TimelineChart from '@/components/charts/TimelineChart.vue'
import DialogFooter from '@/components/dialogs/DialogFooter.vue'

const dialogStore = useDialogStore()

const showApexChart = () => {
  //   dialogStore.open({
  //     header: 'Apex Chart',
  //     content: ApexChart,
  //     footer: DialogFooter,
  //     width: '100rem'
  //   })
}
const showGoogleChart = () => {
  //   dialogStore.open({
  //     header: 'Google Chart',
  //     content: GoogleChart,
  //     footer: DialogFooter,
  //     width: '100rem'
  //   })
}
const showTimelineChart = () => {
  dialogStore.open({
    header: 'Timeline Chart',
    content: TimelineChart,
    footer: DialogFooter,
    width: '100rem'
  })
}
</script>

<style scoped></style>
