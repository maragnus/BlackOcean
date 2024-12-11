import { html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { PoweredSystem } from '../../controlpanel/ControlPanel';
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

    override render() {
        const g = this.system
        if (g === undefined)
            return html`<aries-engineering-slider label="N/A" icon="fal fa-link-simple-slash" value=0 progress=0></aries-engineering-slider>`
        else
            return html`<aries-engineering-slider ?active=${g.powered} toggle label=${this.label} sup=${ifDefined(this.sup)} 
                watts=${g.currentOutput} maxwatts=${g.nominalOutput} min=${g.minLevel} max=${g.maxLevel} value=${g.setLevel} 
                progress=${g.currentLevel} icon=${ifDefined(this.icon)}></aries-engineering-slider>`
    }
}