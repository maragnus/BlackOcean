import {LitElement, html, PropertyValues} from 'lit';
import {customElement, property} from 'lit/decorators.js';
import {FlexLayoutStyles} from '../Styles';

@customElement("a-stack")
export class AriesStack extends LitElement {

    @property({attribute: true, type: Boolean})
    fill: boolean = false

    @property({attribute: true})
    layout: "row" | "column" = "column";
    
    @property({attribute: true})
    justify: "start" | "center" | "end" | "space-between" | "space-around" | "space-evenly" | "stretch" = "start";

    @property({attribute: true})
    align: "start" | "center" | "end" | "space-between" | "space-around" | "space-evenly" | "stretch" = "start";
    
    static override get styles() {
        return FlexLayoutStyles
    }

    override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('fill') 
            || changedProperties.has('layout')
            || changedProperties.has('justify')
            || changedProperties.has('align')
        ) {

        this.className = 
            (this.fill ? "f-fill " : "")
            + `f-${this.layout} `
            + `fjc-${this.justify} `
            + `fac-${this.align}`;
        }
    }

    override render() {
        return html`<slot></slot>`;
    }
}
