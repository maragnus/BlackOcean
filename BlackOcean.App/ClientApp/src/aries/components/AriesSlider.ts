import {LitElement, html, PropertyValues, TemplateResult} from 'lit'
import {ref, Ref, createRef} from 'lit/directives/ref.js'
import {customElement, property, query, queryAll} from 'lit/decorators.js'
import "./AriesIcon"
import { SliderStyles } from '../Styles'

const icons: Record<number, string> = {
    [1]: "level-overdrive",
    [2]: "level-boost",
    [3]: "level-standard",
    [4]: "level-standby",
    [5]: "level-hibernate",
}

@customElement("aries-slider")
export class AriesSlider extends LitElement {    
    @property({attribute: true, type: Boolean})
    available: boolean = false

    @property({attribute: true, type: Boolean})
    disabled: boolean = false

    @property({attribute: true, type: Number})
    min: number = 0

    @property({attribute: true, type: Number})
    max: number = 5

    @property({attribute: true, type: Number})
    value: number = 3

    @property({attribute: true, type: Number})
    progress: number | undefined = undefined

    @query(".label > div")
    label!: Element

    @property({attribute: true})
    box: string = "safe"

    @queryAll(".box")
    boxes!: Array<Element>

    inputRef: Ref<HTMLInputElement> = createRef()

    static override get styles() {
        return SliderStyles
    }

    private getBoxHeight(): number {
        return 100 / (this.max - this.min + 1)
    }

    protected override updated(changedProperties: PropertyValues): void {
        if (changedProperties.has('min') || changedProperties.has('max'))
            this.style.setProperty('--box-height', `${this.getBoxHeight()}%`)

        if (changedProperties.has('value')) {
            const selectedIndex = Number.parseInt(this.inputRef.value!.value) - this.min
            this.style.setProperty('--label-top', `${this.getBoxHeight() * selectedIndex}%`)
        }

        if (changedProperties.has('progress')) {
            const progress = this.progress === undefined ? 0 : (this.progress % 1)
            this.style.setProperty('--progress', `${progress * 100}%`)
        }
    }

    private handleChange() {
        if (!this.inputRef.value) return
        const slider = this.inputRef.value!
        const value = Number.parseInt(slider.value)
        this.value = value
        this.dispatchEvent(new CustomEvent("change", { detail: { value: value } }))
    }

    private getBoxes(classNames: string[]): TemplateResult[] {
        const boxCount = this.max - this.min + 1
        return Array.from({ length: boxCount }, (_, index) => index + this.min)
                .map(index => this.getBox(index, this.value, classNames))
    }

    private getBox(index: number, value: number, classNames: string[]): TemplateResult {
        const colorClass = classNames[Math.min(index - this.min, classNames.length)]
        const activeClass = (value > index) ? "active"
            : (Math.floor(value) === index) ? "current" : ""
        const className = `box ${activeClass} ${colorClass}`
        return html`<div class=${className}></div>`
    }

    
    private getProgressBox(index: number, value: number, classNames: string[]): TemplateResult {
        const colorClass = classNames[Math.min(index - this.min, classNames.length)]

        if (index < value + 0.05) {
            return html`<div class=${`box active ${colorClass}`}></div>`
        }
        else if (index > value + 0.95) {
            return html`<div class=${`box inactive ${colorClass}`}></div>`
        }

        return html`
            <div class=${`box inactive ${colorClass}`}>
                <div class=${`box active partial ${colorClass}`}></div>
            </div>`
    }

    private getProgress(classNames: string[]) {
        if (this.progress === undefined) return undefined
        
        const boxCount = this.max - this.min + 1
        const boxes = Array.from({ length: boxCount }, (_, index) => index + this.min)
            .map(index => this.getProgressBox(index, this.progress!, classNames))        

        return html`<div class="progress">${boxes}</div>`
    }

    override render() {
        const classNames = this.box.split(/[\s]+/)
        const className = classNames[Math.min(this.value - this.min, classNames.length)]

        const boxes = this.getBoxes(classNames)
        const progress = this.getProgress(classNames)

        let value: TemplateResult | undefined
        if (!this.available) {
            value = undefined
        } else if (this.min == 1 && this.max == 5) {
            const valueIconIndex = Math.min(5, Math.max(1, this.value ?? 1))
            value = html`<div class="label"><div></div><a-icon icon=${icons[valueIconIndex]}></a-icon></div></div>`
        } else {
            value = html`<div class="label"><div></div>${this.value}</div></div>`
        }

        const input = this.available 
            ? html`<input ${ref(this.inputRef)} type="range" class=${className} ?disabled=${this.disabled}
                min=${this.min} max=${this.max} .value=${this.value.toString()}
                @change=${this.handleChange} @input=${this.handleChange} style="top:-2px;bottom:-2px">`
            : undefined

        return html`
            <div class="slider">
                ${input}
                ${boxes}
                ${value}
            </div>
            ${progress}
        `
    }
}
