# Simulation

## Units

Black Ocean uses metric units.

Heat energy
: Measured in megajoules (MJ)
: Simplified model to calculate the amount of excess heat stored in components and coolant.
: Reference: A cup of coffee at 82°C/180°F contains approximately 57,000 joules (0.057 MJ) of heat energy.

Electrical Energy
: Stored energy is measured in megajoules (MJ)
: Generation is measured in megawatts (MW). `1 megawatt/second = 1 megajoule`
: Reference: A 100-watt lightbulb consumes per 1 megajoule in 10,000 seconds (2 hours 46 minutes)

Materials
: Other materials (gas, liquid, solid) are stored in liters. See the [Materials](#Materials) section for more information.

## Materials

Black Ocean defines a set of resources use in the simulation. The materials are divided into these classifications: Energy, Solid, Gas, and Liquid

All stored materials are measured in liter and mass. Mass is calculated at grams per liter, with gasses stored at 200 bar.

[Material Table](Docs/Materials.md)

## Physics and Mechanics

Powered System
: Powered components convert between materials. They support adjusting their power level which changes their material output, material consumption, and heat output.
: As power level increases, efficiency decreases. This requires more input material to provide the designed output material. This also releases the extra material as heat.

| Metric      | Shutdown | Hibernate | Suspend | Standard | Boost | Overdrive |
|-------------|----------|-----------|---------|----------|-------|-----------|
| Output      | 0%       | 15%       | 51%     | 94%      | 132%  | 150%      |
| Efficiency  | N/A      | 99%       | 98%     | 96%      | 93%   | 90%       |
| Consumption | N/A      | 15.15%    | 52%     | 98%      | 142%  | 167%      |
| Waste Heat  | N/A      | 1%        | 2%      | 4%       | 7%    | 10%       |

Cooling
: Heat is stored in amonia coolant and measured in megajoules
: Heat is released using radiators with 1kW/m². A typical 72m² (3m x 24m) radiator disperses 72 kW of heat, or 72 kJ each second.
: Heat transfer can be accelerated using a heat pump to boost the temperature of the coolant, this consumes energy to increase the temperature differential and can damage the radiators.

Artificial Gravity
: Inertial dampening and artificial gravity are provided using the same fictional mechanism that converts electrical energy into mechanical force.
: 1 g = 9.8 m/s² requires 9.8 N/kg (newtons per kilogram)
: 1 W = 1 N/m/s (newtons/meter/second)
: 90 kg person requires 882 W to simulate Earth's gravity
: **Lateral force**: 0 g ↔ **Safe** ↔ 0.5 g ↔ **Warn** ↔ 2 g ↔ **Danger** ↔ 5 g
: **Vertical force**: -0.5 g ↔ **Danger** ↔ 0.2 g ↔ **Warn** ↔ 0.8 g ↔ **Safe** ↔ 1.5 g ↔ **Warn** ↔ 3 g ↔ **Danger** ↔ 5 g

Faster-than-light Travel (FTL Drive)
: The simulation is designed to make trans-sector travel significant to discourage frequent sector hopping, but also fast enough to keep it a fun experience.
: The solution to this allowing travel at faster-than-light speeds but requiring significant time prepare. This allows the crew a several minute break to maintain the ship, converse, and stretch before the next sector.
: Propulsion in Black Ocean uses an anti-matter drive to achieve faster-than-light speeds throughout the solar system by plotting coursed through a network of sectors.
: **For example**: Uranus is 160 minutes (2 hours 40 minutes) away from the sun at light speed.

Sub-light Travel (Impulse Drive)
: Intra-sector travel uses Helium-3 propulsion with no upper limit on top speed.

Away Missions
: NPC Crew can be dispatched to other locations while being monitored and directed by the player crew.
: Crew maintain different stats that affect the outcome of the mission and must be chosen carefully.
: Crew are transported via shuttle.