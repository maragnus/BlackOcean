import { html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { translateBands } from '../../controlpanel/ControlPanelManager';
import { Gauge } from '../../controlpanel/ControlPanel';
import { DisplayElement } from './DisplayElement';
import '../components/AriesGauge';
import { Unit } from '../Unit';

@customElement("display-gauge")
export class DisplayGauge extends DisplayElement {
    @property({attribute: false})
    gauge: Gauge | undefined;

    protected override callback(_key: string, value: unknown): void {
        const gauge = value as Gauge;
        this.gauge = gauge;
        this.requestUpdate()
    }

    override render() {
        const g = this.gauge
        if (g === undefined)
            return html`<a-gauge label="N/A" value="0" icon="fal fa-link-simple-slash" bands="safe" unit="none"></a-gauge>`
        else
            return html`<a-gauge label=${g.name} bands=${translateBands(g.bands)} min=${g.min} max=${g.max} unit=${g.unit as Unit} value=${g.value}></a-gauge>`
    }
}