import './assets/main.css'
import 'primevue/resources/themes/aura-light-green/theme.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'

import PrimeVue from 'primevue/config'

import BadgeDirective from 'primevue/badgedirective'
import Button from 'primevue/button'
import Card from 'primevue/card'
import Checkbox from 'primevue/checkbox'
import Column from 'primevue/column'
import ColumnGroup from 'primevue/columngroup'
import ConfirmDialog from 'primevue/confirmdialog'
import ConfirmPopup from 'primevue/confirmpopup'
import ConfirmationService from 'primevue/confirmationservice'
import DataView from 'primevue/dataview'
import DataViewLayoutOptions from 'primevue/dataviewlayoutoptions'
import Dialog from 'primevue/dialog'
import DialogService from 'primevue/dialogservice'
import Dropdown from 'primevue/dropdown'
import DynamicDialog from 'primevue/dynamicdialog'
import InputSwitch from 'primevue/inputswitch'
import InputText from 'primevue/inputtext'
import InputMask from 'primevue/inputmask'
import InputNumber from 'primevue/inputnumber'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'
import Listbox from 'primevue/listbox'
import Menu from 'primevue/menu'
import Menubar from 'primevue/menubar'
import OverlayPanel from 'primevue/overlaypanel'
import Paginator from 'primevue/paginator'
import Panel from 'primevue/panel'
import PanelMenu from 'primevue/panelmenu'
import Password from 'primevue/password'
import ProgressBar from 'primevue/progressbar'
import ProgressSpinner from 'primevue/progressspinner'
import RadioButton from 'primevue/radiobutton'
import Ripple from 'primevue/ripple'
import Row from 'primevue/row'
import SelectButton from 'primevue/selectbutton'
import ScrollPanel from 'primevue/scrollpanel'
import Slider from 'primevue/slider'
import StyleClass from 'primevue/styleclass'
import Textarea from 'primevue/textarea'
import ToastService from 'primevue/toastservice'
import Toolbar from 'primevue/toolbar'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import ToggleButton from 'primevue/togglebutton'
import Tooltip from 'primevue/tooltip'
import TriStateCheckbox from 'primevue/tristatecheckbox'

const app = createApp(App)

app.use(createPinia())
app.use(PrimeVue, { ripple: true })
app.use(ConfirmationService)
app.use(ToastService)
app.use(DialogService)

app.directive('tooltip', Tooltip)
app.directive('badge', BadgeDirective)
app.directive('ripple', Ripple)
app.directive('styleclass', StyleClass)

app.component('PrimeButton', Button)
app.component('PrimeCard', Card)
app.component('PrimeCheckbox', Checkbox)
app.component('PrimeColumn', Column)
app.component('PrimeColumnGroup', ColumnGroup)
app.component('PrimeConfirmDialog', ConfirmDialog)
app.component('PrimeConfirmPopup', ConfirmPopup)
app.component('PrimeDataView', DataView)
app.component('PrimeDataViewLayoutOptions', DataViewLayoutOptions)
app.component('PrimeDialog', Dialog)
app.component('PrimeDropdown', Dropdown)
app.component('PrimeDynamicDialog', DynamicDialog)
app.component('PrimeInputMask', InputMask)
app.component('PrimeInputNumber', InputNumber)
app.component('PrimeInputSwitch', InputSwitch)
app.component('PrimeInputText', InputText)
app.component('PrimeInputGroup', InputGroup)
app.component('PrimeInputGroupAddon', InputGroupAddon)
app.component('PrimeListbox', Listbox)
app.component('PrimeMenu', Menu)
app.component('PrimeMenubar', Menubar)
app.component('PrimeOverlayPanel', OverlayPanel)
app.component('PrimePaginator', Paginator)
app.component('PrimePanel', Panel)
app.component('PrimePanelMenu', PanelMenu)
app.component('PrimePassword', Password)
app.component('PrimeProgressBar', ProgressBar)
app.component('PrimeProgressSpinner', ProgressSpinner)
app.component('PrimeRadioButton', RadioButton)
app.component('PrimeRow', Row)
app.component('PrimeSelectButton', SelectButton)
app.component('PrimeScrollPanel', ScrollPanel)
app.component('PrimeSlider', Slider)
app.component('PrimeTabView', TabView)
app.component('PrimeTabPanel', TabPanel)
app.component('PrimeTextarea', Textarea)
app.component('PrimeToolbar', Toolbar)
app.component('PrimeToggleButton', ToggleButton)
app.component('PrimeTriStateCheckbox', TriStateCheckbox)

app.mount('#app')
