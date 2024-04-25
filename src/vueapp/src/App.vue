<template>
  <AppLayout />
  <PrimeDynamicDialog />
</template>

<script setup lang="ts">
import AppLayout from '@/layout/AppLayout.vue'
import { usePlannedScheduleStore } from './stores/plannedSchedule'
import { onBeforeMount } from 'vue'

const psStore = usePlannedScheduleStore()

onBeforeMount(async () => {
  //await psStore.getPlannedSchedule('public/ps_demo.json')
  await psStore.getPlannedSchedule()
})
</script>

<style scoped></style>
