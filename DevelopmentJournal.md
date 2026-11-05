# Development Journal

## Sprint 1 — Core Systems and Item Foundations

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

I then implemented tile unloading to remove tiles that were too far away from the player. This prevents unnecessary GameObjects from accumulating as the player explores.

The system now generates and removes tiles depending on the player's position, keeping only the surrounding area loaded.

### Player Movement and Camera

After implementing ground tile generation, I started working on player movement.

I created a `PlayerMovement` script that allows the player to move using WASD. Movement is relative to the camera's direction rather than the world's axes, making the controls more suitable for the game's isometric perspective.

I used a `Rigidbody` and `MovePosition()` inside `FixedUpdate()` to handle movement.

I also implemented mouse-based player rotation using raycasting. This allows the player to face the direction of the mouse cursor in the game world.

For the camera, I created a `CameraFollow` script that follows the player's position without rotating alongside them.

I later added a circular direction indicator around the player to make it easier to see which direction they are facing.

### Resource Objects

I started implementing resource objects by importing a tree model and an axe model from Blender into Unity.

One issue I encountered was that some of the tree's faces were displaying incorrectly because their normals were facing the wrong direction. I fixed this in Blender before importing the corrected model.

I also added a collider to the tree's trunk so the player cannot walk through it.

After setting up the tree, I implemented a `Tree` script that tracks the tree's health and includes logic for reducing its health and destroying the tree when it reaches zero.

I initially used `static` variables for the tree's health, but learned that this would cause every tree to share the same health values. I changed the system so each tree can have its own health.

The resource drop system has not been completed yet and will be implemented during Sprint 2.

### Item Data System

Towards the end of Sprint 1, I started working on the foundations of the inventory system.

I created an `ItemData` class using Unity's `ScriptableObject` system. This allows me to create different item assets and define their properties without needing a separate script for every item.

Each item can have the following properties:

- `itemName` - The item's name
- `icon` - The sprite used to represent the item in the UI
- `worldModel` - The 3D model used to display the item
- `weight` - The item's weight, which will be used for inventory capacity
- `maxStackSize` - The maximum number of items that can be stored in one stack
- `gridSize` - The amount of space the item occupies in the inventory grid

I created a wood item asset and imported a log model to use as its world representation.

One problem I encountered was assigning the 3D model to the `worldModel` field. I learned that the field requires a GameObject reference, so I created a prefab for the log model.

Using ScriptableObjects should make it easier to add more items later, as their properties can be changed directly through Unity's Inspector.

### Testing

I tested the ground tile system by moving the player between different areas and observing how tiles were generated and unloaded. I also checked what happened when returning to previously visited areas.

I tested player movement and mouse-based rotation to make sure the controls worked correctly with the camera.

For the resource objects, I checked the imported tree model and its collision setup.

I also checked that the wood item asset could store its properties and reference the correct world model prefab.

### Sprint 1 Reflection

During Sprint 1, I implemented several of the basic systems needed for the game, including ground tile generation, tile unloading, player movement, camera movement and the beginnings of resource interaction.

One of the main things I learned was how to manage GameObjects through code, particularly using dictionaries to keep track of generated tiles. I also gained more experience with Unity's physics system, raycasting and importing 3D models.

I also learned how ScriptableObjects can be used to separate item data from the scripts that will interact with those items. This will be useful for the hybrid inventory system, since different items will need different weights, stack limits and grid dimensions.

Although the foundations are now in place, resource drops, item collection and inventory storage are not yet functional. These will be developed during Sprint 2.

### Next Steps — Sprint 2

For Sprint 2, I plan to focus on resource generation and the inventory backend.

I will start by implementing a universal item pickup system that can be used for different types of resources. I will then connect this system to destructible objects, allowing trees to drop wood when destroyed.

After this, I will begin implementing the inventory's internal functionality, including item storage, stacking, weight limits and grid placement.

The inventory interface and item organisation features, such as dragging and rotating items, will be developed during Sprint 3.