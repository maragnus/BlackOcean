import * as fs from 'fs';
import {library, parse as faParse, dom as faDom, icon as faIcon} from '@fortawesome/fontawesome-svg-core'
import {fal} from '@fortawesome/pro-light-svg-icons'
import {fas} from '@fortawesome/pro-solid-svg-icons'
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const destinationFile = path.join(__dirname, "src", "aries", "Icons.ts")

const iconMap: Record<string, string> = {
    none: "fas fa-link-simple-slash",
    unknown: "fas fa-circle-question",
    on: "fas fa-circle",
    off: "fal fa-circle",
    separator: "fal fa-ellipsis-stroke-vertical",
    destroyed: "fal fa-explosion",
    alert: "fal fa-bell",
    chart: "fal fa-chart-fft",

    directionUp: "fal fa-angle-up",
    directionUpUp: "fal fa-angles-up",
    directionDown: "fal fa-angle-down",
    directionDownDown: "fal fa-angles-down",
    directionLeft: "fal fa-angle-left",
    directionLeftLeft: "fal fa-angles-left",
    directionRight: "fal fa-angle-right",
    directionRightRight: "fal fa-angles-right",

    ariesLogo: "fal fa-solar-system",
    ariesLink: "fal fa-circle-nodes",
    
    // Navigation
    navigationStation: "fal fa-dharmachakra",
    systemMap: "fal fa-solar-system",

    // Tactical
    tacticalStation: "fal fa-crosshairs",

    // Database
    databaseStation: "fal fa-book-open-cover",
    search: "fal fa-magnifying-glass",
    recordShip: "fal fa-starship",
    recordFaction: "fal fa-people-group",
    recordPerson: "fal fa-person-simple",
    recordComponent: "fal fa-diamonds-4",
    recordArticle: "fal fa-newspaper",

    // Engineering
    engineeringStation: "fal fa-sliders",
    automated: "fal fa-microchip-ai",
    reactor: "fal fa-starfighter-twin-ion-engine-advanced",
    emergencyReactor: "fal fa-light-emergency",
    lifeSupport: "fal fa-heart-pulse",
    energyShield: "fal fa-shield-plus",
    ablativeShield: "fal fa-shield-quartered",
    energyWeapon: "fal fa-raygun",
    impulseDrive: "fal fa-rocket-launch",
    warpDrive: "fal fa-coin-vertical",
    scanner: "fal fa-radar",
    communications: "fal fa-satellite-dish",
    emergencyBattery: "fal fa-car-battery",
    shieldField: "fal fa-shield",
    radiator: "fal fa-vent-damper",
    purge: "fal fa-eject",
    energy: "fal fa-bolt",
    fusion: "fal fa-atom-simple",
    batteryCharge: "fal fa-battery-bolt",
    batteryDrain: "fal fa-battery-low",
    batteryEmpty: "fal fa-battery-slash",
    fuel: "fal fa-vial",
    fuelEfficiency: "fal fa-aperture",
    heating: "fal fa-fire",
    cooling: "fal fa-snowflake",
    coolant: "fal fa-meter-fire",
    radiationInterior: "fal fa-circle-radiation",
    radiationExterior: "fal fa-radiation",

    // Communications
    communicationsStation: "fal fa-satellite-dish",
    commEmpty: "fal fa-message",
    commMessage: "fal fa-message-lines",
    commClose: "fal fa-message-xmark",
    commFriendly: "fal fa-message-smile",
    commInquiry: "fal fa-message-question",
    commAlert: "fal fa-message-exclamation",
    commAgreed: "fal fa-message-check",
    commAutomated: "fal fa-message-bot",
    commAssist: "fal fa-message-medical",
    commResponding: "fal fa-message-pen",
    commImpersonate: "fal fa-user-secret",
    commBroadcast: "fal fa-signal-stream",
    commIntercept: "fal fa-diagram-venn",
    commBlock: "fal fa-signal-stream-slash",

    // Science
    scienceStation: "fal fa-atom-simple",
    scienceExamine: "fal fa-microscope",
    scienceScan: "fal fa-signal-stream",
    
    // Research
    researchStation: "fal fa-flask-vial",
    researchExamine: "fal fa-microscope",
    researchDisease: "fal fa-disease",
    researchVirus: "fal fa-virus",
    researchCure: "fal fa-syringe",

    // Biology
    biologyStation: "fal fa-dna",
    healthDna: "fal fa-dna",
    healthDisease: "fal fa-disease",
    healthVirus: "fal fa-virus",
    healthBiohazard: "fal fa-biohazard",
    healthPulse: "fal fa-heart-pulse",
    healthDead: "fal fa-skull",
    healthAirborn: "fal fa-head-side-cough",
    healthFomite: "fal fa-hand-dots",
    healthBandage: "fal fa-bandage",
    healthSyringe: "fal fa-syringe",
    healthCheckup: "fal fa-stethoscope",

    // Operations
    operationsStation: "fal fa-users-gear",

    // Crew assignment
    crewUnassigned: "fal fa-people-roof",
    crewAssigned: "fal fa-people-line",
    crewAssign: "fal fa-person-walking-arrow-right",
    crewUnassign: "fal fa-person-walking-arrow-loop-left",
    crewDispatch: "fal fa-person-walking-dashed-line-arrow-right",
    crewReturn: "fal fa-person-walking-arrow-loop-left",
    crewHaste: "fal fa-person-running-fast"
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

const iconSource = Object.entries(iconTable).map(x => `  ${x[0]}: html\`${x[1]}\``).join(",\n")
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