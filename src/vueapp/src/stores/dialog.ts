import { markRaw, ref } from 'vue'
import { defineStore } from 'pinia'

export interface DialogState {
  header: string
  content: any
  footer?: any
  enableBack?: boolean
  enableNext?: boolean
  labelBack?: string
  labelNext?: string
  width?: any
}

export const useDialogStore = defineStore('dialog', () => {
  const history = ref<DialogState[]>([])
  const activeState = ref<DialogState | null>(null)
  const nextState = ref<DialogState | null>(null)

  function createNext(state: DialogState): void {
    nextState.value = {
      header: state.header,
      content: markRaw(state.content),
      footer: markRaw(state.footer),
      enableBack: state.enableBack,
      enableNext: state.enableNext,
      labelBack: state.labelBack,
      labelNext: state.labelNext,
      width: state.width
    }
  }

  function next(): void {
    if (nextState.value) {
      activeState.value = {
        header: nextState.value.header,
        content: nextState.value.content,
        footer: nextState.value.footer,
        enableBack: nextState.value.enableBack,
        enableNext: nextState.value.enableNext,
        labelBack: nextState.value.labelBack,
        labelNext: nextState.value.labelNext,
        width: nextState.value.width
      }

      history.value.push(nextState.value)

      nextState.value = null
    }
  }

  function open(state: DialogState): void {
    activeState.value = {
      header: state.header,
      content: markRaw(state.content),
      footer: markRaw(state.footer),
      enableBack: state.enableBack,
      enableNext: state.enableNext,
      labelBack: state.labelBack,
      labelNext: state.labelNext,
      width: state.width
    }

    history.value.push(state)
  }

  function back(): void {
    history.value.pop()

    if (history.value.length === 0) {
      activeState.value = null
      nextState.value = null
      return
    }

    const state = history.value.slice(-1)[0]

    activeState.value = state
    nextState.value = null
  }

  function close(): void {
    history.value = []
    nextState.value = null
    activeState.value = null
  }

  return { activeState, nextState, history, createNext, open, next, back, close }
})
