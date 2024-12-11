import {LitElement, html, css, TemplateResult, svg} from 'lit'
import {customElement, property} from 'lit/decorators.js'
import "./AriesLabel"
import { ScaledValue, scaleValue, Unit, UnitInterval } from "../Unit"
import { cache } from '../Cache'

const METER_SIZE = 64
const METER_PATH_LENGTH = 360
const METER_STROKE_WIDTH = 8
const METER_SCALE = 0.75
const METER_WIDTH = METER_SIZE + METER_STROKE_WIDTH * 2
const METER_HEIGHT = METER_WIDTH - 12

interface Band {
    start: number
    end: number
    label: string
}

@customElement("a-gauge")
export class AriesGauge extends LitElement {
    @property({attribute: true, type: String})
    icon: string | undefined = undefined

    @property({attribute: true, type: String})
    label: string = "SYS"

    @property({attribute: true, type: Number})
    min: number = 0

    @property({attribute: true, type: Number})
    max: number = 100

    @property({attribute: true, type: String})
    bands: string | undefined = undefined

    @property({attribute: true, type: Number})
    value: number = 50

    @property({attribute: true})
    unit: Unit | undefined

    @property({attribute: true})
    interval: UnitInterval | undefined

    @property({attribute: true, type: Number})
    lockscale: number | undefined = undefined

    private buildBands(bands: Array<Band>): TemplateResult {
        const strokeWidth = METER_STROKE_WIDTH
        const radius = METER_SIZE / 2
        const offset = radius + strokeWidth / 2
        const bandGap = 4

        const circles = new Array<TemplateResult>()

        const startPos = METER_PATH_LENGTH * 0.625
        bands.forEach(band => {
            const gap = band.start > 0 ? bandGap : 0
            const start = band.start * METER_PATH_LENGTH * METER_SCALE + gap
            const length = (band.end - band.start) * METER_PATH_LENGTH * METER_SCALE - gap
            const dashOffset = startPos - start
            const dashArray = `${length} ${METER_PATH_LENGTH - length}`

            circles.push(svg`<circle cx=${offset} cy=${offset} r=${radius} class=${band.label} pathLength=${METER_PATH_LENGTH} stroke-dasharray=${dashArray} stroke-dashoffset=${dashOffset} />`)
        })

        return svg`${circles}`
    }

    private redrawValue(bands: Array<Band>): TemplateResult {
        const strokeWidth = METER_STROKE_WIDTH
        const radius = METER_SIZE / 2
        const offset = radius + strokeWidth / 2
        const startPos = METER_PATH_LENGTH * 0.625

        const clampedValue = Math.max(Math.min(this.value, this.max), this.min)
        const value = (clampedValue - this.min) / (this.max - this.min)
        let valueClass = bands[0].label

        for (const band of bands) {
            if (value < band.start) break
            valueClass = band.label
        }

        const valueDashOffset = startPos
        const valueLength = value * METER_PATH_LENGTH * METER_SCALE
        const valueDashArray = `${valueLength} ${METER_PATH_LENGTH - valueLength}`
    
        const v: ScaledValue = scaleValue(this.value, this.unit, this.interval, this.lockscale)
        const unitOffset = offset + 14

        return svg`
            <circle cx=${offset} cy=${offset} r=${radius} class="val" pathLength=${METER_PATH_LENGTH} stroke-dasharray=${valueDashArray} stroke-dashoffset=${valueDashOffset}></circle>
            <text x=${offset} y=${offset} text-anchor="middle" dominant-baseline="middle" class=${valueClass}>${v.scaledValue}</text>
            <text x=${offset} y=${unitOffset} text-anchor="middle" dominant-baseline="middle" class="unit">${v.unitAbbreviation}</text>
        `
    }

    static override get styles() {
        return css`
            :host {
                flex-grow: 1;
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                gap: 4px;
            }
            svg { 
                filter: drop-shadow(0 0 var(--meter-glow-distance) var(--meter-glow));
            }
            circle {
                fill: transparent;
                opacity: 0.7;
                stroke-width: 8;
                stroke-linecap: butt;
            }
            circle.error, circle.danger {
                stroke: var(--meter-danger);
            }
            circle.warn {
                stroke: var(--meter-warn);
            }
            circle.safe {
                stroke: var(--meter-safe);
            }
            circle.val {
                stroke: var(--meter-value);
                stroke-width: 5;
                opacity: 1;
            }
            text {
                fill: var(--text-color);
            }
            text.warn {
                fill: var(--text-warn);
            }
            text.unit {
                font-size: 75%;
            }
        `
    }

    private parseBands(): Array<Band> {
        const result: Array<Band> = []
        const min = this.min
        const max = this.max
    
        // Split the bands string into parts and initialize variables
        const parts = (this.bands ?? "safe").trim().split(/\s+/)
        let previousValue = min
    
        // Parse the bands
        for (let i = 0; i < parts.length; i += 2) {
            const label = parts[i]
            let value: number
    
            if (i + 1 < parts.length) {
                // Convert the value to the corresponding position in the range
                value = parseFloat(parts[i + 1])
                const normalizedValue = (value - min) / (max - min)
    
                result.push({
                    label,
                    start: (previousValue - min) / (max - min),
                    end: normalizedValue
                })
    
                previousValue = value
            } else {
                // Handle the last label which spans the rest of the range
                result.push({
                    label,
                    start: (previousValue - min) / (max - min),
                    end: 1
                })
            }
        }
    
        return result
    }

    override render() {
        const bands = cache("bands", [this.bands, this.min, this.max], () => this.parseBands()) as Array<Band>
        const bandCircles = cache("bandCircles", [this.bands, this.min, this.max], () => this.buildBands(bands))
        const valueCircle = this.redrawValue(bands)

        return html`
            <svg width=${METER_WIDTH} height=${METER_HEIGHT}>
                ${bandCircles}
                ${valueCircle}
            </svg>
            <a-label center style="info" size="sm">${this.label}</a-label>
        `
    }
}

