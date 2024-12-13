import { html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { translateBands, translateScale } from '../../controlpanel/ControlPanelManager';
import { Band, Gauge, Scale, Status } from '../../controlpanel/ControlPanel';
import { DisplayElement } from './DisplayElement';
import '../components/AriesGauge';
import { Unit } from '../Unit';
import { ifDefined } from 'lit/directives/if-defined.js'
import { GaugeScale } from '../components/AriesGauge';

const DEFAULT_BANDS: Band[] = [ { value: 0, status: Status.None } ];

@customElement("display-gauge")
export class DisplayGauge extends DisplayElement {
    @property({attribute: true, type: String})
    icon: string | undefined = undefined

    @property({attribute: false})
    gauge: Gauge | undefined;

    protected override callback(_key: string, value: unknown): void {
        this.gauge = value as Gauge;
        this.requestUpdate()
    }

    static defaultGauge: Gauge = {
        bands: DEFAULT_BANDS,
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
        const interval = g.interval as Unit
        const scale = translateScale(g.scale) as GaugeScale
        const bands = translateBands(g.bands)
        return html`<a-gauge bands=${bands} min=${g.min} max=${g.max} unit=${g.unit as Unit} interval=${ifDefined(interval)} value=${g.value} scale=${scale} icon=${ifDefined(this.icon)}><slot></slot></a-gauge>`
    }
}