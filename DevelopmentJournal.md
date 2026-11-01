# Development Journal

## Sprint 1 — Beginning of Ground Tile Generation

### Development

I started implementing a ground tile generation system in Unity using C#. The goal was to generate floor tiles around the player as they explore, rather than manually placing them throughout the world.

I created a `ChunkManager` script that uses the player's position to determine their current tile coordinates. I used `Mathf.FloorToInt()` to convert world positions into integer coordinates and `Vector2Int` to represent positions on the ground grid.

<img width="616" height="450" alt="Screenshot 2026-11-01 153014" src="https://github.com/user-attachments/assets/bcb606e4-c9ee-4c0c-9a9e-8a420e013098" />

### Challenges and Problem-Solving

One problem I encountered was understanding how `Instantiate()` works. I initially wasn't sure whether creating another tile would replace an existing one. I learned that `Instantiate()` creates a separate GameObject and returns a reference to it.

Another issue was preventing duplicate tiles. When the player returned to an area, the system could attempt to generate a tile that already existed.

To address this, I implemented a `Dictionary<Vector2Int, GameObject>` to store generated tiles by their coordinates. I used `ContainsKey()` to check whether a tile had already been created before spawning another one.

### Improvements

Initially, the system generated individual tiles as the player entered new areas. I later introduced nested `for` loops to generate a grid of surrounding tiles.

I also added a configurable `renderDistance` variable, allowing the number of surrounding tiles to be adjusted through Unity's Inspector.

<img width="500" height="312" alt="Screenshot 2026-11-01 180812" src="https://github.com/user-attachments/assets/fdcb395c-17d9-4eb5-9146-06fb6fe45f85" />

### Testing

I tested the system by moving the player between different ground tiles and observing how new tiles appeared. I also checked the behaviour when returning to previously visited areas.

### Next Steps

My next goal is to implement unloading for distant tiles to prevent unnecessary GameObjects from accumulating, and then add simple player movement to ensure the tile generation works as intended.

After that, I plan to introduce trees and rocks that players can break to collect resources. These resources will eventually connect to the hybrid inventory system, where items will have properties such as weight, stack limits and grid dimensions.
