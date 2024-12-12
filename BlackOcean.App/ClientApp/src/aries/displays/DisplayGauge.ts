import { html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { translateBands } from '../../controlpanel/ControlPanelManager';
import { Gauge, Scale, Status } from '../../controlpanel/ControlPanel';
import { DisplayElement } from './DisplayElement';
import '../components/AriesGauge';
import { Unit, UnitInterval } from '../Unit';
import { ifDefined } from 'lit/directives/if-defined.js'

@customElement("display-gauge")
export class DisplayGauge extends DisplayElement {
    @property({attribute: true, type: String})
    icon: string | undefined = undefined

    @property({attribute: false})
    gauge: Gauge | undefined;

    protected override callback(_key: string, value: unknown): void {
        const gauge = value as Gauge;
        this.gauge = gauge;
        this.requestUpdate()
    }

    static defaultGauge: Gauge = {
        bands: [ { value: 0, status: Status.Safe } ],
        interval: undefined,
        max: 100,
        min: 0,
        name: "N/A",
        scale: Scale.Linear,
        unit: "none",
        value: 0
    }

    override render() {
        const g = this.gauge ?? DisplayGauge.defaultGauge
        const interval = g.interval as UnitInterval
        const logarithmic = g.scale === Scale.Logarithmic

        // if (g === undefined)
            // return html`<a-gauge value="0" bands="safe" unit="none" icon="fal fa-link-simple-slash"><a-label>N/A</a-label></a-gauge>`
        // else
        return html`<a-gauge bands=${translateBands(g.bands)} min=${g.min} max=${g.max} unit=${g.unit as Unit} interval=${ifDefined(interval)} value=${g.value} ?logarithmic=${logarithmic} icon=${ifDefined(this.icon)}><slot></slot></a-gauge>`
    }
}