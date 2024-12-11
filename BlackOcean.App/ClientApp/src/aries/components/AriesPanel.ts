import {LitElement, html, css, PropertyValues} from 'lit';
import {customElement, property} from 'lit/decorators.js';

@customElement("aries-panel")
export class AriesPanel extends LitElement {

    @property({attribute: true})
    area: string = "";

    @property({attribute: true})
    layout: "row" | "column" = 'column';

    static override get styles() {
        return css`
            :host {
                flex: 1;
                display: flex;
                grid-area: var(--grid-area);
                flex-direction: var(--layout);
                border: 2px solid var(--border-light);
                border-radius: var(--panel-border-radius);
                box-shadow: 
                    0 0 var(--panel-bloom-size) var(--panel-bloom-color), 
                    inset 0 0 var(--panel-bloom-size) var(--panel-bloom-color);
            }

            ::slotted(:first-child),
            ::slotted(:last-child) {
                padding-left: var(--edge-gap);
                padding-right: var(--edge-gap);
            }
            ::slotted(:not(:first-child)) {
                border-top: 1px solid var(--border-light);
            }
        `;
    }

    protected override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('area'))
            this.style.setProperty('--grid-area', this.area);

        if (changedProperties.has('layout'))
            this.style.setProperty('--layout', this.layout);
    }

    override render() {
        // let header = this.querySelectorAll('[slot="header"]').length > 0
        //     ? html`<div class="header"><slot name="header"></slot></div>`
        //     : nothing;

        // let footer = this.querySelectorAll('[slot="footer"]').length > 0
        //     ? html`<div class="footer"><slot name="footer"></slot></div>`
        //     : nothing;

        return html`<slot></slot>`;
    }
}
