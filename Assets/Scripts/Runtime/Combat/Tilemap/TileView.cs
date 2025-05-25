using System;
using DG.Tweening;
using Runtime.Selection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Combat.Tilemap
{
    public class TileView : MonoBehaviour, ISelectableEntity, IPointerClickHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        private static Action _onClearHighlights;
        [SerializeField] private SpriteRenderer spriteRenderer; // Reference to the sprite renderer

        [ShowInInspector, ReadOnly] private Tile tile;

        public Tile Tile
        {
            get => tile;
            private set => tile = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null)
            {
                Debug.LogWarning("OnPointerClick called with null eventData.");
                return;
            }

            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (SelectionService.Instance.CurrentState != SelectionState.InProgress) return;

            TryToSelect();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (SelectionService.Instance.CurrentState == SelectionState.InProgress)
            {
                // Trigger AOE visualization on hover
                AOEHiighlight.HighlightForSelection(tile); // Highlight the tile for AOE selection
            }
            else if (tile.Pawn)
            {
                tile.Pawn.OnPinterEnter(eventData); // Trigger the pawn's OnPointerEnter method if it exists
                // Highlight the tile in green if the pawn is on it
                Highlight(Color.green);
            }
            else
            {
                Highlight(Color.yellow); // Highlight the tile in yellow
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ClearHighlights();
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

        internal void Init(Tile tile)
        {
            Tile = tile; // Set the Tile object
            Tile.SetView(this);
            spriteRenderer ??= GetComponent<SpriteRenderer>();

            //name the gameobject after the tile's coordinates
            gameObject.name = $"Tile({tile.Position.x},{tile.Position.y})";

            OnOwnerModified(); // Call the method to set the initial color based on the owner

            _onClearHighlights += () =>
            {
                // Clear AOE visualization when hover ends
                ClearHighlight();
                AOEHiighlight.ClearHighlights(); // Clear the AOE highlights
            };
        }

        public void Highlight(Color color)
        {
            spriteRenderer.DOColor(color, 0.3f).SetEase(Ease.InOutSine).onComplete +=
                () => spriteRenderer.color = color;
        }

        public void ClearHighlight()
        {
            OnOwnerModified(); // Reset to the default color based on ownership
        }

        private static void ClearHighlights()
        {
            _onClearHighlights?.Invoke();
        }


        internal void OnOwnerModified()
        {
            // Change color based on the owner using a shader or color assignment
            var owner = tile.Owner;

            var color = Color.white;
            switch (owner)
            {
                case TileOwner.None:
                    color = Color.white; // Default color for no owner
                    break;
                case TileOwner.Player:
                    color = Color.blue; // Color for player-owned tiles
                    break;
                case TileOwner.Enemy:
                    color = Color.red; // Color for enemy-owned tiles
                    break;
                default:
                    Debug.LogWarning("Unknown TileOwner type.");
                    spriteRenderer.color = Color.white; // Fallback color
                    break;
            }

            Highlight(color);
        }

        public bool IsAnimating()
        {
            return DOTween.IsTweening(spriteRenderer);
        }
    }
}