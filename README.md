# DeepMine

![Unity Version](https://img.shields.io/badge/Unity-2D%20URP-black?logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)
![Status](https://img.shields.io/badge/Status-Released-success)
![License](https://img.shields.io/badge/License-All%20Rights%20Reserved-red)

**A strategic mining game where terrain instability creates emergent gameplay challenges.**

DeepMine is a systems-driven mining game built in 72 hours for Ludum Dare 57. Players navigate an unstable underground world using tool-based mechanics, extracting valuable ores while managing structural integrity through strategic support placement.

![DeepMine Gameplay](https://github.com/user-attachments/assets/22fef938-bec0-42f4-ba52-6f1273a4b49b)

## Quick Start

[Play on itch.io](https://eduardo79silva.itch.io/deepmine) | [View Ludum Dare Submission](https://ldjam.com/events/ludum-dare/57/deepmine)

## Why DeepMine

Traditional mining games often lack meaningful consequences for player actions. DeepMine addresses this by implementing a tension-based collapse system where every dig decision matters. Collapsed blocks are permanently lost, forcing players to balance speed against safety. This creates genuine strategic depth rather than mindless resource grinding.

## Features

**Core Gameplay Systems**
- Dynamic 2D grid system tracking block state and structural integrity
- Tension-based collapse mechanics with cascading chain reactions
- Tool-based interaction system with swappable items
- Resource economy tied to ore extraction and surface trading
- Support placement system to reinforce fragile terrain

**Technical Implementation**
- ScriptableObject-driven architecture for modular item behavior
- Custom grid interaction and collision systems
- Procedural ore generation with balanced risk-reward distribution
- Real-time physics calculations for terrain stability

## Getting Started

### Playing the Game

Visit the [itch.io project page](https://eduardo79silva.itch.io/deepmine) and launch in browser or download for Windows.

**Controls**
- WASD or Arrow Keys: Movement
- Mouse: Tool selection and interaction
- Left Click: Use equipped tool
- E: Open inventory

**Gameplay Flow - Incomplete due to time constraints**
1. Select tools from your starting loadout
2. Dig carefully while monitoring terrain stability
3. Place supports to prevent collapses
4. Collect ores and return to surface
5. Sell resources and purchase upgrades
6. Repeat with improved equipment

## System Architecture

The project follows a modular component-based architecture optimized for rapid iteration during the game jam timeframe.

**Grid System**  
The spatial hash grid provides O(1) block lookups and neighbor queries. Each cell stores state data including material type, support status, and stability values. This enables efficient real-time physics calculations across the entire playfield.

**Collapse Logic**  
Unsupported blocks trigger a breadth-first search to identify connected unstable regions. The system calculates tension propagation through adjacent cells, determining which blocks cascade into failure. Support structures interrupt this propagation by redistributing load.

**Item Framework**  
ScriptableObjects define tool properties and behaviors, allowing designers to create new items without code changes. The runtime system handles tool switching, durability tracking, and effect application through a unified interface.

**Economy Balancing**  
Ore values scale with depth and extraction difficulty. The upgrade system gates progression through cumulative earnings rather than arbitrary unlocks, ensuring players engage with the risk management mechanics.

## Technology Stack

- Unity 2D with Universal Render Pipeline
- C# gameplay programming
- Custom physics and grid systems
- ScriptableObject data architecture
- Procedural generation algorithms

## Project Status and Roadmap

**Current State**  
DeepMine represents a complete vertical slice with core mechanics, art integration, and balanced progression. The game successfully demonstrates production-quality systems architecture implemented under extreme time constraints.

**Planned Improvements**
- Expand upgrade tree with risk-based tool progression
- Implement environmental hazards such as lava flows and gas pockets
- Add mobile-friendly touch control scheme
- Develop deeper narrative framing and tutorial sequence

**Known Limitations**
- Support placemen
- Inventory System
- Save system not yet implemented
- No audio implementation due to time constraints

## Screenshots

![Terrain and Stability](https://github.com/user-attachments/assets/6a4d2652-4735-4918-b309-ff1ae68733a5)

![Underground Mining](https://github.com/user-attachments/assets/234cc573-c564-4a92-9f6c-a7d1c4ec6b5e)

## Development Context

This project was built in 72 hours for Ludum Dare 57 with the theme "Depth". The accelerated timeline required careful scope management and architectural decisions that balanced feature completeness against code quality. Despite time constraints, the codebase maintains production standards with clear separation of concerns and extensible systems.

The project demonstrates competency in gameplay programming, systems design, and rapid prototyping while maintaining code quality suitable for production environments.

## Contact

**Eduardo Silva**  
Gameplay Programmer specializing in systems design and player-driven mechanics

[Portfolio](https://eduardo79silva.github.io/) | [LinkedIn](https://www.linkedin.com/in/eduardo79silva/) | [Twitter](https://x.com/79_eduardosilva)  
eduardo4silva@gmail.com

## License

Copyright 2024 Eduardo Silva. All rights reserved.

This project was created for Ludum Dare 57. The code and assets are not available for commercial use without permission.
