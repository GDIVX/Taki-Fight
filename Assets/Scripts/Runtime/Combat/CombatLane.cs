using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat
{
    public class CombatLane : MonoBehaviour
    {
        [SerializeField] private LaneSide _allySide, _enemySide;
        public LaneSide AllySide => _allySide;
        public LaneSide EnemySide => _enemySide;


        private void Start()
        {
            _allySide.Init(this, true);
            _enemySide.Init(this, false);
        }

        public void Clear()
        {
            AllySide.Clear();
            EnemySide.Clear();
        }

        public void StartTurn(Action onComplete)
        {
            StartCoroutine(HandleTurnCoroutine(onComplete));
        }

        private IEnumerator HandleTurnCoroutine(Action onComplete)
        {
            var agileAllyPawns = _allySide.Pawns.Where(pawn => pawn.IsAgile).ToList();
            var nonAgileAllies = _allySide.Pawns.Except(agileAllyPawns).ToList();
            var enemies = _enemySide.Pawns;

            // Agile allies
            foreach (var pawn in agileAllyPawns)
            {
                yield return PlayPawn(pawn);
            }

            // Enemies
            foreach (var pawn in enemies)
            {
                yield return PlayPawn(pawn);
            }

            // Non-agile allies
            foreach (var pawn in nonAgileAllies)
            {
                yield return PlayPawn(pawn);
            }

            onComplete?.Invoke();
        }

        private static IEnumerator PlayPawn(PawnController pawn)
        {
            Debug.Log($"Playing {pawn}");
            pawn.OnTurn();
            return new WaitUntil(() => !pawn.IsProcessingTurn);
        }
    }
}