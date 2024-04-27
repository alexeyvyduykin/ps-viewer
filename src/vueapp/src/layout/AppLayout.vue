<template>
  <div>
    <div class="layout-wrapper" :class="containerClass">
      <div class="sidepanel-wrapper">
        <SidePanel />
        <div class="sidepanel-content" v-if="isSidebarOpen">
          <TaskResultTab v-if="isSidePanelTabActive(0)" />
          <SatelliteTab v-else-if="isSidePanelTabActive(1)" />
          <GroundStationTab v-else-if="isSidePanelTabActive(2)" />
          <GroundTargetTab v-else-if="isSidePanelTabActive(3)" />
        </div>
      </div>
      <div class="layout-main-container">
        <TheMap />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useLayoutStore } from '@/stores/layout'
import SidePanel from './SidePanel.vue'
import TaskResultTab from '@/components/sidepanel/TaskResult.vue'
import SatelliteTab from '@/components/sidepanel/Satellite.vue'
import GroundStationTab from '@/components/sidepanel/GroundStation.vue'
import GroundTargetTab from '@/components/sidepanel/GroundTarget.vue'
import TheMap from '@/components/TheMap.vue'
import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useTaskResultStore } from '@/stores/taskResults'
import { useSatelliteStore } from '@/stores/satellites'
import { useGroundStationStore } from '@/stores/groundStations'
import { useGroundTargetStore } from '@/stores/groundTargets'

const layoutStore = useLayoutStore()
const { isSidePanelTabActive } = layoutStore
const { isSidebarOpen } = storeToRefs(layoutStore)

const containerClass = computed(() => {
  return {
    'layout-close': !isSidebarOpen.value,
    'layout-open': isSidebarOpen.value
  }
})

useTaskResultStore().getObservationTaskResults()
useSatelliteStore().getSatellites()
useGroundStationStore().getGroundStations()
useGroundTargetStore().getGroundTargets()
</script>

<style scoped></style>
