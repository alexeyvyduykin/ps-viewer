<template>
  <div>
    <div class="flex flex-column p-2" :style="{ height: '150px' }">
      <div class="flex">
        <PrimeButton
          class="flex-auto text-xl"
          type="button"
          :icon="isFilterOpen ? 'pi pi-times' : 'pi pi-sliders-h'"
          label="Filter options"
          @click="toggle"
        />
        <PrimeButton
          class="flex text-xl ml-2"
          :icon="isDirty ? 'pi pi-filter-slash ' : 'pi pi-filter'"
          @click="reset()"
        />
      </div>
      <PrimeOverlayPanel ref="op" @show="show" @hide="hide">
        <TaskResultFilter />
      </PrimeOverlayPanel>

      <div class="mt-2">
        <span class="flex p-input-icon-left">
          <i class="pi pi-search" />
          <PrimeInputText
            class="w-screen"
            v-model="searchString"
            @input="taskResultSearch"
            placeholder="Search"
          />
        </span>
      </div>

      <div class="flex mt-2">
        <PrimeSelectButton
          v-model="value"
          :options="options"
          optionLabel="value"
          dataKey="value"
          multiple
          aria-labelledby="multiple"
        >
          <template #option="slotProps">
            <i :class="slotProps.option.icon"></i>
          </template>
        </PrimeSelectButton>
        <PrimeSelectButton
          v-if="isTrack"
          class="ml-2"
          v-model="value2"
          :options="options2"
          optionLabel="value"
          dataKey="value"
          aria-labelledby="basic"
        >
          <template #option="slotProps">
            <i :class="slotProps.option.icon"></i>
          </template>
        </PrimeSelectButton>
      </div>
    </div>
    <div v-if="isLoading">
      <PrimeProgressSpinner></PrimeProgressSpinner>
    </div>

    <div v-else>
      <PrimeDataView
        :value="filteringTaskResults"
        :rows="30"
        paginator
        :pt="{
          content: {
            class: 'p-2'
          }
        }"
      >
        <template #list="slotProps">
          <PrimeScrollPanel :style="{ height: 'calc(100vh - 150px - 62px - 14px)' }">
            <div class="grid grid-nogutter">
              <div v-for="item in slotProps.items" :key="item.name" class="col-12">
                <TaskResultItem v-bind:item="item" />
              </div>
            </div>
          </PrimeScrollPanel>
        </template>
      </PrimeDataView>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import TaskResultItem from '@/components/sidepanel/TaskResultItem.vue'
import { useTaskResultStore } from '@/stores/taskResults'
import { useDebounceFn } from '@vueuse/core'
import TaskResultFilter from '@/components/sidepanel/TaskResultFilter.vue'
import { storeToRefs } from 'pinia'
import { usePreviewStore } from '@/stores/layers/preview'

const isFilterOpen = ref(false)
const store = useTaskResultStore()
const { reset } = store
const { isDirty, isLoading, searchString, filteringTaskResults } = storeToRefs(store)
const previewStore = usePreviewStore()
const { states, trackState } = storeToRefs(previewStore)

interface OptionItem {
  icon: string
  value: string
}

const options = ref<OptionItem[]>([
  { icon: 'pi pi-image', value: 'Footprint' },
  { icon: 'pi pi-arrow-up', value: 'Track' },
  { icon: 'pi pi-arrows-h', value: 'Swath' },
  { icon: 'pi pi-map-marker', value: 'GroundTarget' }
])

const options2 = ref([
  { icon: 'pi pi-globe', value: 'Full' },
  { icon: 'pi pi-share-alt', value: 'Segment' }
])

const value = computed({
  get() {
    return states.value.map(
      (s) =>
        ({
          value: s
        }) as OptionItem
    )
  },
  set(newValue: OptionItem[]) {
    states.value = newValue.map((s) => s.value)
  }
})

const isTrack = computed(() => value.value.some((s) => s.value === 'Track'))

const value2 = computed({
  get() {
    return { value: trackState.value } as OptionItem
  },
  set(newValue: OptionItem) {
    const isLeft = newValue.value === 'Full' ? 'Full' : 'Segment'
    if (trackState.value !== isLeft) {
      trackState.value = isLeft
    }
  }
})

store.getObservationTaskResults()

const debouncedFn = useDebounceFn(() => {
  store.getObservationTaskResults()
}, 1000)

function taskResultSearch() {
  debouncedFn()
}

const op = ref()

const show = () => {
  isFilterOpen.value = true
}
const hide = () => {
  isFilterOpen.value = false
}

const toggle = (event: Event) => {
  op.value.toggle(event)
}
</script>

<style scoped></style>
