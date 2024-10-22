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
        [SerializeField] private ArrangeInLine arrangeInLine;

        [SerializeField, Required, TabGroup("Enemies")]
        private PawnFactory pawnFactory;

        [SerializeField] private List<PawnController> pawns = new List<PawnController>();

        public PawnController[] Pawns => pawns.ToArray();

        public void SpawnPawnsForCombat(CombatConfig combatConfig)
        {
            foreach (var data in combatConfig.Enemies)
            {
                var pawn = pawnFactory.SpawnEnemy(data);
                arrangeInLine.Add(pawn.gameObject);
            }
        }
    }
}