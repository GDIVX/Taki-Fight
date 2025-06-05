# Editor Tools

This directory contains custom Unity editor windows used to manage game data and sprites.

## Art Manager

The **Art Manager** window lists all `CardData` and `PawnData` assets so you can assign sprites to them.

### Opening the Window

1. In Unity, open the **Tools** menu.
2. Choose **Art Manager**.

### Importing Sprites

1. Click **Refresh Art Status** to populate the list of assets.
2. For the desired asset, click **Import Sprite**.
3. Select a `.png` or `.jpg` file. The sprite is copied into the project and assigned automatically.
4. Use **Delete Outdated Sprites** to remove previous versions from the sprite folders.

## Gameplay Data Editor

The **Gameplay Data Editor** lets you browse and create gameplay assets such as cards and pawns.

### Opening the Window

1. In Unity, open the **Tools** menu.
2. Choose **Gameplay Data Editor**.

### Creating New Assets

1. Click **Create New Asset** at the top of the window.
2. Pick the asset type (e.g. `CardData`, `PawnData`, or `School`).
3. Enter a name and confirm to create the asset inside `Assets/Resources/Data`.

Use the tree on the left to select an asset and edit its properties in the inspector.
