<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <div v-if="isLoading">
    <PrimeProgressSpinner></PrimeProgressSpinner>
  </div>
  <div v-else>
    <PrimeDataView class="overflow-y-auto h-screen" :value="gss">
      <template #list="slotProps">
        <div class="grid grid-nogutter">
          <div v-for="item in slotProps.items" :key="item.name" class="col-12">
            <GroundStationItem v-bind:item="item" />
          </div>
        </div>
      </template>
    </PrimeDataView>
  </div>
</template>

<script setup lang="ts">
import GroundStationItem from '@/components/sidepanel/GroundStationItem.vue'
import { useGroundStationStore } from '@/stores/groundStations'
import { storeToRefs } from 'pinia'

const store = useGroundStationStore()
const { gss, isLoading } = storeToRefs(store)

store.getGroundStations()

//const { update } = store;

// const controller = new AbortController();
// const signal = controller.signal;

// onMounted(async () => {
//   const promise = new Promise((resolve) => {
//     setTimeout(() => {
//       resolve(update(signal));
//     }, 2000);
//   });

//   await promise.then();

//   //await updateAsync(signal);
// });

// onUnmounted(() => {
//   controller.abort();
// });
</script>

<style scoped lang="scss"></style>
