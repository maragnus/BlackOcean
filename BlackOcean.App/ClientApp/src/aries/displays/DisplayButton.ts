import { html } from 'lit'
import { customElement, property } from 'lit/decorators.js'
import { DisplayElement } from './DisplayElement'
import { ifDefined } from 'lit/directives/if-defined.js'
import { Button } from '../../controlpanel/ControlPanel'
import "../components/AriesButton"
import { controlPanelManager } from "../../controlpanel/ControlPanelManager"

const defaultButton: Button = {
    name: "N/A",
    icon: "none",
    pressed: false,
    available: false,
    toggle: false
}

@customElement("display-button")
export class DisplayButton extends DisplayElement {   
    @property({attribute: true, type: String})
    icon: string | undefined = undefined

    @property({attribute: false})
    button: Button | undefined

    protected override callback(_key: string, value: unknown): void {
        this.button = value as Button
        this.requestUpdate()
    }

    private handleTrigger(e: CustomEvent) {
        e.preventDefault()
        if (this.source === undefined) return
        const value = !(this.button?.pressed ?? false)
        controlPanelManager.sendAction(`${this.source}.pressed`, value)
    }

    override render() {
        const b = this.button ?? defaultButton
        const disabled = this.button === undefined ? true : !this.button.available
        return html`<a-button icon=${ifDefined(this.icon)} ?active=${b.pressed} ?toggle=${b.toggle} ?disabled=${disabled} @trigger=${this.handleTrigger}><slot></slot></a-button>`
    }
}