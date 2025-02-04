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

        [SerializeField, Required, TabGroup("Enemies")]
        private PawnFactory _pawnFactory;

        [SerializeField] private List<PawnController> _pawns = new List<PawnController>();

        public event Action OnPawnRemoved;

        public List<PawnController> Pawns => new List<PawnController>(_pawns);

        public void SpawnPawnsForCombat(CombatConfig combatConfig)
        {
            foreach (var data in combatConfig.Enemies)
            {
                var pawn = _pawnFactory.SpawnEnemy(data);
                _arrangeInLine.Add(pawn.gameObject);
                _pawns.Add(pawn);
                pawn.Health.OnDead += (sender, args) => { RemovePawn(pawn); };
            }
        }

        private void RemovePawn(PawnController pawn)
        {
            _pawns.Remove(pawn);
            if (pawn.isActiveAndEnabled)
            {
                Destroy(pawn.gameObject);
            }

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