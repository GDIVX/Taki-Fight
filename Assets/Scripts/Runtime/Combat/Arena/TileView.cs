using Runtime.Selection;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Runtime.Combat.Arena
{
    public class TileView : MonoBehaviour, ISelectableEntity, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

    
        [SerializeField] private SpriteRenderer spriteRenderer; // Reference to the sprite renderer

        private Tile tile; // Reference to the Tile object
        internal void SetTile(Tile tile)
        {
            this.tile = tile; // Set the Tile object
            spriteRenderer ??= GetComponent<SpriteRenderer>(); // Get the sprite renderer if not assigned
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
    }
}