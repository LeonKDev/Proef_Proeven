# Player System

This folder contains the player-specific scripts that have been separated from the ball movement scripts. This separation provides better code organization, clearer responsibilities, and more maintainable code.

## Architecture Overview

### Player Scripts

- **Player**: The main hub class that ensures all player components are connected.
- **PlayerController**: Handles player input processing, ball targeting, and high-level game logic.
- **PlayerBatHandler**: Handles bat-specific functionality like swinging and hitting the ball.

### Multi-Ball Support

The player system is designed to work with multiple balls in the scene:

- `PlayerController` maintains a list of active ball references
- `BallRegistrationManager` automatically tracks ball creation/destruction
- The player always targets the closest ball within hitting range
- Each ball maintains its own state and behavior

### Interaction with Ball System

The player scripts interact with the ball through the BallController's public methods and through its component handlers (BallBoostHandler, BallMovementHandler). This separation allows both systems to evolve independently while maintaining a clean API between them.

## Usage

1. Add the `Player` component to your player GameObject.
2. Configure any necessary references in the Inspector.
3. Ensure the `BallRegistrationManager` is in the scene (it will be created automatically if not present).
4. Instantiate as many balls as needed - they will automatically register with the player.

## Dependencies

- Requires the ball to have a `BallController` component.
- Ball should have the appropriate tag for targeting ("Ball").
- Boss should have the appropriate tag for targeting ("Boss").