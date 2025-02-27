using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.Selection
{
    public class SelectionService : Singleton<SelectionService>
    {
        [ShowInInspector, ReadOnly] public SelectionState CurrentState { get; private set; } = SelectionState.None;
        [ShowInInspector, ReadOnly] public List<ISelectableEntity> SelectedEntities { get; private set; } = new();
        public event Action<List<ISelectableEntity>> OnSelectionComplete;
        public event Action<Predicate<ISelectableEntity>> OnSearchInitialized;

        public Predicate<ISelectableEntity> Predicate { get; private set; }
        [ShowInInspector, ReadOnly] private int _requiredSelections;
        [ShowInInspector, ReadOnly] private List<ISelectableEntity> _allSelectable = new();

        private void Update()
        {
            if (CurrentState != SelectionState.InProgress) return;

            if (Input.GetMouseButtonUp(1)) // Right-click cancels selection
            {
                var cancelSelection = CancelSelection();
                StartCoroutine(cancelSelection);
            }
        }

        [Button]
        public void RequestSelection(Predicate<ISelectableEntity> predicate, int count,
            Action<List<ISelectableEntity>> onComplete)
        {
            if (CurrentState == SelectionState.InProgress)
            {
                throw new InvalidOperationException("A selection is already in progress.");
            }

            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            OnSelectionComplete = onComplete;
            _requiredSelections = count;
            SelectedEntities.Clear();
            CurrentState = SelectionState.InProgress;

            if (_requiredSelections == 0)
            {
                var completeSelection = CompleteSelection();
                StartCoroutine(completeSelection);
                return;
            }

            OnSearchInitialized?.Invoke(predicate);
        }


        public void Select(ISelectableEntity entity)
        {
            if (CurrentState != SelectionState.InProgress) return;
            if (!IsValid(entity) || SelectedEntities.Contains(entity))
            {
                var cancelSelection = CancelSelection();
                StartCoroutine(cancelSelection);
                return;
            }

            SelectedEntities.Add(entity);
            entity.OnSelected();

            if (SelectedEntities.Count >= _requiredSelections)
            {
                var completeSelection = CompleteSelection();
                StartCoroutine(completeSelection);
            }
        }

        private bool IsValid(ISelectableEntity entity) => Predicate?.Invoke(entity) ?? false;

        private IEnumerator CancelSelection()
        {
            if (CurrentState != SelectionState.InProgress) yield break;

            yield return new WaitForEndOfFrame();
            CurrentState = SelectionState.Canceled;
            _allSelectable.ForEach(e => e.OnDeselected());
            SelectedEntities.Clear();
            Predicate = null;
            OnSelectionComplete?.Invoke(SelectedEntities);
        }

        private IEnumerator CompleteSelection()
        {
            yield return new WaitForEndOfFrame();
            CurrentState = SelectionState.Complete;
            _allSelectable.ForEach(e => e.OnDeselected());
            OnSelectionComplete?.Invoke(SelectedEntities);
            Predicate = null;
        }

        public void Register(ISelectableEntity entity)
        {
            _allSelectable.Add(entity);
        }
    }
}