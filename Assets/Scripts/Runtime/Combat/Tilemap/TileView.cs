using Runtime.Selection;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Tilemap
{
    public class TileView : MonoBehaviour, ISelectableEntity, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {


        [SerializeField] private SpriteRenderer spriteRenderer; // Reference to the sprite renderer

        [ShowInInspector, ReadOnly] private Tile tile;

        public Tile Tile { get => tile; private set => tile = value; }

        internal void Init(Tile tile)
        {
            Tile = tile; // Set the Tile object
            Tile.SetView(this);
            spriteRenderer ??= GetComponent<SpriteRenderer>();

            //name the gameobject after the tile's coordinates
            gameObject.name = $"Tile({tile.Position.x},{tile.Position.y})";

            OnOwnerModified(); // Call the method to set the initial color based on the owner
        }


        public void TryToSelect()
        {
            if (SelectionService.Instance.CurrentState != SelectionState.InProgress)
            {
                return;
            }

            var predicate = SelectionService.Instance.Predicate;
            if (predicate.Invoke(this))
            {
                SelectionService.Instance.Select(this);
            }
        }

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null)
            {
                Debug.LogWarning("OnPointerClick called with null eventData.");
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (SelectionService.Instance.CurrentState != SelectionState.InProgress)
            {
                return;
            }

            TryToSelect();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            //change color to indicate hover
            spriteRenderer.color = Color.yellow;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            //reset color
            spriteRenderer.color = Color.white;
        }

        internal void OnOwnerModified()
        {
            // Change color based on the owner using a shader or color assignment
            var owner = tile.Owner;

            switch (owner)
            {
                case TileOwner.None:
                    spriteRenderer.color = Color.white; // Default color for no owner
                    break;
                case TileOwner.Player:
                    spriteRenderer.color = Color.blue; // Color for player-owned tiles
                    break;
                case TileOwner.Heartland:
                    spriteRenderer.color = Color.cyan; // Color for Heartland tiles
                    break;
                case TileOwner.castle:
                    spriteRenderer.color = Color.magenta; // Color for castle tiles
                    break;
                case TileOwner.Enemy:
                    spriteRenderer.color = Color.red; // Color for enemy-owned tiles
                    break;
                default:
                    Debug.LogWarning("Unknown TileOwner type.");
                    spriteRenderer.color = Color.white; // Fallback color
                    break;
            }
        }

    }
}