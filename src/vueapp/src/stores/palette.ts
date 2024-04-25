import { computed, reactive } from 'vue'
import { defineStore } from 'pinia'
import chroma from 'chroma-js'
import type { Dictionary } from 'lodash'

export const usePaletteStore = defineStore('palette', () => {
  const dict = reactive<Dictionary<string>>({})
  const palette = computed(() => chroma.brewer.Set1)
  const huePaletteType = computed(() => chroma.brewer.Purples)

  function pickColor(key: string): string {
    if (dict[key] !== undefined) {
      return dict[key]
    }

    // TODO: change length searching method
    let len = 0
    for (const item in dict) {
      len++
    }

    if (palette.value.length > len) {
      dict[key] = palette.value[len]
      return dict[key]
    }

    dict[key] = chroma.random().hex()

    return dict[key]
  }

  function pickPalette(index: number, count: number): string {
    const minCount = 3
    const maxCount = 9
    const num = Math.max(minCount, Math.min(maxCount, count))
    const i = Math.max(0, Math.min(num - 1, index))
    return chroma.scale(huePaletteType.value).colors(num)[num - i - 1]
  }

  return { palette, dict, pickColor, pickPalette }
})
