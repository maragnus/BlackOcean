import { css } from "lit"

export const FlexLayoutStyles = css`
    :host(.f-row) {
        display: flex;
        flex-direction: row;
    }
    :host(.f-column) {
        display: flex;
        flex-direction: column;
    }
    :host(.f-none-d) {
        display: flex;
        flex-direction: column;
        justify-content: flex-start;
        align-items: flex-start;
    }
    :host(.f-fill) { flex: 1; }

    :host(.fjc-start) { justify-content: flex-start; }
    :host(.fjc-center) { justify-content: center; }
    :host(.fjc-end) { justify-content: flex-end; }
    :host(.fjc-space-between) { justify-content: space-between; }
    :host(.fjc-space-around) { justify-content: space-around; }
    :host(.fjc-space-evenly) { justify-content: space-evenly; }
    :host(.fjc-stretch) { justify-content: stretch; }

    :host(.fac-start) { align-content: flex-start; }
    :host(.fac-center) { align-content: center; }
    :host(.fac-end) { align-content: flex-end; }
    :host(.fac-space-between) { align-content: space-between; }
    :host(.fac-space-around) { align-content: space-around; }
    :host(.fac-space-evenly) { align-content: space-evenly; }
    :host(.fac-stretch) { align-content: stretch; }
    `

export const TextStyles = css`
    :host(.text-normal) {
        color: var(--text-color);
    }
    :host(.text-dim) {
        color: var(--text-dim);
    }
    :host(.text-title) {
        font-weight: bold;
    }
    :host(.text-info) {
        color: var(--text-info);
    }
    :host(.text-warn) {
        color: var(--text-warn);
    }
    :host(.text-error) {
        color: var(--text-error);
        text-shadow: 0 0 8px var(--text-error-glow);
    }
    :host(.text-xs) {
        font-size: 50%;
    }
    :host(.text-sm) {
        font-size: 75%;
    }
    :host(.text-md) {
        font-size: 100%;
    }
    :host(.text-lg) {
        font-size: 175%;
    }
    :host(.text-xl) {
        font-size: 240%;
    }
    :host(.text-center) {
        text-align: center;
    }
    sup {
        font-size: 66%;
        vertical-align: 30%;
    }
    `

export const SliderStyles = css`
    :host {
        flex-grow: 1;
        display: flex;
        flex-direction: row;
        justify-content: center;
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

    input[type="range"]:disabled::-webkit-slider-thumb {
        border: 1px solid #888;
        box-shadow: none !important;
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
    `

export const ButtonStyles = css`
    :host(.fill) {
        flex: 1;
    }
    :host {
        display: contents;
    }
    button {
        font-family: var(--font-family);
        display: flex;
        align-items: center;
        border: 1px solid var(--border-light);
        border-radius: var(--button-right-radius) var(--button-left-radius) var(--button-left-radius) var(--button-right-radius);
        background: var(--button-background);
        padding: var(--button-padding);
        color: var(--text-color);
        text-shadow: 0 0 var(--text-glow-distance) var(--text-glow);
        background-image: radial-gradient(ellipse at center center, rgba(255, 255, 255, 0.15) 0%, rgba(255, 255, 255, 0.10) 24%, rgba(255, 255, 255, 0) 100%);
        box-shadow: 
            0 0 var(--button-bloom-size) 0 var(--button-bloom-color),
            inset 0 0 var(--button-bloom-size) 0 var(--button-bloom-color);
        transition: box-shadow 0.1s ease-in, border 0.1s ease-in;
        text-transform: uppercase;
        white-space: nowrap;
        min-height: 42px;
    }
    button:disabled {
        border: 1px solid #888;
        color: #888;
        background-image: none;
    }
    button:not(:disabled):hover {
        border: 1px solid #fff;
        box-shadow: 
            0 0 var(--button-bloom-size) 1px var(--button-hover-bloom-color),
            inset 0 0 var(--button-bloom-size) 1px var(--button-hover-bloom-color);
    }
    button:not(:disabled):active {
        border: 1px solid var(--button-active-border-color);
        background-color: var(--button-active-background-color);
        box-shadow: 0 0 var(--button-bloom-size) 1px var(--button-active-bloom-color);
    }
    :host(.toggle) button {
        border-radius: var(--button-right-radius);
    }
    :host(.active) button:not(:active) {
        background-color: var(--button-active-color);
    }
    .start-icon { 
        flex: 0;
        position: relative;
        left: -6px;
        display: block;
        padding: 0;
        margin: 0;
    }
    .end-icon { 
        flex: 0;
        position: relative;
        right: -6px;
        display: block;
        padding: 0;
        margin: 0;
    }
    .label {
        display: block;
        flex: 1;
        text-align: center;
    }
    `