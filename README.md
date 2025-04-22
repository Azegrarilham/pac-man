# Pac-Man Clone

A Unity-based recreation of the classic Pac-Man arcade game, featuring authentic ghost behavior and classic gameplay mechanics.


## 🎮 Game Features

- Classic Pac-Man gameplay mechanics
- Four unique ghosts with distinct AI behaviors:
  - Blinky (Red): Direct chase
  - Pinky (Pink): Ambush ahead of Pac-Man
  - Inky (Blue): Cooperative chase with Blinky
  - Clyde (Orange): Alternates between chase and scatter
- Power pellets and score multipliers
- Multiple levels with increasing difficulty
- Original-style ghost house mechanics
- Wrap-around tunnel system

## 🎯 Ghost Behaviors

### Ghost Modes
- **Chase Mode**: Each ghost uses their unique strategy to pursue Pac-Man
- **Scatter Mode**: Ghosts retreat to their designated corners
- **Frightened Mode**: Ghosts turn blue and flee from Pac-Man

### Individual Ghost AI
- **Blinky**: Directly targets Pac-Man's position
- **Pinky**: Aims 4 tiles ahead of Pac-Man's current direction
- **Inky**: Uses Blinky's position to set up ambushes
- **Clyde**: Switches between chase and scatter based on distance to Pac-Man

## 🕹️ Controls

- **Arrow Keys/WASD**: Move Pac-Man
- **ESC**: Pause game

## 🔧 Technical Features

- Node-based movement system
- Efficient ghost pathfinding
- Smooth animation transitions
- Dynamic difficulty scaling
- Score system with multipliers
- Life system with game over handling

## 📝 Implementation Details

### Core Systems
- Node-based navigation grid
- Ghost state management
- Pellet collection system
- Power pellet mechanics
- Score tracking
- Life system

### Ghost AI Components
- Pathfinding algorithms
- State machines for behavior
- Target calculation
- Mode switching timers

## 🛠️ Built With

- Unity 2D
- C# Scripts
- Unity's New Input System
- Unity Animation System

## 🎨 Visual Assets

- Classic Pac-Man style sprites
- Animated characters
- Score display
- Power pellet effects

## 🔄 Game Loop

1. Initialize maze and characters
2. Player collects pellets while avoiding ghosts
3. Power pellets enable ghost hunting
4. Complete level when all pellets are collected
5. Increase difficulty and repeat

## 📚 Resources
🔗 [The Pac-Man Dossier](https://www.gamedeveloper.com/design/the-pac-man-dossier) — Deep breakdown of the original game's logic and AI.

## 💡 Future Improvements

- [ ] High score system
- [ ] Additional mazes
- [ ] Sound effects and music
- [ ] Mobile touch controls
- [ ] Multiplayer mode

## 🤝 Contributing

Feel free to fork and submit pull requests. For major changes, please open an issue first.

## 🎓 Acknowledgments

- Original Pac-Man game by Namco
- Unity game engine
- Community resources and tutorials
