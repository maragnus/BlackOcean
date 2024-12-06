import { css } from "lit";

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
    :host(.f-fill) { flex-grow: 1; }

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