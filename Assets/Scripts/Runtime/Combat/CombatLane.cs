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

        public List<PawnController> Pawns => _pawns;

        public void SpawnPawnsForCombat(CombatConfig combatConfig)
        {
            foreach (var data in combatConfig.Enemies)
            {
                var pawn = _pawnFactory.SpawnEnemy(data);
                _arrangeInLine.Add(pawn.gameObject);
                _pawns.Add(pawn);
            }
        }
    }
}