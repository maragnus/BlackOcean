export interface ControlPanel {
    [index: string]: unknown
    identity: Identity
    reactor: PoweredSystem
    emergencyReactor: PoweredSystem
    lifeSupport: PoweredSystem
    energyShield: PoweredSystem
    ablativeShield: PoweredSystem
    impulseDrive: PoweredSystem
    warpDrive: PoweredSystem
    energyWeapon: PoweredSystem
    scanner: PoweredSystem
    communications: PoweredSystem
    shieldPurge: Button
    emergencyPower: Button
    radiator: Button
    cooler: Button
    purge: Button
    fuel: Gauge
    fuelConsumption: Gauge
    fuelEfficiency: Gauge
    emergencyFuel: Gauge
    ablativeShielding: Gauge
    forwardShield: Gauge
    aftShield: Gauge
    generated: Gauge
    draw: Gauge
    energy: Gauge
    battery: Gauge
    emergencyBattery: Gauge
    heatDelta: Gauge
    heatPurge: Gauge
    heatStore: Gauge
    interiorExposure: Gauge
    exteriorExposure: Gauge
}

export interface Gauge {
    [index: string]: unknown
    name: string
    unit: string
    interval: string | undefined
    min: number
    max: number
    value: number
    bands: Band[]
    scale: Scale
}

export enum Scale {
    Linear = "Linear",
    Exp = "Exp",
    Log = "Log",
}

export interface Band {
    [index: string]: unknown
    value: number
    status: Status
}

export enum Status {
    None = "None",
    Safe = "Safe",
    Warn = "Warn",
    Danger = "Danger",
}

export interface Button {
    [index: string]: unknown
    name: string
    icon: string
    pressed: boolean
    available: boolean
    toggle: boolean
}

export interface PoweredSystem {
    [index: string]: unknown
    name: string
    abbreviation: string
    icon: string
    minLevel: number
    maxLevel: number
    currentLevel: number
    levelStatuses: Status[]
    available: boolean
    operating: boolean
    setLevel: number
    currentHeat: number | undefined
    heatBands: Band[] | undefined
    currentOutput: number
    nominalOutput: number
    powered: Button
    auto: Button
}

export interface Identity {
    [index: string]: unknown
    shipName: string | undefined
    callSign: string | undefined
    osIdentifier: string | undefined
    velocity: number
    heading: number
    mark: number
    verticalG: number
    lateralG: number
    interiorRadiation: number
    exteriorRadiation: number
}
