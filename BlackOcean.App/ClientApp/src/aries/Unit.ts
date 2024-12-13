export type Unit = 'none' | 'percent' | 'meter' | 'liter' | 'gram' | 'kelvin' | 'second' | 'minute' | 'hour' | 'day' | 'year' 
    | 'volt' | 'amp' | 'ohm' | 'watt' | 'joule' | 'coulomb' | 'hertz' | 'henry' | 'farad' | 'byte' | 'bit' 
    | 'beats per minute' | 'millimeter of mercury' | 'celsius' | 'gram per deciliter' | 'ph' | 'newton' 
    | 'gravity' | 'pascal' | 'mole' | 'candela' | 'lumen' | 'lux' | 'decibel' | 'gray' | 'sievert';

export const unitAbbreviations: Record<string, string> = {
    none: "",

    // General Units
    percent: "%",
    meter: "m",
    liter: "L",
    gram: "g",
    kelvin: "K",

    // Time units
    second: "s",
    minute: "m",
    hour: "h",
    day: "d",
    year: "y",

    // Electrical Units
    volt: "V",
    amp: "A",
    ohm: "Ω",
    watt: "W",
    joule: "J",
    coulomb: "C",
    hertz: "Hz",
    henry: "H",
    farad: "F",

    // Computer Units
    byte: "B",
    bit: "b",

    // Human Health Units
    "beats per minute": "BPM",
    "millimeter of mercury": "mmHg",
    celsius: "°C",
    "gram per deciliter": "g/dL",
    ph: "ph",

    // Scientific Units
    newton: "N",
    gravity: "g",
    pascal: "Pa",
    mole: "mol",
    candela: "cd",
    lumen: "lm",
    lux: "lx",
    decibel: "dB",
    gray: "Gy",
    sievert: "Sv",
};

export interface ScaledValue {
    value: number
    scaledValue: number
    displayValue: number
    unit: Unit
    interval: Unit | undefined
    unitAbbreviation: string
    unitName: string
}

const scalable: Set<Unit> = new Set<Unit>([ 
    'meter', 'liter', 'gram', 'kelvin', 'volt', 'amp', 'ohm', 'watt', 'joule', 'coulomb', 'hertz', 'henry', 'farad', 
    'gram per deciliter', 'pascal', 'mole', 'candela', 'lumen', 'lux', 'gray', 'sievert'
]);

function trimDecimals(v: number) {
    if (v >= 100) return Math.floor(v)
    if (v >= 10) return Math.floor(v * 10) / 10
    return Math.floor(v * 100) / 100
}

export function scaleValue(value: number, unit: Unit | undefined, interval: Unit | undefined = undefined, lockScale: number | undefined = undefined): ScaledValue {
    unit ??= "none"

    if (!scalable.has(unit))
        lockScale = 0

    // Define metric prefixes and their factors
    const prefixes = [
        { factor: 1e-10, abbreviation: "p", name: "pico" },
        { factor: 1e-9, abbreviation: "n", name: "nano" },
        { factor: 1e-6, abbreviation: "µ", name: "micro" },
        { factor: 1e-3, abbreviation: "m", name: "milli" },
        { factor: 1, abbreviation: "", name: "" },
        { factor: 1e3, abbreviation: "k", name: "kilo" },
        { factor: 1e6, abbreviation: "M", name: "mega" },
        { factor: 1e9, abbreviation: "G", name: "giga" },
        { factor: 1e10, abbreviation: "T", name: "tera" },
    ];

    let bestPrefix = prefixes[0];

    if (lockScale !== undefined) {
        bestPrefix = prefixes[4 + lockScale]
    } else {
        // Select the best prefix based on the absolute value
        for (const prefix of prefixes) {
            if (Math.abs(value) < prefix.factor) break;
            bestPrefix = prefix;
        }
    }

    // Scale the value
    const scaledValue = value / bestPrefix.factor;
    const displayValue = trimDecimals(scaledValue)

    const unitInterval = interval ? `/${interval}` : '';
    const unitName = `${bestPrefix.name}${unit}${unitInterval}`;

    const unitIntervalAbbreviation = interval ? `/${unitAbbreviations[interval]}` : '';
    const unitAbbreviation = `${bestPrefix.abbreviation}${unitAbbreviations[unit]}${unitIntervalAbbreviation}`;

    return {
        value,
        scaledValue,
        displayValue,
        unit, 
        interval,
        unitAbbreviation,
        unitName,
    };
}