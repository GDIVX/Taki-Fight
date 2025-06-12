# View Components Guide

This document outlines the main visual components in **Taki Fight**. It is intended for UX designers and artists who want a quick overview of the in‑game views that can be styled or updated. The focus here is on what each component represents in the game rather than how it works under the hood.

## Card Gameplay

### CardView
Represents an individual card. It displays the card art, title, description and energy cost. When players hover over a card it scales up slightly to draw attention.

### RewardCardView
Used in the reward screen after battles. It shows a preview of a card so players can decide which reward to pick. Hovering enlarges the card; clicking selects it.

### HandView and HorizontalCardListView
`HandView` arranges the player’s cards in an arc at the bottom of the screen. It fades in at the start of a turn and fades out at the end. The underlying `HorizontalCardListView` handles the curved layout and smooth card rearranging animations.

### DeckView
Shows how many cards remain in the draw pile, discard pile and other piles. It also indicates how many cards will be drawn at the start of a turn.

### EnergyView
Displays the player’s current energy and the amount gained each turn.

## Combat

### PawnView
Visual representation of a familiar or enemy on the battlefield. It shows health, defense, attacks and movement animations. The sprite can flash or dissolve when hit or destroyed.

### TilemapView and TileView
`TilemapView` creates the grid used during combat. Each individual square is a `TileView`. Tiles change color to indicate who owns them or when they are highlighted.

### StatusEffectView and StatusEffectListView
These components display status effect icons (for example poison or stun) above a pawn. The list view collects multiple effects while the individual view shows an icon and stack count.

### HealthBarUI and MageHealthUI
`HealthBarUI` is a generic bar showing current versus maximum health. `MageHealthUI` is a variation used for the player’s castle or mage.

## User Interface Utilities

### MessageView and MessageViewContainer
Short messages such as “Turn Start” or “Combo!” slide onto the screen using these components. The container keeps a small pool of message views so new art can be swapped in easily.

### Tooltip System
`TooltipController`, `TooltipCaller` and related classes manage pop‑up tooltips. These provide extra details when the cursor hovers over an element. The look of the tooltip’s background, icon and text can be customized.

### UIOutline
A simple outline graphic used to highlight cards or buttons. Artists can tweak its color and width to match the UI style.

---

For any visual changes, update the relevant prefabs located under `Assets/Prefabs`. Keeping these components consistent will help maintain a clear and polished user interface.
