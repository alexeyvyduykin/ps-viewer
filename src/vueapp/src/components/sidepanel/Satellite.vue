<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <div v-if="isLoading">
    <PrimeProgressSpinner></PrimeProgressSpinner>
  </div>
  <div v-else>
    <PrimeDataView class="overflow-y-auto h-screen" :value="satellites">
      <template #list="slotProps">
        <div class="grid grid-nogutter">
          <div v-for="item in slotProps.items" :key="item.name" class="col-12">
            <SatelliteItem v-bind:item="item" />
          </div>
        </div>
      </template>
    </PrimeDataView>

    <!-- <PrimeDataView class="overflow-y-auto h-screen" :value="store.satellites" data-key="name">
      <template #list="slotProps">
        <div class="col-12">
          <SidePanelSatelliteItem v-bind:item="slotProps.data" />
        </div>
      </template>
    </PrimeDataView>  -->
  </div>
</template>

<script setup lang="ts">
import SatelliteItem from '@/components/sidepanel/SatelliteItem.vue'
import { useSatelliteStore } from '@/stores/satellites'
import { storeToRefs } from 'pinia'

const store = useSatelliteStore()
const { isLoading, satellites } = storeToRefs(store)

//store.getSatellites()
</script>

<style scoped lang="scss"></style>
