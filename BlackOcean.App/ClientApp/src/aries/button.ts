import {LitElement, html, css, PropertyValues, nothing} from 'lit';
import {customElement, property} from 'lit/decorators.js';
import "./icon";

@customElement("a-button")
export class AriesButton extends LitElement {
    @property({attribute: true, type: Boolean})
    active: boolean = false

    @property({attribute: true, type: Boolean})
    fill: boolean = false

    @property({attribute: true, type: Boolean})
    toggle: boolean = false

    @property({attribute: true, type: String})
    icon: string | undefined = undefined;

    @property({attribute: true, type: String})
    on_icon: string = "fas fa-circle";

    @property({attribute: true, type: String})
    off_icon: string = "fal fa-circle";

    static override get styles() {
        return css`
            :host(.fill) {
                flex: 1;
            }
            :host {
                display: contents;
            }
            button {
                font-family: var(--font-family);
                display: flex;
                align-items: center;
                border: 1px solid var(--border-light);
                border-radius: var(--button-right-radius) var(--button-left-radius) var(--button-left-radius) var(--button-right-radius);
                background: var(--button-background);
                padding: var(--button-padding);
                color: var(--text-color);
                text-shadow: 0 0 var(--text-glow-distance) var(--text-glow);
                background-image: radial-gradient(ellipse at center center, rgba(255, 255, 255, 0.15) 0%, rgba(255, 255, 255, 0.10) 24%, rgba(255, 255, 255, 0) 100%);
                box-shadow: 
                    0 0 var(--button-bloom-size) 0 var(--button-bloom-color),
                    inset 0 0 var(--button-bloom-size) 0 var(--button-bloom-color);
                transition: box-shadow 0.1s ease-in, border 0.1s ease-in;
                text-transform: uppercase;
                white-space: nowrap;
                min-height: 42px;
            }
            button:hover {
                border: 1px solid #fff;
                box-shadow: 
                    0 0 var(--button-bloom-size) 1px var(--button-hover-bloom-color),
                    inset 0 0 var(--button-bloom-size) 1px var(--button-hover-bloom-color);
            }
            button:active {
                border: 1px solid var(--button-active-border-color);
                background-color: var(--button-active-background-color);
                box-shadow: 0 0 var(--button-bloom-size) 1px var(--button-active-bloom-color);
            }
            :host(.toggle) button {
                border-radius: var(--button-right-radius);
            }
            :host(.active) button:not(:active) {
                background-color: var(--button-active-color);
            }
            .start-icon { 
                flex: 0;
                position: relative;
                left: -6px;
                display: block;
                padding: 0;
                margin: 0;
            }
            .end-icon { 
                flex: 0;
                position: relative;
                right: -6px;
                display: block;
                padding: 0;
                margin: 0;
            }
            .label {
                display: block;
                flex: 1;
                text-align: center;
            }
        `;
    }

    override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('fill'))
            this.classList.toggle('fill', this.fill);

        if (changedProperties.has('toggle'))
            this.classList.toggle('toggle', this.toggle);

        if (changedProperties.has('active'))
            this.classList.toggle('active', this.active);
    }

    private handleClick(e: Event) {
        e.preventDefault();
        if (this.toggle)
            this.active = !this.active;
    }

    override render() {
        const startIcon = this.icon 
            ? html`<div class="start-icon"><a-icon icon=${this.icon}></a-icon></div>` 
            : nothing;
        
            const endIcon = this.toggle
            ? html`<div class="end-icon"><a-icon icon=${this.active ? this.on_icon : this.off_icon}></a-icon></div>`
            : nothing;

        return html`
        <button type="button" @click=${this.handleClick}>
            ${startIcon}
            <div class="label"><slot></slot></div>
            ${endIcon}
        </button>`;
    }
}
