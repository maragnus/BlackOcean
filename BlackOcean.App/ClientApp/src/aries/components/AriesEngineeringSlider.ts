import {LitElement, html, css} from 'lit'
import {customElement, property} from 'lit/decorators.js'
import {ifDefined} from 'lit/directives/if-defined.js'
import "./AriesStack"
import "./AriesLabel"
import "./AriesButton"
import "./AriesSlider"
import { TextStyles } from '../Styles'
import { scaleValue } from '../Unit'

@customElement("aries-engineering-slider")
export class AriesEngineeringSlider extends LitElement {
    @property({attribute: true, type: Boolean})
    active: boolean = false

    @property({attribute: true, type: Boolean})
    toggle: boolean = false

    @property({attribute: true, type: String})
    icon: string | undefined = undefined;

    @property({attribute: true, type: String})
    label: string = "SYS";

    @property({attribute: true, type: String})
    sup: string | undefined = undefined;

    @property({attribute: true, type: Number})
    watts: number = 0;

    @property({attribute: true, type: Number})
    maxwatts: number = 120;

    @property({attribute: true, type: Number})
    min: number = 1;

    @property({attribute: true, type: Number})
    max: number = 5;

    @property({attribute: true, type: Number})
    value: number = 1;

    @property({attribute: true, type: Number})
    progress: number | undefined = undefined

    static override get styles() {
        return [
            TextStyles,
            css`
                :host {
                    flex-grow: 1;
                    display: flex;
                    flex-direction: column;
                    gap: 8px;
                }
            `]
    }

    override render() {
        const wattLabelClass =
            (this.watts < this.maxwatts * 0.1) ? "dim" :
                (this.watts > this.maxwatts ? "error" : "warn");

        const icon = this.icon ? html`
            <a-stack layout="row" justify="center">
                <a-icon icon=${this.icon} size="xl"></a-icon>
            </a-stack>
            ` : undefined;

        const sup = this.sup ? html`<sup>${this.sup}</sup>` : undefined
        const watts = scaleValue(this.watts, "watt", undefined, 2)
        const maxWatts = scaleValue(this.maxwatts, "watt", undefined, 2)

        return html`
            ${icon}
            <a-stack layout="row" justify="center" fill>
                <aries-slider min=${this.min} max=${this.max} value=${this.value} progress=${ifDefined(this.progress)} box="safe safe safe warn danger"></aries-slider>
            </a-stack>
            <a-stack layout="column">
                <a-label center typo=${wattLabelClass} size="sm">${watts.displayValue} ${watts.unitAbbreviation}</a-label>
                <a-label center typo="info" size="sm">${maxWatts.displayValue} ${maxWatts.unitAbbreviation}</a-label>
            </a-stack>
            <a-stack layout="row" justify="center">
                <a-button ?toggle=${this.toggle} ?active=${this.active}>${this.label}${sup}</a-button>
            </a-stack>
        `;
    }
}
