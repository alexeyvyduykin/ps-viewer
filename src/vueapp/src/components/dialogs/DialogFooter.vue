<template>
  <div class="flex justify-content-between">
    <PrimeButton
      v-if="activeState?.enableBack ?? true"
      style="width: 6.25rem"
      :label="activeState?.labelBack ?? 'Cancel'"
      @click="back"
    />
    <div v-if="activeState?.enableNext ?? true">
      <PrimeButton
        :label="activeState?.labelNext ?? 'Ok'"
        style="width: 6.25rem"
        @click="next"
        autofocus
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useDialogStore } from '@/stores/dialog'

const dialogStore = useDialogStore()
const { back, next } = dialogStore
const { activeState } = storeToRefs(dialogStore)
</script>
