<template>
  <AppLayout />
  <PrimeDynamicDialog />
</template>

<script setup lang="ts">
import AppLayout from '@/layout/AppLayout.vue'
import { usePlannedScheduleStore } from './stores/plannedSchedule'
import { useDialogStore } from '@/stores/dialog'
import { onBeforeMount, watch } from 'vue'
import { useDialog } from 'primevue/usedialog'
import type { DynamicDialogInstance } from 'primevue/dynamicdialogoptions'
import { createDialogInstance } from '@/composables/dialogs'
import { storeToRefs } from 'pinia'

const dialog = useDialog()
const dialogStore = useDialogStore()
const { activeState } = storeToRefs(dialogStore)
const psStore = usePlannedScheduleStore()

let dialogRef: DynamicDialogInstance | undefined | null

watch(activeState, (state) => {
  //console.log("activeState change");

  dialogRef?.close()

  if (state) {
    dialogRef = createDialogInstance(dialog, state)
  }
})

onBeforeMount(async () => {
  //await psStore.getPlannedSchedule('public/ps_demo.json')
  await psStore.getPlannedSchedule()
})
</script>

<style scoped></style>
