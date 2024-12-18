import {LitElement, html, css, PropertyValues, nothing} from 'lit'
import {customElement, property} from 'lit/decorators.js'
import "./AriesIcon"
import { ButtonStyles } from '../Styles'

@customElement("a-button")
export class AriesButton extends LitElement {
    @property({attribute: true, type: Boolean})
    disabled: boolean = false

    @property({attribute: true, type: Boolean})
    active: boolean = false

    @property({attribute: true, type: Boolean})
    fill: boolean = false

    @property({attribute: true, type: Boolean})
    toggle: boolean = false

    @property({attribute: true, type: String})
    icon: string | undefined = undefined

    @property({attribute: true, type: String})
    on_icon: string = "fas fa-circle"

    @property({attribute: true, type: String})
    off_icon: string = "fal fa-circle"

    static override get styles() {
        return ButtonStyles
    }

    override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('fill'))
            this.classList.toggle('fill', this.fill)

        if (changedProperties.has('toggle'))
            this.classList.toggle('toggle', this.toggle)

        if (changedProperties.has('active'))
            this.classList.toggle('active', this.active)
    }

    private handleClick(e: Event) {
        e.preventDefault()
        if (this.toggle)
            this.active = !this.active
        const event = new CustomEvent('trigger', { detail: { toggle: this.active } })
        this.dispatchEvent(event)
    }

    override render() {
        const startIcon = this.icon 
            ? html`<div class="start-icon"><a-icon icon=${this.icon}></a-icon></div>` 
            : nothing
        
            const endIcon = this.toggle
            ? html`<div class="end-icon"><a-icon icon=${this.active ? this.on_icon : this.off_icon}></a-icon></div>`
            : nothing

        return html`
        <button type="button" @click=${this.handleClick} ?disabled=${this.disabled}>
            ${startIcon}
            <div class="label"><slot></slot></div>
            ${endIcon}
        </button>`
    }
}
