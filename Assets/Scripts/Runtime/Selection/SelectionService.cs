using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.Selection
{
    public class SelectionService : Singleton<SelectionService>
    {
        public SelectionState CurrentState { get; private set; } = SelectionState.None;
        public List<ISelectableEntity> SelectedEntities { get; private set; } = new();

        private Predicate<ISelectableEntity> _predicate;
        private Action<List<ISelectableEntity>> _onSelectionComplete;
        private int _requiredSelections;

        private void Update()
        {
            if (CurrentState != SelectionState.InProgress) return;

            if (Input.GetMouseButtonUp(1)) // Right-click cancels selection
            {
                CancelSelection();
            }
            else if (Input.GetMouseButtonUp(0)) // Left-click attempts selection
            {
                TrySelectEntityUnderMouse();
            }
        }

        [Button]
        public void RequestSelection(Predicate<ISelectableEntity> predicate, int count, Action<List<ISelectableEntity>> onComplete)
        {
            if (CurrentState == SelectionState.InProgress)
            {
                throw new InvalidOperationException("A selection is already in progress.");
            }

            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _onSelectionComplete = onComplete;
            _requiredSelections = count;
            SelectedEntities.Clear();
            CurrentState = SelectionState.InProgress;

            if (_requiredSelections == 0)
            {
                CompleteSelection(); // Immediately resolve if no selections are required
            }
        }

        private void TrySelectEntityUnderMouse()
        {
            // If clicking on UI, do nothing (prevents false positives)
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Camera.main == null) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out ISelectableEntity entity))
                {
                    Select(entity);
                }
                else
                {
                    CancelSelection(); // Clicked on something invalid
                }
            }
            else
            {
                CancelSelection(); // Clicked on empty space
            }
        }


        public void Select(ISelectableEntity entity)
        {
            if (CurrentState != SelectionState.InProgress) return;
            if (!IsValid(entity) || SelectedEntities.Contains(entity))
            {
                CancelSelection(); // Clicking an invalid entity cancels the selection
                return;
            }

            SelectedEntities.Add(entity);

            if (SelectedEntities.Count >= _requiredSelections)
            {
                CompleteSelection();
            }
        }

        public bool IsValid(ISelectableEntity entity) => _predicate?.Invoke(entity) ?? false;

        public void CancelSelection()
        {
            if (CurrentState != SelectionState.InProgress) return;

            CurrentState = SelectionState.Canceled;
            SelectedEntities.Clear();
            _predicate = null;
            _onSelectionComplete = null;
        }

        private void CompleteSelection()
        {
            CurrentState = SelectionState.Complete;
            _onSelectionComplete?.Invoke(SelectedEntities);
            _predicate = null;
            _onSelectionComplete = null;
        }
    }
}
