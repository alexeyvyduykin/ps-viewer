import { t, StateMachine, type Callback } from 'typescript-fsm'

export enum States {
  none = 0,
  creating,
  created,
  editing,
  translating
}

export enum Events {
  createStart = 100,
  createFinish,
  editStart,
  editFinish,
  translateStart,
  translateFinish,
  cancel,
  remove
}

export type ICallbacks = Record<Events, Callback> & {
  [Events.createStart]: (shape: string) => void
}

export class Aoi extends StateMachine<States, Events, ICallbacks> {
  private readonly _context: any
  private _shape?: string | undefined
  private _btn?: string | undefined

  constructor(context: any, init = States.none) {
    super(init)
    this._context = context

    this.addTransitions([
      t(States.none, Events.createStart, States.creating, () =>
        this.onCreating(this._context, this._shape!)
      ),

      t(States.creating, Events.createFinish, States.created, () =>
        this.onCreate(this._context, this._shape!)
      ),
      t(States.creating, Events.remove, States.none, () =>
        this.onRemove(this._context, this._shape!)
      ),

      t(States.created, Events.editStart, States.editing),
      t(States.created, Events.translateStart, States.translating),
      t(States.created, Events.remove, States.none, () => {
        this.onRemove(this._context, this._shape!)
      }),

      t(States.editing, Events.editFinish, States.created),
      t(States.editing, Events.cancel, States.created, (s: any) => this.onCancel(this._context, s)),
      t(States.editing, Events.remove, States.none, () => {
        this.onRemove(this._context, this._shape!)
      }),

      t(States.translating, Events.translateFinish, States.created),
      t(States.translating, Events.cancel, States.created, (s: any) =>
        this.onCancel(this._context, s)
      ),
      t(States.translating, Events.remove, States.none, () => {
        this.onRemove(this._context, this._shape!)
      })
    ])
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public onCreating(context: any, shape: string) {}

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public onCreate(context: any, shape: string) {}

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public onRemove(context: any, shape: string) {}

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public onCancel(context: any, btn: string) {}

  log(state?: string) {
    console.log('Aoi State: ' + `${state ?? States[this.getState()]}`)
  }

  async creating(shape: string) {
    this._shape = shape

    return this.dispatch(Events.createStart, shape)
  }

  async create() {
    return this.dispatch(Events.createFinish, this._shape!)
  }

  async remove() {
    this.cancel()

    return this.dispatch(Events.remove, this._shape!)
  }

  async edit(btn: string) {
    this.cancel()

    this._btn = btn

    return this.dispatch(Events.editStart)
  }

  async translate(btn: string) {
    this.cancel()

    this._btn = btn

    return this.dispatch(Events.translateStart)
  }

  async cancel() {
    if (this.can(Events.cancel) === true) {
      return this.dispatch(Events.cancel, this._btn!)
    }
  }

  async finish() {
    if (this.isEdit() === true) {
      return this.dispatch(Events.editFinish)
    }

    if (this.isTranslate() === true) {
      return this.dispatch(Events.translateFinish)
    }
  }

  isNone(): boolean {
    return this.getState() === States.none
  }

  isCreated(): boolean {
    return this.getState() === States.created
  }

  isEdit(): boolean {
    return this.getState() === States.editing
  }

  isTranslate(): boolean {
    return this.getState() === States.translating
  }
}
