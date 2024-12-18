import {LitElement, html, css} from 'lit'
import {customElement, property} from 'lit/decorators.js'
import { Icons, IconStyles, SizeProp } from '../Icons'

const defaultIcon = Icons.unknown


@customElement("a-icon")
export class AriesIcon extends LitElement {
    @property({attribute: true})
    icon: string = "fa-solid fa-circle"

    @property()
    size: SizeProp = "1x"

    static override get styles() {
        return [
            IconStyles,
            css`
                :host(.fast-glow) {
                    background-image: radial-gradient(rgba(255, 255, 255, 0.25) 5%, rgba(255, 255, 255, 0) 65%, rgba(255, 255, 255, 0) 100%);
                }
                svg {
                    filter: drop-shadow(0 0 var(--text-glow-distance) var(--text-glow));
                }
            `
        ]
    }

    override render() {
        const icon = Icons[this.icon] ?? defaultIcon
        // TODO: add support for `size`
        return html`${icon}`
    }
}