import { html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { PoweredSystem, Status } from '../../controlpanel/ControlPanel';
import { DisplayElement } from './DisplayElement';
import '../components/AriesEngineeringSlider';
import { ifDefined } from 'lit/directives/if-defined.js';

@customElement("display-system")
export class DisplaySystem extends DisplayElement {
    @property({attribute: true, type: String})
    icon: string | undefined = undefined;

    @property({attribute: true, type: String})
    label: string = "SYS";

    @property({attribute: true, type: String})
    sup: string | undefined = undefined;

    @property({attribute: false})
    system: PoweredSystem | undefined;

    protected override callback(_key: string, value: unknown): void {
        const system = value as PoweredSystem;
        this.system = system;
        this.requestUpdate()
    }

    static emptySystem: PoweredSystem = {
        abbreviation: "N/A",
        currentHeat: 0,
        currentLevel: 0,
        currentOutput: 0,
        heatBands: [ { value: 0, status: Status.Safe } ],
        icon: "fal fa-link-simple-slash",
        levelStatuses: [ Status.Safe, Status.Safe, Status.Safe, Status.Warn, Status.Danger ],
        maxLevel: 5,
        minLevel: 1,
        name: "Not Installed",
        nominalOutput: 0,
        operating: false,
        powered: false,
        setLevel: 1
    }

    override render() {
        const g = this.system ?? DisplaySystem.emptySystem;
        console.log(this.label, g)
        return html`<aries-engineering-slider ?active=${g.powered} toggle label=${this.label} sup=${ifDefined(this.sup)} 
            watts=${g.currentOutput} maxwatts=${g.nominalOutput} min=${g.minLevel} max=${g.maxLevel} value=${g.setLevel} 
            progress=${g.currentLevel} icon=${ifDefined(this.icon)}></aries-engineering-slider>`
    }
}