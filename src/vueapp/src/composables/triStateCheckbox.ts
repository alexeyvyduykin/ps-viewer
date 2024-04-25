import { type Ref, ref, watchEffect } from 'vue'

export function useTriStateCheckbox<T>(
  values: Ref<T[]>,
  selectedValues: Ref<string[]>,
  map: (context: T[]) => string[]
) {
  const isSelected = ref(false)
  const isIndeterminate = ref(false)

  watchEffect(() => {
    // HACK: trigger init triCheckBox
    console.log(`tri: ${selectedValues}`)

    if (selectedValues.value.length === 0) {
      isIndeterminate.value = false
      isSelected.value = false
    } else if (selectedValues.value.length === values.value.length) {
      isIndeterminate.value = false
      isSelected.value = true
    } else {
      isIndeterminate.value = true
      isSelected.value = true
    }
  })

  const checkboxClick = () => {
    isIndeterminate.value = false
    isSelected.value = !isSelected.value
    if (isSelected.value === false) {
      selectedValues.value = []
    } else if (isSelected.value === true) {
      selectedValues.value = [...map(values.value)]
    }
  }

  return {
    isSelected,
    isIndeterminate,
    checkboxClick
  }
}
