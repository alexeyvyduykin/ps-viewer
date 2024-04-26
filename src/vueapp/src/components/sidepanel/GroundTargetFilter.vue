<template>
  <div style="width: 400px">
    <div class="flex justify-content-left">
      <div class="flex flex-column gap-2 ml-4">
        <div class="flex -ml-4">
          <PrimeCheckbox @click="checkboxClick" v-model="isSelected" binary>
            <template #icon>
              <span v-if="isIndeterminate" class="pi pi-circle-fill"></span>
            </template>
          </PrimeCheckbox>
          <label class="ml-2" for="checkbox">Types</label>
        </div>
        <div v-for="item of types" :key="item.key" class="flex align-items-center">
          <PrimeCheckbox
            v-model="selectedTypes"
            :inputId="item.key"
            name="category"
            :value="item.name"
          />
          <label class="ml-2" :for="item.key">{{ item.name }}</label>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { watch } from 'vue'
import { useGroundTargetStore } from '@/stores/groundTargets'
import { useDebounceFn } from '@vueuse/core'
import { useTriStateCheckbox } from '@/composables/triStateCheckbox'
import { storeToRefs } from 'pinia'

const store = useGroundTargetStore()

const { types, selectedTypes } = storeToRefs(store)

const { isSelected, isIndeterminate, checkboxClick } = useTriStateCheckbox(
  types,
  selectedTypes,
  (arr) => arr.map((s) => s.name)
)

const debouncedFn = useDebounceFn(() => {
  store.getGroundTargets()
}, 1000)

function filterChanged() {
  debouncedFn()
}
watch(selectedTypes, () => filterChanged())
</script>

<style scoped></style>
