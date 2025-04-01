using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Combat.Pawn;
using Runtime.Selection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Runtime.Combat.Arena
{
    [Serializable]
    public class LaneSide : ISelectableEntity, IPointerClickHandler
    {
        [SerializeField] private ArrangeInLine _arrangeInLine;
        [SerializeField] private List<PawnController> _pawns = new List<PawnController>();
        [ShowInInspector, ReadOnly] private CombatLane _lane;
        [SerializeField] private LaneView _view;
        private PawnFactory PawnFactory => ServiceLocator.Get<PawnFactory>();

        public int PawnsLimit { get; set; } = 3;
        public CombatLane Lane => _lane;
        public bool IsAllySide { get; private set; }


        public event Action OnPawnRemoved;

        public List<PawnController> Pawns => new(_pawns);
        public int Count => Pawns.Count;

        public LaneSide Other()
        {
            return IsAllySide ? _lane.EnemySide : _lane.AllySide;
        }

        public PawnController First()
        {
            return _pawns.Count == 0 ? null : Pawns?.First();
        }

        public void SpawnPawnsForCombat(CombatConfig combatConfig, Action onComplete)
        {
            foreach (var data in combatConfig.Enemies)
            {
                AddPawn(data);
            }

            onComplete?.Invoke();
        }

        public PawnController AddPawn(PawnData data)
        {
            if (_pawns.Count >= PawnsLimit) return null;

            var pawn = PawnFactory.Spawn(data);
            _arrangeInLine.Add(pawn.gameObject);
            _pawns.Add(pawn);
            pawn.SetPosition(_lane, this);
            pawn.Health.OnDead += (_, _) => { RemovePawn(pawn); };

            pawn.GetComponent<SpriteRenderer>().flipX = !IsAllySide;
            return pawn;
        }

        public void RemovePawn(PawnController pawn)
        {
            // if (!pawn.Health.IsDead())
            // {
            //     pawn.Health.Die();
            // }

            _pawns.Remove(pawn);
            _arrangeInLine.Remove(pawn.gameObject);
            OnPawnRemoved?.Invoke();
        }

        [Button]
        public void Clear()
        {
            var snapshot = new List<PawnController>(_pawns);

            foreach (PawnController controller in snapshot)
            {
                RemovePawn(controller);
            }
        }

        public void Init(CombatLane combatLane, bool isAllySide)
        {
            _lane = combatLane;
            IsAllySide = isAllySide;
        }

        #region Selection

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
            _view.OnSelected();
        }

        public void OnDeselected()
        {
            _view.ClearSelection();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
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

        #endregion
    }
}