<template>
  <div class="taskresult-filter-container">
    <div class="column">
      <div>Node range</div>
      <div>{{ nodes[0] }} - {{ nodes[1] }}</div>
    </div>
    <div class="column">
      <PrimeSlider class="w-full" v-model="nodes" range :min="minNode" :max="maxNode" :step="1" />
    </div>
    <div class="column">
      <label>Left swath</label>
      <PrimeCheckbox v-model="isLeft" :binary="true" />
    </div>
    <div class="column">
      <label>Right swath</label>
      <PrimeCheckbox v-model="isRight" :binary="true" />
    </div>
    <div class="list-container">
      <div class="header">
        <PrimeCheckbox @click="checkboxClick" v-model="isSelected" binary>
          <template #icon>
            <span v-if="isIndeterminate" class="pi pi-circle-fill"></span>
          </template>
        </PrimeCheckbox>
        <label class="ml-2" for="checkbox">Satellites</label>
      </div>

      <div class="items" v-for="item of satellites" :key="item.key">
        <PrimeCheckbox
          v-model="selectedSatellites"
          :inputId="item.key"
          name="category"
          :value="item.name"
        />
        <label class="ml-2" :for="item.key">{{ item.name }}</label>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { watch } from 'vue'
import { useTaskResultStore, type SatelliteItem } from '@/stores/taskResults'
import { useDebounceFn } from '@vueuse/core'
import { useTriStateCheckbox } from '@/composables/triStateCheckbox'
import { storeToRefs } from 'pinia'
import { usePlannedScheduleStore } from '@/stores/plannedSchedule'

const store = useTaskResultStore()
const psStore = usePlannedScheduleStore()
const { minNode, maxNode } = storeToRefs(psStore)
const { nodes, isLeft, isRight, satellites, selectedSatellites } = storeToRefs(store)

const { isSelected, isIndeterminate, checkboxClick } = useTriStateCheckbox(
  satellites,
  selectedSatellites,
  (arr: SatelliteItem[]) => arr.map((s) => s.name)
)

const debouncedFn = useDebounceFn(() => {
  store.getObservationTaskResults()
}, 1000)

function filterChanged() {
  debouncedFn()
}

watch(nodes, () => filterChanged())
watch(isLeft, () => filterChanged())
watch(isRight, () => filterChanged())
watch(selectedSatellites, () => filterChanged(), { deep: true })
</script>

<style scoped></style>
