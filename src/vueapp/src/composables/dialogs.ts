import { markRaw } from 'vue'
import type { DynamicDialogInstance, DynamicDialogOptions } from 'primevue/dynamicdialogoptions'
import type { DialogState } from '@/stores/dialog'

type DialogInstance = {
  open: (content: any, options?: DynamicDialogOptions | undefined) => DynamicDialogInstance
}

export function createDialogInstance(
  dialog: DialogInstance,
  state: DialogState
): DynamicDialogInstance {
  const dialogRef = dialog.open(state.content, {
    props: {
      header: state.header,
      style: {
        width: state.width ?? '50rem'
      },
      closable: false,
      modal: true
    },
    templates: {
      footer: state.footer ?? markRaw(state.footer)
    }
  })

  return dialogRef
}
