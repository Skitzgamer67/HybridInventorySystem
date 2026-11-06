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

## Sprint 2 — Inventory Backend and Resource Generation

### Development

I started Sprint 2 by implementing the resource drop system that I had planned during Sprint 1.

I created an `ItemPickup` script that can be used for different items rather than creating a separate pickup script for each resource. The script uses the existing `ItemData` assets to determine which item is being dropped and how many are in the stack.

I connected this system to the `Tree` script so that destroying a tree spawns a wood log pickup. The amount of wood dropped is randomly generated between 5 and 9.

I also created an `ItemRotation` script to make dropped items slowly rotate and float up and down using `Mathf.Sin()`.

### Procedural Tree Generation

After implementing resource drops, I expanded the `ChunkManager` script to generate trees alongside the ground tiles.

Initially, I had several trees spawning in each chunk, but this caused the trees to overlap and made the environment look too crowded.

<img width="987" height="625" alt="Screenshot 2026-11-06 164148" src="https://github.com/user-attachments/assets/d03fc815-bcae-4629-a07f-7c87857797f5" />

To improve this, I reduced the number of trees per chunk and added a configurable spawn chance. This allows some chunks to generate without trees, making the environment less crowded.

I also added a `worldSeed` variable and used `System.Random` to generate consistent tree positions based on each chunk's coordinates.

This means that trees will appear in the same positions when the player leaves an area and returns.

### Challenges and Problem-Solving

One issue I encountered was that trees would respawn in different positions whenever their chunks were unloaded and generated again.

This also meant that trees which had already been destroyed would reappear when returning to the area.

To fix this, I implemented a `Dictionary<Vector2Int, HashSet<int>>` to track destroyed trees. Each tree is assigned an ID based on its position in the chunk's generation process.

When a tree is destroyed, its ID and chunk coordinates are recorded. The generation system then checks this information before spawning trees.

This allows destroyed trees to remain destroyed when revisiting chunks during the same play session.

### Inventory Backend

I then started implementing the inventory's internal functionality.

I created an `InventorySlot` class to store an item's data, quantity, grid position and rotation.

I also created an `Inventory` script to manage the items stored in the player's inventory.

The inventory uses a configurable grid, currently set to 8 columns and 6 rows. Each item uses its `gridSize` property to determine how much space it occupies.

I implemented a method that checks whether an item can fit at a particular position by comparing its occupied cells with the positions of other items.

I also added item stacking, allowing items of the same type to be combined into existing stacks until their maximum stack size is reached.

### Weight System

I implemented a weight system using the `weight` property from `ItemData`.

The inventory calculates its current weight by multiplying each item's weight by its quantity and adding the results together.

I also added a configurable maximum weight, which is currently set to 50.

When collecting items, the system checks how much weight is available and only adds the quantity that can fit within the limit.

One issue I encountered during testing was that the inventory weight remained at zero even after collecting several wood logs.

I initially thought the calculation was not working, but found that the wood item's weight had not been configured in its ScriptableObject asset.

After setting the weight in Unity's Inspector, the inventory began calculating the total correctly.

### Automatic Item Collection

Initially, the player had to press E while standing near a dropped item to collect it.

I later changed this to an automatic pickup system.

<img width="570" height="351" alt="Screenshot 2026-11-05 225054" src="https://github.com/user-attachments/assets/98fe3dee-ea7a-480d-9348-627baec345f0" />

When the player moves within a configurable range, the dropped item moves towards the player using `Vector3.MoveTowards()`.

I also added a shrinking effect so the item gradually becomes smaller while moving towards the player.

Once the item reaches the player, it is added to the inventory and the pickup GameObject is destroyed.

If the inventory cannot hold the entire stack, the remaining items are left on the ground instead.

### Item Pickup Indicators

I also added floating text labels above dropped items using TextMeshPro.

The labels display the item's name and quantity, such as `Wood Log x7`.

<img width="556" height="229" alt="Screenshot 2026-11-06 201106" src="https://github.com/user-attachments/assets/93876f1e-cb6e-4de7-ac07-5ac8b06d4e72" />

The text is generated using the information stored in the item's `ItemData` and the pickup's quantity.

I made the labels face the camera so they remain readable from the game's isometric perspective.

The labels also follow the dropped items during collection and shrink alongside them.

### Testing

I tested the resource drop system by destroying trees and checking that wood logs spawned with different quantities.

I also tested procedural tree generation by moving between chunks and returning to previously visited areas. The trees retained their original positions, and destroyed trees did not respawn.

For the inventory, I collected multiple wood pickups and checked the results using messages printed to Unity's Console.

<img width="342" height="198" alt="Screenshot 2026-11-06 181914" src="https://github.com/user-attachments/assets/78c04e80-9479-46c5-8247-be1db1162db4" />

I confirmed that items of the same type were combined into stacks and that the inventory stored their quantities and grid positions.

I also tested the weight calculation after configuring the wood item's weight.

Finally, I tested automatic collection by approaching dropped items and checking that they moved towards the player, shrank and were added to the inventory without requiring a key press.

### Sprint 2 Reflection

During Sprint 2, I completed the resource drop system and implemented the main backend functionality for the inventory.

One of the main things I learned was how to use dictionaries and unique IDs to keep track of changes to procedurally generated objects.

I also gained more experience with ScriptableObjects by connecting item data to world pickups and inventory storage.

Implementing stacking and weight calculations helped me understand how different inventory restrictions can work together.

I also learned that some problems are caused by incorrect Inspector settings rather than errors in the scripts, as demonstrated by the issue with the wood item's weight.

The inventory can now store items and manage their quantities, weights and grid positions, but there is not yet a visual interface for organising them.

### Next Steps — Sprint 3

For Sprint 3, I plan to implement the visual inventory interface.

I will start by creating an 8×6 inventory grid that displays the items currently stored in the player's inventory.

I will then add item icons or 3D previews, along with quantity indicators for stacked items.

After this, I plan to implement drag-and-drop functionality so items can be moved around the grid.

I will also add item rotation, allowing larger items to be rotated to fit into different spaces.

Finally, I will work on stack splitting and merging, along with a visual indicator showing the inventory's current weight and maximum capacity.

## Sprint 3 — Inventory User Interface

### Development

I started Sprint 3 by creating the visual interface for the inventory system that I implemented during Sprint 2.

I created an `InventoryCanvas`, `InventoryPanel` and `InventoryGrid` in Unity. I then created an `InventoryUI` script to manage the interface.

The inventory opens and closes using the Tab key. I used nested `for` loops to automatically generate an 8×6 grid containing 48 cells.

Each item is displayed based on its position and size stored in the `InventorySlot` class.

### Item Previews

Initially, I displayed items using coloured rectangles with their names and quantities.

I wanted the inventory to display the actual appearance of each item instead, so I started experimenting with Render Textures.

I created a separate camera that renders 3D models into a texture. This allowed me to manually arrange the models to create the appearance I wanted for each item.

I added a `previewTexture` property to `ItemData` so different items could have their own inventory previews.

I also changed the property to use `Texture`, allowing both regular images and Render Textures to be assigned through Unity's Inspector.

Each item now displays its preview with a quantity indicator in the bottom-right corner.

### Inventory Interactions

After creating the visual grid, I worked on allowing the player to organise items.

I added drag-and-drop functionality to move items between grid positions.

I also added the ability to rotate items by pressing R while dragging them. This allows items with different grid dimensions to fit into different spaces.

I added right-click stack splitting and the ability to merge matching stacks by dragging them onto each other.

I also created a tooltip that displays an item's name, quantity and description when the mouse is hovering over it.

### Weight Display and Starting Equipment

I added a weight indicator to the inventory interface, showing the current weight of the stored items.

This uses the weight calculations already implemented in Sprint 2.

I also updated the `PlayerInventory` script so the player starts with an axe.

Finally, I added TextMeshPro instructions to explain the controls for moving, rotating and organising items.

### Challenges and Problem-Solving

One problem I encountered was that the inventory grid was not appearing when I opened the interface.

I found that I had forgotten to call the `CreateGrid()` method when the game started. After fixing this, all 48 cells appeared correctly.

Another issue was displaying the 3D item previews. I initially had problems with the Render Texture background and the texture becoming black during Play Mode.

I found that an earlier preview rendering script was disabling the camera. After correcting the camera setup, the Render Texture displayed correctly.

I also noticed that item previews could become stretched when an item occupied more than one grid cell.

To address this, I used an `AspectRatioFitter` to preserve the image's original proportions.

When implementing the starting axe, I encountered a `NullReferenceException` because the `PlayerInventory` script was not correctly referencing the `Inventory` component.

I fixed this by using `GetComponent<Inventory>()` to retrieve the component from the player.

### Testing

I tested the inventory interface by collecting wood logs and opening the inventory using Tab.

I confirmed that the collected items appeared in the grid with their quantities.

I also checked that the custom Render Texture previews displayed correctly and that the starting axe was added to the inventory.

I tested the inventory controls by moving items between cells, rotating them, splitting stacks and merging matching items.

I also checked the item tooltips and weight indicator.

### Sprint 3 Reflection

During Sprint 3, I created the visual interface for the inventory system and connected it to the backend from Sprint 2.

One of the main things I learned was how to use RectTransforms to position UI elements and generate inventory cells through code.

I also learned how Render Textures can be used to display 3D models inside Unity's UI.

Working on the inventory interactions helped me understand how mouse input can be used to move and organise items within a grid.

I also gained more experience debugging problems involving component references and Inspector settings.

The inventory now has a visual interface for displaying and organising items, along with quantity indicators, tooltips and a weight display.

