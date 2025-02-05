using System;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat
{
    public class CombatLane : MonoBehaviour
    {
        [SerializeField] private ArrangeInLine _arrangeInLine;

        [SerializeField, Required] private PawnFactory _pawnFactory;

        [SerializeField] private List<PawnController> _pawns = new List<PawnController>();

        public event Action OnPawnRemoved;

        public List<PawnController> Pawns => new(_pawns);

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
            var pawn = _pawnFactory.Spawn(data);
            _arrangeInLine.Add(pawn.gameObject);
            _pawns.Add(pawn);
            pawn.Health.OnDead += (_, _) => { RemovePawn(pawn); };
            return pawn;
        }

        private void RemovePawn(PawnController pawn)
        {
            _pawns.Remove(pawn);
            OnPawnRemoved?.Invoke();
        }

        public void Clear()
        {
            var snapshot = new List<PawnController>(_pawns);

            foreach (PawnController controller in snapshot)
            {
                RemovePawn(controller);
            }
        }
    }
}