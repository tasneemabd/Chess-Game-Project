# Chess-Game
This project is a chess game built with Unity. It features an interactive chessboard with playable white and black teams.


## Game Mode
- **Single-Player Mode**: The game allows one player to manually move chess pieces for both white and black teams.

## Assumptions and Design Decisions
- The chess game currently doesn't include AI for single-player mode, so both white and black moves must be manually controlled by the player.
- Chess rules like checkmate and piece movement are partially implemented; edge cases may not yet be handled.
- The game's orientation is set based on the player's team. Team 0 (White) faces one direction, and Team 1 (Black) faces the opposite.
- Victory detection occurs when a king is captured.

## Running the Project Locally
1. Clone the repository:
   ```bash
   git clone https://github.com/tasneemabd/Chess-Game.git
2. Open the project in Unity (version 2022.3 or newer recommended).
3. Press the Play button in the Unity Editor to start the game.

##  Layers Configuration
Ensure the following layers exist in the Unity project:

- Tile: Default layer for the chessboard tiles.
- Hover: Layer for highlighting hovered tiles.
- Highlight: Layer for highlighting valid moves.

##  Controls

- Left Mouse Button: Select and move pieces.
- Right Mouse Button: Reset the camera view.
##  Notes

* The game is designed as a local prototype and does not currently support online multiplayer or advanced chess mechanics like en passant or castling.
