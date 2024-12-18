import {LitElement, html, css, unsafeCSS} from 'lit'
import {customElement, property} from 'lit/decorators.js'
import {library, parse as faParse, icon as faIcon, dom as faDom, IconLookup, SizeProp} from '@fortawesome/fontawesome-svg-core'
import {fal} from '@fortawesome/pro-light-svg-icons'
import {fas} from '@fortawesome/pro-solid-svg-icons'

library.add(fal)
library.add(fas)

@customElement("a-icon")
export class AriesIcon extends LitElement {
    @property({attribute: true})
    icon: string = "fa-solid fa-circle"

    @property()
    size: SizeProp = "1x"

    static defaultIcon: IconLookup = faParse.icon("fas fa-question-mark")

    static override get styles() {
        return [
            unsafeCSS(faDom.css()),
            css`
                :host(.fast-glow) {
                    background-image: radial-gradient(rgba(255, 255, 255, 0.25) 5%, rgba(255, 255, 255, 0) 65%, rgba(255, 255, 255, 0) 100%);
                }
                .glow {
                    filter: drop-shadow(0 0 var(--text-glow-distance) var(--text-glow));
                }
            `
        ]
    }

    override render() {
        const iconLookup = faParse.icon(this.icon) ?? AriesIcon.defaultIcon
        const iconHtml: HTMLCollection = faIcon(iconLookup).node
        const element = iconHtml.item(0)
        if (element) {
            element.classList.add(`fa-${this.size}`)
            element.classList.add(`glow`)
        }
        return html`${iconHtml}`
    }
}