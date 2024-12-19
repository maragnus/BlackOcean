import { html } from 'lit'
import { customElement, property } from 'lit/decorators.js'
import { PoweredSystem, Status } from '../../controlpanel/ControlPanel'
import { DisplayElement } from './DisplayElement'
import '../components/AriesEngineeringSlider'
import { ifDefined } from 'lit/directives/if-defined.js'
import "./DisplayButton"
import { controlPanelManager } from '../../controlpanel/ControlPanelManager'

const emptySystem: PoweredSystem = {
    available: false,
    abbreviation: "N/A",
    currentHeat: 0,
    currentLevel: 0,
    currentOutput: 0,
    heatBands: [ { value: 0, status: Status.None } ],
    icon: "none",
    levelStatuses: [ Status.Safe, Status.Safe, Status.Safe, Status.Warn, Status.Danger ],
    maxLevel: 5,
    minLevel: 1,
    name: "Not Installed",
    nominalOutput: 0,
    operating: false,
    powered: {
        name: "N/A",
        available: false,
        icon: "",
        pressed: false,
        toggle: true
    },
    auto: {
        name: "N/A",
        available: false,
        icon: "",
        pressed: false,
        toggle: true
    },
    setLevel: 1
}

@customElement("display-system")
export class DisplaySystem extends DisplayElement {
    @property({attribute: true, type: String})
    icon: string | undefined = undefined

    @property({attribute: true, type: String})
    label: string = "SYS"

    @property({attribute: true, type: String})
    sup: string | undefined = undefined

    @property({attribute: false})
    system: PoweredSystem | undefined

    protected override callback(_key: string, value: unknown): void {
        const system = value as PoweredSystem
        this.system = system
        this.requestUpdate()
    }

    private handleChange(e: CustomEvent): void {
        if (this.source === undefined) return;
        controlPanelManager.sendAction(`${this.source}.setLevel`, e.detail.value)
    }

    override render() {
        const g = this.system ?? emptySystem
        const label = this.sup ? html`${this.label}<sup>${this.sup}</sup>` : html`${this.label}`
        const disabled = !g.available || g.auto.pressed
        const value = g.available ? g.setLevel : 0

        return html`<aries-engineering-slider @change=${this.handleChange} ?disabled=${disabled} ?available=${g.available}
            min=${g.minLevel} max=${g.maxLevel} value=${value} 
            watts=${g.currentOutput} maxwatts=${g.nominalOutput} 
            progress=${g.currentLevel} heat=${ifDefined(g.currentHeat)} icon=${ifDefined(this.icon)}>
            <display-button source=${`${this.source}.powered`}>${label}</display-button></aries-engineering-slider>`
    }
}