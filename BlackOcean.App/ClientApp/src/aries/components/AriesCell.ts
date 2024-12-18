import {LitElement, html, css} from 'lit'
import {customElement, property} from 'lit/decorators.js'
import {FlexLayoutStyles} from '../Styles'

@customElement("a-cell")
export class AriesCell extends LitElement {

    @property({attribute: true, type: Boolean})
    fill: boolean = false

    @property({attribute: true})
    layout: "none" | "row" | "column" = "none";
    
    @property({attribute: true})
    justify: "start" | "center" | "end" | "space-between" | "space-around" | "space-evenly" | "stretch" = "start";

    @property({attribute: true})
    align: "start" | "center" | "end" | "space-between" | "space-around" | "space-evenly" | "stretch" = "start";
    
    @property({attribute: true, type: Boolean})
    content: boolean = false

    @property({attribute: true, type: Boolean})
    center: boolean = false

    @property({attribute: true, type: Boolean})
    endcap: boolean = false

    override updated(): void {
        const justify = this.center ? 'center' : this.justify

        this.className = 
            (this.fill ? "f-fill " : "")
            + (this.content || this.layout === 'none' ? "f-content " : "")
            + (this.endcap ? "f-endcap " : "")
            + `f-${this.layout} `
            + `fjc-${justify} `
            + `fac-${this.align}`
    }

    static override get styles() {
        return [
            FlexLayoutStyles,
            css`
                :host(.f-content) {
                    padding: var(--cell-padding);
                }
                :host(.f-row) ::slotted(*:not(:first-child)) {
                    border-left: 1px solid var(--border-light);
                }
                :host(.f-column) ::slotted(*:not(:first-child)) {
                    border-top: 1px solid var(--border-light);
                }
                :host(.f-endcap) {
                    padding-left: var(--edge-gap);
                    padding-right: var(--edge-gap);
                }
            `
        ];
    }

    override render() {
        return html`<slot></slot>`
    }
}
