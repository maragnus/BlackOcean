import {LitElement, html, css, PropertyValues, TemplateResult} from 'lit'
import {ref, Ref, createRef} from 'lit/directives/ref.js'
import {customElement, property, query, queryAll} from 'lit/decorators.js'

@customElement("aries-slider")
export class AriesSlider extends LitElement {

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
        return css`
            :host {
                flex-grow: 1;
                display: flex;
                flex-direction: row;
                justify-content: center;
                /* padding-top: 3px;
                padding-bottom: 4px;
                padding-left: 0px;
                padding-right: 0px; */
                gap: 6px;
            }
            .slider, .progress {
                flex-grow: 1;
                position: relative;
                display: flex;
                flex-direction: column-reverse;
                gap: 6px;
            }
            .slider {
                width: 42px;
                max-width: 42px;
            }
            .progress {
                width: 12px;
                max-width: 12px;
            }
            .progress .box {
                border-radius: 4px;
            }
            .box {
                position: relative;                
                flex: 1;
                box-shadow: 0 0 var(--slider-box-inactive-bloom-size) var(--slider-box-active-safe);
                border: var(--slider-border);
                border-radius: 4px;
                background-color: var(--slider-inactive-safe);
                pointer-events: none;
            }
            .box.partial {
                position: absolute;
                bottom: 0;
                height: var(--progress);
                left: 0;
                right: 0;
            }
            .box.warn {
                background-color: var(--slider-inactive-warn);
                box-shadow: 0 0 var(--slider-box-inactive-bloom-size) var(--slider-active-warn)
            }
            .box.danger {
                background-color: var(--slider-inactive-danger);
                box-shadow: 0 0 var(--slider-box-inactive-bloom-size) var(--slider-active-danger)
            }
            .box.active.safe {
                background-color: var(--slider-active-safe);
            }
            .box.active.warn {
                background-color: var(--slider-active-warn);
            }
            .box.active.danger {
                background-color: var(--slider-active-danger);
            }
            .slider .box.current {
                background-color: transparent !important;
                box-shadow: none !important;
            }

            input[type="range"] {
                writing-mode: vertical-lr;
                direction: rtl;
                position: absolute;
                background: transparent;
                inset: 0;
                margin: 0;
                padding: 0;
            }

            input[type="range"],
            ::-webkit-slider-runnable-track,
            ::-webkit-slider-thumb {
                -webkit-appearance: none;
            }

            input[type="range"]::-webkit-slider-runnable-track {
                border: none;
                width: 42px;
                height: 100%;
                cursor: pointer;
            }

            input[type="range"]::-webkit-slider-thumb {
                position: relative;
                width: 100%;
                height: var(--box-height);
                border: 1px solid var(--border-light);
                border-radius: 8px;
                background-color: var(--slider-thumb-safe);
                background-image: radial-gradient(ellipse at center center, rgba(255, 255, 255, 0.15) 0%, rgba(255, 255, 255, 0.10) 24%, rgba(255, 255, 255, 0) 100%);
                box-shadow:
                    0 0 var(--button-bloom-size) 0 var(--button-bloom-color),
                    inset 0 0 var(--button-bloom-size) 0 var(--button-bloom-color);
                cursor: ns-resize;
            }

            input[type="range"].warn::-webkit-slider-thumb {
                background-color: var(--slider-thumb-warn);
                box-shadow:
                    0 0 var(--button-bloom-size) 0 var(--slider-active-warn),
                    inset 0 0 var(--button-bloom-size) 0 var(--slider-active-warn);
            }

            input[type="range"].danger::-webkit-slider-thumb {
                background-color: var(--slider-thumb-danger);
                box-shadow:
                    0 0 var(--button-bloom-size) 0 var(--slider-active-danger),
                    inset 0 0 var(--button-bloom-size) 0 var(--slider-active-danger);
            }

            .label {
                display: flex;
                align-items: center;
                justify-content: center;
                pointer-events: none;
                position: absolute;
                bottom: var(--label-top);
                width: 100%;
                height: var(--box-height);
            }
        `;
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

        if (index < value + 0.1) {
            return html`<div class=${`box active ${colorClass}`}></div>`
        }
        else if (index > value + 0.9) {
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

        return html`
            <div class="slider">
                <input ${ref(this.inputRef)} type="range" class=${className} min=${this.min} max=${this.max} .value=${this.value.toString()}
                    @change=${this.handleChange} @input=${this.handleChange} style="top:-2px;bottom:-2px">
                ${boxes}
                <div class="label"><div>${this.value}</div></div>
            </div>
            ${progress}
        `
    }
}
