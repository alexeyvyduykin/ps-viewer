<template>
  <div>
    <div class="flex flex-column p-2" :style="{ height: '112px' }">
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
        <GroundTargetFilter />
      </PrimeOverlayPanel>

      <div class="mt-2">
        <span class="flex p-input-icon-left">
          <i class="pi pi-search" />
          <PrimeInputText
            class="w-screen"
            v-model="searchString"
            @input="gtsSearch"
            placeholder="Search"
          />
        </span>
      </div>
    </div>

    <div v-if="isLoading">
      <PrimeProgressSpinner />
    </div>

    <div v-else>
      <PrimeDataView
        paginator
        :value="filteringGts"
        :rows="30"
        :pt="{
          content: {
            class: 'p-2'
          }
        }"
      >
        <template #list="slotProps">
          <PrimeScrollPanel :style="{ height: 'calc(100vh - 112px - 62px - 14px)' }">
            <div class="grid grid-nogutter">
              <div v-for="item in slotProps.items" :key="item.name" class="col-12">
                <GroundTargetItem :item="item" />
              </div>
            </div>
          </PrimeScrollPanel>
        </template>
      </PrimeDataView>
    </div>
  </div>
</template>

<script setup lang="ts">
import GroundTargetItem from '@/components/sidepanel/GroundTargetItem.vue'
import { useGroundTargetStore } from '@/stores/groundTargets'
import { ref } from 'vue'
import { useDebounceFn } from '@vueuse/core'
import GroundTargetFilter from '@/components/sidepanel/GroundTargetFilter.vue'
import { storeToRefs } from 'pinia'

const store = useGroundTargetStore()
const { isLoading, isDirty, filteringGts, searchString } = storeToRefs(store)
const { reset } = store
const isFilterOpen = ref(false)

store.getGroundTargets()

const debouncedFn = useDebounceFn(() => {
  store.getGroundTargets()
}, 1000)

function gtsSearch() {
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

<style scoped lang="scss">
// .p-input-icon-left {
//   i {
//     margin-top: -8px;
//     font-size: 18px;
//   }
// }
</style>
