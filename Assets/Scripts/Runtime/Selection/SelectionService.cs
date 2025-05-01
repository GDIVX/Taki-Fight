using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Runtime.Selection
{
    public class SelectionService : Singleton<SelectionService>
    {
        [SerializeField] private SelectionLinePointer _linePointer;
        [SerializeField] private List<Button> _buttonsToDisableDuringSearch;
        [ShowInInspector, ReadOnly] public SelectionState CurrentState { get; private set; } = SelectionState.None;
        [ShowInInspector, ReadOnly] public List<ISelectableEntity> SelectedEntities { get; private set; } = new();
        public event Action<List<ISelectableEntity>> OnSelectionComplete;
        public event Action OnSearchCanceled;
        public event Action<Predicate<ISelectableEntity>> OnSearchInitialized;

        public Predicate<ISelectableEntity> Predicate { get; private set; }
        public Vector2Int SearchSize { get; internal set; }

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
            Action<List<ISelectableEntity>> onComplete, Action onCanceled, Vector3 pointerOrigin)
        {
            if (CurrentState == SelectionState.InProgress)
            {
                throw new InvalidOperationException("A selection is already in progress.");
            }

            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            OnSelectionComplete = onComplete;
            OnSearchCanceled = onCanceled;
            _requiredSelections = count;
            SelectedEntities.Clear();
            CurrentState = SelectionState.InProgress;

            if (_requiredSelections == 0)
            {
                var completeSelection = CompleteSelection();
                StartCoroutine(completeSelection);
                return;
            }

            _buttonsToDisableDuringSearch.ForEach(button => button.interactable = false);

            _linePointer.Show(pointerOrigin);

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
            OnSearchCanceled?.Invoke();
            _linePointer.Hide();
            _buttonsToDisableDuringSearch.ForEach(button => button.interactable = true);
            SearchSize = Vector2Int.zero;
        }

        private IEnumerator CompleteSelection()
        {
            yield return new WaitForEndOfFrame();
            CurrentState = SelectionState.Complete;
            _allSelectable.ForEach(e => e.OnDeselected());
            OnSelectionComplete?.Invoke(SelectedEntities);
            Predicate = null;
            _linePointer.Hide();
            _buttonsToDisableDuringSearch.ForEach(button => button.interactable = true);
            SearchSize = Vector2Int.zero;
        }

        public void Register(ISelectableEntity entity)
        {
            _allSelectable.Add(entity);
        }
    }
}