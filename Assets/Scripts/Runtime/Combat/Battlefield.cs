using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField] private List<CombatLane> _combatLanes;

        public PawnController Hero { get; set; }
        public PawnController Boss { get; set; }

        public List<CombatLane> CombatLanes => _combatLanes;

        public LaneSide GetLaneSide(bool isAlly, int index)
        {
            if (index < 0 || index >= _combatLanes.Count)
            {
                Debug.LogError($"Index out of range.\n" +
                               $" Index given:{index} \n" +
                               $" Allowed range is between 0 to {_combatLanes.Count - 1}\n" +
                               $"Returning first lane");
                index = 0;
            }

            var lane = _combatLanes[index];
            return isAlly ? lane.AllySide : lane.EnemySide;
        }

        public void Clear(bool includeHero)
        {
            _combatLanes.ForEach(lane => lane.Clear());
            // Boss.gameObject.SetActive(false);
            // Hero.gameObject.SetActive(includeHero);
        }

        public void PlayTurn(Action onComplete)
        {
            StartCoroutine(HandleBattlefieldTurnCoroutine(onComplete));
        }

        private IEnumerator HandleBattlefieldTurnCoroutine(Action onComplete)
        {
            foreach (var lane in _combatLanes)
            {
                bool isTurnComplete = false;

                lane.StartTurn(() => isTurnComplete = true);

                yield return new WaitUntil(() => isTurnComplete);
            }

            onComplete?.Invoke();
        }
    }
}