import * as fs from 'fs';
import {library, parse as faParse, dom as faDom, icon as faIcon} from '@fortawesome/fontawesome-svg-core'
import {fal} from '@fortawesome/pro-light-svg-icons'
import {fas} from '@fortawesome/pro-solid-svg-icons'
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const destinationFile = path.join(__dirname, "src", "aries", "Icons.ts")

const iconMap: Record<string, string> = {
    ["none"]: "fas fa-link-simple-slash",
    ["unknown"]: "fas fa-circle-question",
    ["on"]: "fas fa-circle",
    ["off"]: "fal fa-circle",
    ["separator"]: "fal fa-ellipsis-stroke-vertical",
    ["destroyed"]: "fal fa-explosion",
    ["alert"]: "fal fa-bell",
    ["chart"]: "fal fa-chart-fft",

    // Indicators
    ["level-overdrive"]: "fal fa-arrows-down-to-line",
    ["level-boost"]: "fal fa-arrow-down-to-line",
    ["level-standard"]: "fal fa-arrows-from-line",
    ["level-standby"]: "fal fa-arrow-up-to-line",
    ["level-hibernate"]: "fal fa-arrows-up-to-line",
    
    ["direction-up"]: "fal fa-angle-up",
    ["direction-up-up"]: "fal fa-angles-up",
    ["direction-down"]: "fal fa-angle-down",
    ["direction-down-down"]: "fal fa-angles-down",
    ["direction-left"]: "fal fa-angle-left",
    ["direction-left-left"]: "fal fa-angles-left",
    ["direction-right"]: "fal fa-angle-right",
    ["direction-right-right"]: "fal fa-angles-right",

    ["aries-logo"]: "fal fa-solar-system",
    ["aries-link"]: "fal fa-circle-nodes",
    
    // Navigation
    ["navigation-station"]: "fal fa-dharmachakra",
    ["system-map"]: "fal fa-solar-system",

    // Tactical
    ["tactical-Station"]: "fal fa-crosshairs",

    // Database
    ["database-station"]: "fal fa-book-open-cover",
    ["search"]: "fal fa-magnifying-glass",
    ["record-ship"]: "fal fa-starship",
    ["record-faction"]: "fal fa-people-group",
    ["record-person"]: "fal fa-person-simple",
    ["record-component"]: "fal fa-diamonds-4",
    ["record-article"]: "fal fa-newspaper",

    // Engineering
    ["engineering-station"]: "fal fa-sliders",
    ["component-automated"]: "fal fa-microchip-ai",
    ["component-reactor"]: "fal fa-starfighter-twin-ion-engine-advanced",
    ["component-emergency-reactor"]: "fal fa-light-emergency",
    ["component-life-support"]: "fal fa-heart-pulse",
    ["component-energy-shield"]: "fal fa-shield-plus",
    ["component-ablative-shield"]: "fal fa-shield-quartered",
    ["component-energy-weapon"]: "fal fa-raygun",
    ["component-impulse-drive"]: "fal fa-rocket-launch",
    ["component-warp-drive"]: "fal fa-coin-vertical",
    ["component-scanner"]: "fal fa-radar",
    ["component-communications"]: "fal fa-satellite-dish",
    ["component-emergency-battery"]: "fal fa-car-battery",
    ["component-shield-field"]: "fal fa-shield",
    ["component-radiator"]: "fal fa-vent-damper",
    ["component-purge"]: "fal fa-eject",
    ["component-energy"]: "fal fa-bolt",
    ["component-fusion"]: "fal fa-atom-simple",
    ["component-battery-charge"]: "fal fa-battery-bolt",
    ["component-battery-drain"]: "fal fa-battery-low",
    ["component-battery-empty"]: "fal fa-battery-slash",
    ["component-fuel"]: "fal fa-vial",
    ["component-fuel-efficiency"]: "fal fa-aperture",
    ["component-heating"]: "fal fa-fire",
    ["component-cooling"]: "fal fa-snowflake",
    ["component-coolant"]: "fal fa-meter-fire",

    // Status
    ["status-station"]: "fal fa-circle-radiation",
    ["status-radiation-interior"]: "fal fa-circle-radiation",
    ["status-radiation-exterior"]: "fal fa-radiation",

    // Communications
    ["communications-station"]: "fal fa-satellite-dish",
    ["comm-empty"]: "fal fa-message",
    ["comm-message"]: "fal fa-message-lines",
    ["comm-close"]: "fal fa-message-xmark",
    ["comm-friendly"]: "fal fa-message-smile",
    ["comm-inquiry"]: "fal fa-message-question",
    ["comm-alert"]: "fal fa-message-exclamation",
    ["comm-agreed"]: "fal fa-message-check",
    ["comm-automated"]: "fal fa-message-bot",
    ["comm-assist"]: "fal fa-message-medical",
    ["comm-responding"]: "fal fa-message-pen",
    ["comm-impersonate"]: "fal fa-user-secret",
    ["comm-broadcast"]: "fal fa-signal-stream",
    ["comm-intercept"]: "fal fa-diagram-venn",
    ["comm-block"]: "fal fa-signal-stream-slash",

    // Science
    ["science-station"]: "fal fa-atom-simple",
    ["science-examine"]: "fal fa-microscope",
    ["science-scan"]: "fal fa-signal-stream",
    
    // Research
    ["research-station"]: "fal fa-flask-vial",
    ["research-examine"]: "fal fa-microscope",
    ["research-disease"]: "fal fa-disease",
    ["research-virus"]: "fal fa-virus",
    ["research-cure"]: "fal fa-syringe",

    // Biology
    ["biology-station"]: "fal fa-dna",
    ["health-dna"]: "fal fa-dna",
    ["health-disease"]: "fal fa-disease",
    ["health-virus"]: "fal fa-virus",
    ["health-biohazard"]: "fal fa-biohazard",
    ["health-pulse"]: "fal fa-heart-pulse",
    ["health-dead"]: "fal fa-skull",
    ["health-airborn"]: "fal fa-head-side-cough",
    ["health-fomite"]: "fal fa-hand-dots",
    ["health-bandage"]: "fal fa-bandage",
    ["health-syringe"]: "fal fa-syringe",
    ["health-checkup"]: "fal fa-stethoscope",

    // Operations
    ["operations-station"]: "fal fa-users-gear",

    // Crew assignment
    ["crew-unassigned"]: "fal fa-people-roof",
    ["crew-assigned"]: "fal fa-people-line",
    ["crew-assign"]: "fal fa-person-walking-arrow-right",
    ["crew-unassign"]: "fal fa-person-walking-arrow-loop-left",
    ["crew-dispatch"]: "fal fa-person-walking-dashed-line-arrow-right",
    ["crew-return"]: "fal fa-person-walking-arrow-loop-left",
    ["crew-haste"]: "fal fa-person-running-fast"
}

const css = faDom.css();

library.add(fal)
library.add(fas)

const unknownIcon = faParse.icon(iconMap.unknown)

const iconTable: Record<string, string> = {}

for (const [iconName, fontAwesomeName] of Object.entries(iconMap)) {
    const iconLookup = faParse.icon(fontAwesomeName) ?? unknownIcon
    const iconHtml = faIcon(iconLookup).html

    iconTable[iconName] = iconHtml.join()
}

const iconSource = Object.entries(iconTable).map(x => `  ['${x[0]}']: html\`${x[1]}\``).join(",\n")
const fileContents = `
import { css, html, TemplateResult } from "lit"

export type SizeProp = "2xs" | "xs" | "sm" | "lg" | "xl" | "2xl" | "1x" | "2x" | "3x" | "4x" | "5x" | "6x" | "7x" | "8x" | "9x" | "10x";

export const IconStyles = css\`
${css}
\`

export const Icons: Record<string, TemplateResult> = {
${iconSource}
}
`

fs.writeFileSync(destinationFile, fileContents)