<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<template>
  <div class="flex flex-column">
    <div class="flex align-items-center p-3 font-bold">
      <div class="rect mr-2" :style="{ backgroundColor: color }"></div>
      <div class="text-xl">{{ item.name }}</div>
      <div class="flex flex-1 justify-content-between">
        <PrimeButton class="ml-2 my-3 max-h-2rem max-w-2rem" rounded text @click="infoToggle">
          <template #icon>
            <span class="pi pi-info-circle text-xl" />
          </template>
        </PrimeButton>

        <PrimeToggleButton
          class="m-2 border-circle"
          v-model="states[name].visible"
          onLabel=""
          offLabel=""
        >
          <template #icon>
            <span v-if="states[name].visible" class="pi pi-eye" />
            <span v-else class="pi pi-eye-slash" />
          </template>
        </PrimeToggleButton>

        <PrimeOverlayPanel ref="info">
          <div class="flex flex-column gap-1">
            <div class="font-bold text-xl">Orbit</div>
            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Semiaxis</div>
              <label class="col">{{ item.semiaxis }}</label>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Eccentricity</div>
              <label class="col">{{ item.eccentricity }}</label>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Inclination</div>
              <label class="col">{{ item.inclinationDeg }} deg</label>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Argument of perigee</div>
              <label class="col flex align-items-center">{{ item.argumentOfPerigeeDeg }} deg</label>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">RAAN</div>
              <div class="col">
                {{ Number(item.rightAscensionAscendingNodeDeg).toFixed(2) }} deg
              </div>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Longitude of the ascending node</div>
              <div class="col flex align-items-center">
                {{ item.longitudeAscendingNodeDeg }} deg
              </div>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Period</div>
              <label class="col">{{ item.period }} sec</label>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Epoch</div>
              <label class="col">{{ item.epoch }}</label>
            </div>

            <div class="font-bold text-xl">Sensor</div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Look angle</div>
              <label class="col">{{ item.lookAngleDeg }} deg</label>
            </div>

            <div class="grid ml-2">
              <div class="col-fixed" style="width: 120px">Radar angle</div>
              <label class="col">{{ item.radarAngleDeg }} deg</label>
            </div>
          </div>
        </PrimeOverlayPanel>
      </div>
    </div>

    <div class="p-3" v-if="states[name].visible">
      <div class="grid align-items-center">
        <div class="col-11">
          <PrimeSlider v-model="states[name].node" :min="minNode" :max="maxNode" />
        </div>
        <label class="col-1 flex justify-content-center">{{ states[name].node }}</label>
      </div>

      <div class="grid align-items-center">
        <label class="col-11">Track</label>
        <div class="col-1 flex justify-content-center">
          <PrimeCheckbox v-model="states[name].isTrack" :binary="true" />
        </div>
      </div>

      <div class="grid align-items-center">
        <label class="col-11">Left swath</label>
        <div class="col-1 flex justify-content-center">
          <PrimeCheckbox v-model="states[name].isLeftSwath" :binary="true" />
        </div>
      </div>

      <div class="grid align-items-center">
        <label class="col-11">Right swath</label>
        <div class="col-1 flex justify-content-center">
          <PrimeCheckbox v-model="states[name].isRightSwath" :binary="true" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Satellite } from '@/types'
import { computed, ref } from 'vue'
import { useTrackStore } from '@/stores/layers/tracks'
import { storeToRefs } from 'pinia'
import { usePaletteStore } from '@/stores/palette'
import { usePlannedScheduleStore } from '@/stores/plannedSchedule'

const props = defineProps({
  item: {
    type: Object as () => Satellite,
    required: true
  }
})
const trackStore = useTrackStore()
const psStore = usePlannedScheduleStore()
const { minNode, maxNode } = storeToRefs(psStore)

const name = computed(() => props.item.name)

const paletteStore = usePaletteStore()

const { pickColor } = paletteStore
const { states } = storeToRefs(trackStore)
const color = computed(() => pickColor(name.value))

const info = ref()

const infoToggle = (event: Event) => {
  info.value.toggle(event)
}
</script>

<style scoped>
.rect {
  width: 1rem;
  height: 2rem;
}
</style>
