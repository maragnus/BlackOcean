import {LitElement, html, css, PropertyValues} from 'lit';
import {customElement, property} from 'lit/decorators.js';

@customElement("aries-grid")
export class AriesGrid extends LitElement {

    @property({attribute: true})
    template : string = "";

    static override get styles() {
        return css`
            :host {
                flex: 1;
                display: grid;
                margin: var(--grid-gap);
                gap: var(--grid-gap);
                grid-template-columns: repeat(var(--column-count), 1fr);
                grid-template-rows: repeat(var(--row-count), 1fr);
                grid-template-areas: var(--areas);
            }
        `;
    }

    protected override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('template')) {
            const rows = this.template.split(/\s*,\s*/);
            const columns = rows[0].split(/\s+/);
            const template = rows.join("\" \"");
            this.style.setProperty('--column-count', `${columns.length}`)
            this.style.setProperty('--row-count', `${rows.length}`)
            this.style.setProperty('--areas', `"${template}"`);
        }
    }

    override render() {
        return html`<slot></slot>`;
    }

    // override createRenderRoot() {
    //     return this;
    // }
}
