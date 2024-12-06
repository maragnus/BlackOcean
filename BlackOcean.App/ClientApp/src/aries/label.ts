import {LitElement, PropertyValues, html, css} from 'lit';
import {customElement, property} from 'lit/decorators.js';
import './icon';
import { TextStyles } from './styles';

@customElement("a-label")
export class AriesLabel extends LitElement {
    @property({attribute: true})
    typo: "normal" | "info" | "warn" | "error" | "title" | "dim" = "normal";

    @property({attribute: true})
    size: "xs" | "sm" | "md" | "lg" | "xl" = "md";

    @property({attribute: true, type: Boolean})
    center: boolean = false;

    @property({attribute: true})
    icon: string | undefined = undefined;

    static override get styles() {
        return [
            TextStyles,
            css`
                :host {
                    display: flex;
                    flex-direction: row;
                    font-size: var(--font-size);
                    gap: 0.5ch;
                }
                .text {
                    flex: 1;
                    text-transform: uppercase;       
                }
                sup {
                    font-size: 66%;
                    vertical-align: 30%;
                }
            `
        ]
    }

    protected override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('typo') || changedProperties.has('size') || changedProperties.has('center'))
            this.className = `text-${this.typo} text-${this.size}` + (this.center ? ' text-center' : '');
    }

    override render() {
        const icon = this.icon ? html`<a-icon icon=${this.icon}></a-icon>` : undefined;
        return html`${icon}<div class="text"><slot></slot></div>`;
    }
}