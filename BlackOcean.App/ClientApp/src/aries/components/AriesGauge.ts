import {LitElement, html, css, TemplateResult, svg} from 'lit'
import {customElement, property} from 'lit/decorators.js'
import "./AriesLabel"
import { ScaledValue, scaleValue, Unit } from "../Unit"
import { cache } from '../Cache'
import { ifDefined } from 'lit/directives/if-defined.js'

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

export type GaugeScale = "linear" | "exp" | "log"

@customElement("a-gauge")
export class AriesGauge extends LitElement {
    @property({attribute: true, type: String})
    icon?: string = undefined

    @property({attribute: true, type: Number})
    min: number = 0

    @property({attribute: true, type: Number})
    max: number = 100

    /**
     * Bands definition: label/value pairs. For example:
     * "safe 50 caution 80 danger"
     * This means:
     * - "safe" from min to 50
     * - "warn" from 50 to 80
     * - "danger" from 80 to max
     */
    @property({attribute: true, type: String})
    bands?: string = undefined

    @property({attribute: true, type: Number})
    value: number = 50

    @property({attribute: true})
    unit?: Unit

    @property({attribute: true})
    interval?: Unit

    @property({attribute: true, type: Number})
    lockscale?: number = undefined

    /**
     * Supported scales:
     * - linear: (value-min)/(max-min)
     * - exp: 
     * - log: (log(value)-log(min)) / (log(max)-log(min)) [natural log]
     */
    @property({attribute: true})
    scale: GaugeScale = "linear"

    @property({attribute: true})
    indicator: "needle" | "bar" = "needle"
    
    private normalizeValue(value: number): number {
        let scale = this.scale
        const min = this.min
        const max = this.max

        const k = 1; // TODO: make adjustable?

        // Avoid issues with non-positive values in log scales
        // For log-based scales, values must be > 0. If not, fall back to exp.
        if (min <= 0 && scale == "log") scale = "exp" // fallback
        const v = Math.min(Math.max(value, min), max)

        if (max === min) return 0 // avoid division by zero if degenerate range

        switch (scale) {
            case "log":
                return (Math.log(v) - Math.log(min)) / (Math.log(max) - Math.log(min))
            case "exp":
                return (Math.exp(k * (v - min)) - 1) / (Math.exp(k * (max - min)) - 1);
            default:
                return (v - min) / (max - min)
        }
    }

    private buildBands(bands: Array<Band>): TemplateResult {
        const strokeWidth = METER_STROKE_WIDTH
        const radius = METER_SIZE / 2
        const offset = radius + strokeWidth / 2
        const bandGap = 4

        const startPos = METER_PATH_LENGTH * 0.625
        const circles: TemplateResult[] = []

        for (const band of bands) {
            // Normalize the start and end using the chosen scale
            const startVal = this.normalizeValue(this.min + (band.start * (this.max - this.min)))
            const endVal = this.normalizeValue(this.min + (band.end * (this.max - this.min)))

            const gap = startVal > 0 ? bandGap : 0
            const start = startVal * METER_PATH_LENGTH * METER_SCALE + gap
            const length = (endVal - startVal) * METER_PATH_LENGTH * METER_SCALE - gap
            const dashOffset = startPos - start
            const dashArray = `${length} ${METER_PATH_LENGTH - length}`

            if (length < bandGap + 1) continue;
            
            circles.push(svg`<circle cx=${offset} cy=${offset} r=${radius} class=${band.label} pathLength=${METER_PATH_LENGTH} stroke-dasharray=${dashArray} stroke-dashoffset=${dashOffset} />`)
        }

        return svg`${circles}`
    }

    private redrawValue(bands: Array<Band>): TemplateResult {
        const strokeWidth = METER_STROKE_WIDTH
        const radius = METER_SIZE / 2
        const offset = radius + strokeWidth / 2
        const startPos = METER_PATH_LENGTH * 0.625

        // Normalize the value
        const normValue = this.normalizeValue(this.value)
        
        // Determine the band for the current value
        let valueClass = bands[0]?.label ?? "default"
        for (const band of bands) {
            const startVal = this.normalizeValue(this.min + (band.start * (this.max - this.min)))
            if (normValue < startVal) break
            valueClass = band.label
        }

        let indicatorClass: string
        let valueDashOffset: number
        let valueLength: number 
        let valueDashArray: string
        
        switch (this.indicator) {
            case "bar":
                indicatorClass = "indicator bar"
                valueDashOffset = startPos
                valueLength = normValue * METER_PATH_LENGTH * METER_SCALE
                valueDashArray = `${valueLength} ${METER_PATH_LENGTH - valueLength}`
                break;
            default:
                indicatorClass = "indicator needle"
                valueDashOffset = startPos
                valueLength = Math.max(normValue * METER_PATH_LENGTH * METER_SCALE - 4, 0)
                valueDashArray = `0 ${valueLength} 8 ${METER_PATH_LENGTH - valueLength - 8}`
                break;
        }
        
        const v: ScaledValue = scaleValue(this.value, this.unit, this.interval, this.lockscale)
        const unitOffset = offset + 14
        const displayValue = v.displayValue

        return svg`
            <circle cx=${offset} cy=${offset} r=${radius} class=${indicatorClass} pathLength=${METER_PATH_LENGTH} stroke-dasharray=${valueDashArray} stroke-dashoffset=${valueDashOffset}></circle>
            <text x=${offset} y=${offset} text-anchor="middle" dominant-baseline="middle" class=${valueClass}>${displayValue}</text>
            <text x=${offset} y=${unitOffset} text-anchor="middle" dominant-baseline="middle" class="unit">${v.unitAbbreviation}</text>
        `
    }

    private parseBands(): Array<Band> {
        const result: Array<Band> = []
        const min = this.min
        const max = this.max
        const bandsString = this.bands?.trim() ?? "none"
        const parts = bandsString.split(/\s+/)
        let previousValue = min

        for (let i = 0; i < parts.length; i += 2) {
            const label = parts[i]
            if (i + 1 < parts.length) {
                const value = parseFloat(parts[i + 1])
                const startNorm = (previousValue - min) / (max - min)
                const endNorm = (value - min) / (max - min)
                result.push({ label, start: startNorm, end: endNorm })
                previousValue = value
            } else {
                // last band goes to the end
                const startNorm = (previousValue - min) / (max - min)
                result.push({ label, start: startNorm, end: 1 })
            }
        }

        return result
    }

    override render() {
        const bands = cache("bands", [this.bands, this.min, this.max, this.scale], () => this.parseBands()) as Array<Band>
        const bandCircles = cache("bandCircles", [this.bands, this.min, this.max, this.scale], () => this.buildBands(bands))
        const valueCircle = this.redrawValue(bands)

        return html`
            <svg width=${METER_WIDTH} height=${METER_HEIGHT}>
                ${bandCircles}
                ${valueCircle}
            </svg>
            <a-label center style="info" size="sm" icon=${ifDefined(this.icon)}><slot></slot></a-label>
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
            circle.none {
                stroke: var(--meter-none);
            }
            circle.indicator.bar {
                stroke: var(--meter-value);
                stroke-width: 5;
                opacity: 1;
            }
            circle.indicator.needle {
                stroke: var(--meter-value);
                stroke-width: 10;
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
}
