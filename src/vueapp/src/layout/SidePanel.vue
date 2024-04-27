<template>
  <div class="sidepanel-container">
    <div class="sidepanel-top-container">
      <PrimeButton
        class="sidepanel-button"
        raised
        @click="onMenuToggle"
        :icon="isSidebarOpen ? 'pi pi-angle-left' : 'pi pi-angle-right'"
      />
      <div v-for="(item, index) in topItems" :key="index">
        <PrimeButton
          class="sidepanel-button"
          @click="itemClick(index)"
          v-tooltip.left="item.tooltip"
          :icon="item.icon"
        />
      </div>
    </div>

    <div class="sidepanel-bottom-container">
      <PrimeButton
        class="sidepanel-button"
        @click="showCharts"
        v-tooltip.left="'Charts'"
        icon="pi pi-chart-bar"
      />
      <PrimeButton
        class="sidepanel-button"
        @click="showConnectionClick"
        v-tooltip.left="'Connection'"
        icon="pi pi-sign-in"
      />
      <PrimeButton
        class="sidepanel-button"
        @click="nextClick"
        v-tooltip.left="'Settings'"
        icon="pi pi-cog"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useDialogStore } from '@/stores/dialog'
import { useLayoutStore } from '@/stores/layout'
import { storeToRefs } from 'pinia'
import TheCharts from '@/components/dialogs/TheCharts.vue'
import DialogFooter from '@/components/dialogs/DialogFooter.vue'

const layoutStore = useLayoutStore()
const { setSidebarItem, onMenuToggle } = layoutStore
const { isSidebarOpen } = storeToRefs(layoutStore)
const dialogStore = useDialogStore()

const itemClick = (index: number) => {
  setSidebarItem(index)
}

const topItems = [
  {
    to: '/sidepanel/taskresults',
    icon2: 'pi pi-fw pi-truck',
    icon: 'pi pi-truck',
    tooltip: 'TaskResults'
  },
  {
    to: '/sidepanel/satellites',
    icon2: 'pi pi-fw pi-star-fill',
    icon: 'pi pi-send',
    tooltip: 'Satellites'
  },
  {
    to: '/sidepanel/groundstations',
    icon2: 'pi pi-fw pi-wifi',
    icon: 'pi pi-wifi',
    tooltip: 'GroundStations'
  },
  {
    to: '/sidepanel/groundtargets',
    icon: 'pi pi-map-marker',
    tooltip: 'GroundTargets'
  }
]

const showConnectionClick = () => {
  //   completed.value = false
  //   dialogStore.open({
  //     header: 'Add Planned Schedule',
  //     content: AddPlannedSchedule,
  //     footer: DialogFooter,
  //     enableNext: false
  //   })
}

const nextClick = () => {
  //   dialogStore.open({
  //     header: 'Settings',
  //     content: TheSettings,
  //     footer: DialogFooter,
  //     width: '70rem'
  //   })
}

const showCharts = () => {
  dialogStore.open({
    header: 'Select Charts',
    content: TheCharts,
    footer: DialogFooter,
    enableNext: false
  })
}
</script>

<style scoped></style>
