# Black Ocean

Black Ocean is an immersive spaceship simulation game where players connect to console screens to collaboratively control and manage the ship's systems. The game features deep mechanics that simulate the spaceship's internals and even the economy of an entire solar system. It offers a rich, multi-layered experience for players who love strategic and cooperative gameplay.

---

## Features
- **Comprehensive Spaceship Simulation**: Control every aspect of the ship, from engineering to navigation.
- **Collaborative Gameplay**: Players take on distinct roles such as Captain, Helmsman (Navigation), Tactical, Science, Engineering, and Communications to manage specific ship systems.
- **Role-Playing with a Full Bridge Crew**: Immerse yourself in the experience of working as a cohesive team to tackle challenges and explore the universe.
- **Deep Mechanics**: Simulates both the internals of the spaceship and the economy of an entire solar system.
- **Web Frontend Accessibility**: The frontend makes the game accessible from many types of devices, especially suited to touchscreens, enhancing ease of interaction and gameplay.- **Comprehensive Spaceship Simulation**: Control every aspect of the ship, from engineering to navigation.
- **Collaborative Gameplay**: Players connect to individual console screens to manage specific ship systems.
- **Deep Mechanics**: Simulates both the internals of the spaceship and the economy of an entire solar system.
- **Modern Web Technologies**: Built with cutting-edge frameworks for performance and scalability.

---

## Technologies Used
- **Backend**:
  - [ASP.NET](https://dotnet.microsoft.com/apps/aspnet)
  - [.NET 8.0](https://dotnet.microsoft.com/)
  - [C# 12](https://learn.microsoft.com/en-us/dotnet/csharp/)
  - [Jitter2](https://github.com/mattleibow/jitter)
- **Frontend**:
  - [TypeScript 5.7](https://www.typescriptlang.org/)
  - [Lit 3](https://lit.dev/)
  - [FontAwesome Pro](https://fontawesome.com/)
- **Testing**:
  - [NUnit](https://nunit.org/) (Backend testing framework)
- **Communication**:
  - [WebSockets](https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API) (Real-time data exchange)- **Backend**:
  - ASP.NET

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
   git clone https://github.com/your-repo/BlackOcean.git
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
4. The backend will start on [http://localhost:5000](http://localhost:5000) by default.

### Frontend Setup
1. Navigate to the frontend directory:
   ```bash
   cd BlackOcean.Frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the frontend:
   ```bash
   npm start
   ```
4. The frontend will start on [http://localhost:3000](http://localhost:3000) by default.

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
├── BlackOcean.App/              # Backend code (ASP.NET)
├── BlackOcean.Common/           # Utilities used by backend components
├── BlackOcean.Simulation/       # Simulation
│   ├── ControlPanel/            # Interface with frontend components
│   ├── Definitions/             # Materials, Component Prefabs, Solar System
│   ├── Scenarios/               # Game scenarios
│   └── ShipSystems/             # Subsystems for a ship
├── BlackOcean.Frontend/         # Frontend code (TypeScript, Lit 3)
│   ├── src/                     # Source code
│   └── package.json             # Node.js configuration
├── LICENSE.md                   # License
└── README.md                    # Documentation
```

---

## Contributing
We welcome contributions! To contribute:
1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Submit a pull request.

You must agree to the [Black Ocean License](LICENSE.md) when forking this repository. All accepted contributions are owned by the Licensor.

---

## License
This project is licensed under the [Black Ocean License](LICENSE.md). Please ensure compliance with the terms outlined in the license.

---

## Contact
For questions, suggestions, or feedback, contact the developer at [blackocean@maragnus.com](mailto:blackocean@maragnus.com).
