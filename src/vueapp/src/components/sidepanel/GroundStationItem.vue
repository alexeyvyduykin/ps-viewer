<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<template>
  <div class="flex flex-column">
    <div class="flex align-items-center p-3">
      <div>
        <div class="text-xl font-bold">{{ item.name }}</div>
        <div class="text-sm">{{ toStringFormat(item.center) }}</div>
      </div>

      <div class="flex flex-1 justify-content-end">
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
      </div>
    </div>

    <div class="flex flex-column gap-3 p-3" v-if="states[name].visible">
      <div class="grid align-items-center">
        <label class="col">Range angles</label>
        <div>{{ angles }}</div>
      </div>

      <div class="grid align-items-center">
        <div class="col">
          <PrimeSlider
            :disabled="disableMode"
            v-model="states[name].rangeAngle"
            range
            :min="0"
            :max="45"
            :step="1"
          />
        </div>
      </div>

      <div class="grid align-items-center">
        <label class="col">Split method</label>
        <PrimeDropdown
          class="col-5 h-2rem"
          v-model="selectedCountMode"
          :options="availableCountModes"
          placeholder="Select a Count mode"
          :pt="{
            root: { class: 'p-0' },
            input: { class: 'px-3 py-1' }
          }"
        />
      </div>

      <div class="grid align-items-center">
        <label class="col">Count segments</label>

        <PrimeInputNumber
          :disabled="disableMode"
          class="col-5 h-2rem"
          v-model="states[name].areaCount"
          mode="decimal"
          showButtons
          buttonLayout="horizontal"
          inputId="horizontal-buttons"
          incrementButtonIcon="pi pi-plus"
          decrementButtonIcon="pi pi-minus"
          :min="1"
          :max="5"
          :pt="{
            root: { class: 'p-0' },
            input: { class: 'border-noround w-5' },
            incrementButton: { class: 'border-noround-left' },
            decrementButton: { class: 'border-noround-right' }
          }"
        />
      </div>
      <div class="flex flex-column">
        <div v-for="item in colorRects" :key="item.index">
          <div class="flex grid-nogutter align-items-center">
            <div
              class="col-fixed"
              :style="{ 'background-color': item.color, width: '60px', height: '20px' }"
            ></div>
            <div class="ml-3 mr-2">-</div>
            <div class="flex col-1 justify-content-end">
              {{ Number(item.angle).toFixed(2) }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { type ColorRect, createAngles } from '@/composables/groundStationHelper'
import { GroundStationAreaMode, useGroundStationLeafletStore } from '@/stores/layers/groundStations'
import { storeToRefs } from 'pinia'
import { usePaletteStore } from '@/stores/palette'
import type { GroundStation } from '@/types'
import { toStringFormat } from '@/utils/index'

const props = defineProps({
  item: {
    type: Object as () => GroundStation,
    required: true
  }
})

const name = computed(() => props.item.name)
const gsStore = useGroundStationLeafletStore()
const { pickPalette } = usePaletteStore()
const { resetToDefault } = gsStore
const { states } = storeToRefs(gsStore)

const availableCountModes = ref<string[]>(Object.values(GroundStationAreaMode))

const angles = computed(
  () =>
    states.value[name.value].rangeAngle[0] + '° - ' + states.value[name.value].rangeAngle[1] + '°'
)

const colorRects = computed(() => {
  const state = states.value[name.value]
  const inner = state.rangeAngle[0]
  const outer = state.rangeAngle[1]
  const areaCount = state.areaCount
  const mode = state.countMode
  let angles =
    mode === GroundStationAreaMode.None
      ? state.defaultAngles.slice(1)
      : createAngles(inner, outer, areaCount, mode)
  return angles.map(
    (angle, index) =>
      ({
        index: index,
        color: pickPalette(index, angles.length),
        angle: angle
      }) as ColorRect
  )
})

const selectedCountMode = computed({
  get() {
    return states.value[name.value].countMode
  },
  set(newValue) {
    if (newValue === GroundStationAreaMode.None) {
      resetToDefault(name.value)
      return
    }
    states.value[name.value].countMode = newValue
  }
})

const disableMode = computed(() => selectedCountMode.value === GroundStationAreaMode.None)
</script>

<style lang="scss" scoped></style>
