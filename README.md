# Black Ocean

Black Ocean is an immersive spaceship simulation game where players connect to console screens to collaboratively control and manage the ship's systems. The game features deep mechanics that simulate the spaceship's internals and even the economy of an entire solar system. It offers a rich, multi-layered experience for players who love strategic and cooperative gameplay.

---

## Features
- **Comprehensive Spaceship Simulation**: Control every aspect of the ship, from engineering to navigation.
- **Collaborative Gameplay**: Players take on distinct roles such as Captain, Helmsman, Tactical, Science, Operations, Engineering, and Communications.
- **Role-Playing with a Full Bridge Crew**: Immerse yourself in the experience of working as a cohesive team to tackle challenges and explore the universe.
- **Deep Mechanics**: Simulates the internals of the spaceship, faction dynamics, and the economy of an entire solar system.
- **Web Frontend Accessibility**: The frontend makes the game accessible from many types of devices, especially suited to touchscreens, enhancing ease of interaction and gameplay. 

See also:
* [Roles](Docs/Roles.md) for more information on player roles and mission types.
* [Simulation](Docs/Simulation.md) for more information on simulation physics and game mechanics.
 
---

## Technologies Used
- **Backend**:
  - [ASP.NET](https://dotnet.microsoft.com/apps/aspnet)
  - [.NET 8.0](https://dotnet.microsoft.com/)
  - [C# 12](https://learn.microsoft.com/en-us/dotnet/csharp/)
  - [Jitter2](https://github.com/mattleibow/jitter) (Physics engine)
- **Frontend**:
  - [TypeScript 5.7](https://www.typescriptlang.org/)
  - [Lit 3](https://lit.dev/)
  - [FontAwesome Pro](https://fontawesome.com/)
- **Testing**:
  - [NUnit](https://nunit.org/) (Backend testing framework)
- **Communication**:
  - [WebSockets](https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API) (Real-time data exchange)

---

## Installation and Setup

### Prerequisites
- **Backend Requirements**:
  - .NET 8.0 SDK or later
- **Frontend Requirements**:
  - Node.js 18.x or later
  - npm 9.x or later

### Backend Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/maragnus/BlackOcean.git
   cd BlackOcean
   ```
2. Navigate to the backend directory:
   ```bash
   cd BlackOcean.App
   ```
3. Run the backend:
   ```bash
   dotnet run --project BlackOcean.App\BlackOcean.App.csproj
   ``` 
4. The backend will start on http://localhost:5226 by default.
   When the frontend becomes available, you will be redirected to http://localhost:44414
   The frontend will start if not running, it may take a couple of minutes on first build.

### Frontend Setup
1. Navigate to the frontend directory:
   ```bash
   cd BlackOcean.App/ClientApp
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Build and serve the frontend:
   ```bash
   npm run dev
   ```
4. The frontend will start on http://localhost:44414 by default.

---

## Testing
- **Backend Testing**:
  - The backend uses NUnit for unit testing.
  - Run tests using the following command:
    ```bash
    dotnet test
    ```
- **Frontend Testing**:
  - There is currently no testing on the frontend.

---

## Project Structure
```
BlackOcean/
├── BlackOcean.App/              # Backend server (ASP.NET)
│   ├── ClientApp/               # Frontend client (TypeScript, Lit 3)
│   │   ├── src/                  
│   │   │   ├── aries/           # Aries UI components 
│   │   │   │   ├── components/  # Simple components
│   │   │   │   └── displays/    # Complex connected widgets
│   │   │   ├── controlpanel/    # Client/server interface model
│   │   │   ├── public/          # Statis hosts assets (html, images) 
│   │   │   └── styles/          # SASS stylesheets
│   │   └── package.json         # Node.js configuration
│   └── Program.cs               # Entry point for backend server
├── BlackOcean.Common/           # Utilities used by backend components
├── BlackOcean.Simulation/       # Simulation
│   ├── ControlPanel/            # Client/server interface model
│   ├── Definitions/             # Materials, Component Prefabs, Solar System
│   │   └── ShipSystems.json     # Prefab definitions of ShipSystems
│   ├── Scenarios/               # Game scenarios
│   └── ShipSystems/             # Subsystems for a ship
├── LICENSE.md                   # License
├── Docs                         # Documentation
│   ├── Materials.md             # Table of simulated materials
│   ├── Roles.md                 # Details on player roles and mission types
│   └── Simulation.md            # Details on simulation physics and mechanics
└── README.md                    # Documentation
```

---

## Contributing
We welcome contributions! To contribute:
1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Submit a pull request.

You must agree to the [Black Ocean License](#License) when forking this repository. All accepted contributions are owned by the Licensor.

---

## License
The code is available under good faith for use by mod makers and for educational purposes only. You **MAY NOT** use any portion of this repository for personal or commercial projects. The only exception is open source mods for Black Ocean.

This project is licensed under the [Black Ocean License](LICENSE.md). Please ensure compliance with the terms outlined in the license.

---

## Contact
For questions, suggestions, or feedback, contact the developer at [blackocean@maragnus.com](mailto:blackocean@maragnus.com).
