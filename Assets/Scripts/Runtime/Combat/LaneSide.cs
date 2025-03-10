using System;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat
{
    [Serializable]
    public class LaneSide
    {
        [SerializeField] private ArrangeInLine _arrangeInLine;
        [SerializeField] private List<PawnController> _pawns = new List<PawnController>();
        private PawnFactory PawnFactory => ServiceLocator.Get<PawnFactory>();

        public int PawnsLimit { get; set; } = 3;


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
            if (_pawns.Count >= PawnsLimit) return null;

            var pawn = PawnFactory.Spawn(data);
            _arrangeInLine.Add(pawn.gameObject);
            _pawns.Add(pawn);
            pawn.Health.OnDead += (_, _) => { RemovePawn(pawn); };
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
    }
}