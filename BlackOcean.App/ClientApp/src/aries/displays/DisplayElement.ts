import { css, LitElement, PropertyValues } from "lit"
import { controlPanelManager } from "../../controlpanel/ControlPanelManager"
import { property } from "lit/decorators.js"

export abstract class DisplayElement extends LitElement {
    @property({ attribute: true })
    source: string | undefined

    @property({ attribute: false })
    sourceUnsubscribe: (() => void) | undefined

    static override get styles() {
        return css`:host { display: flex; }`
    }

    protected override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('source')) {
            // Unsubscribe from last source
            if (this.sourceUnsubscribe) 
                this.sourceUnsubscribe()

            // Unsubscribe to new source
            if (this.source !== undefined) {
                this.sourceUnsubscribe = controlPanelManager.subscribe(this.source, this.callback.bind(this))
                this.callback(this.source, controlPanelManager.getValue(this.source))
            }
        }
    }

    protected abstract callback(key: string, value: unknown): void
}
