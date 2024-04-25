import { ref } from 'vue'
import { defineStore } from 'pinia'

export const useLayoutStore = defineStore('layout', () => {
  const isSidebarOpen = ref(true)
  const selectedItem = ref(0)

  function setSidebarItem(item: any) {
    selectedItem.value = item.value || item
  }

  function isSidePanelTabActive(index: number): boolean {
    return selectedItem.value === index
  }

  function onMenuToggle() {
    isSidebarOpen.value = !isSidebarOpen.value
  }

  return {
    isSidebarOpen,
    selectedItem,
    onMenuToggle,
    setSidebarItem,
    isSidePanelTabActive
  }
})
