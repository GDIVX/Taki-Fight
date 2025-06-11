using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Combat.Tilemap;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn
{
    [Serializable]
    internal class PawnCombat
    {
        public Observable<int> Defense;
        public Observable<int> Damage;
        public Observable<int> Attacks;

        public PawnCombat(PawnController pawn, PawnData data)
        {
            Pawn = pawn;
            Defense = new Observable<int>(data.Defense);
            Damage = new Observable<int>(data.Damage);
            Attacks = new Observable<int>(data.Attacks);
        }

        [ShowInInspector] [ReadOnly] public int AttackRange { get; set; }

        public PawnController Pawn { get; private set; }

        public event Action<int, int> OnBeingAttacked;

        public void ReceiveAttack(int damage)
        {
            if (damage <= 0)
            {
                OnBeingAttacked?.Invoke(damage, 0);
                return;
            }

            int finalDamage = Mathf.Max(0, damage - Defense.Value);
            Pawn.Health.Damage(finalDamage);
            Defense.Value = Mathf.Max(0, Defense.Value - damage);

            OnBeingAttacked?.Invoke(damage, finalDamage);
            Pawn.ExecuteStrategies(Pawn.Data.OnDamagedStrategies);
        }


        public IEnumerator Attack(PawnController target, Action onComplete)
        {
            for (int i = 0; i < Attacks.Value; i++)
            {
                var feedbackDone = false;
                Pawn.ExecuteAttackFeedbackStrategy(target, () => feedbackDone = true);
                yield return new WaitUntil(() => feedbackDone);

                int attackDamage = Damage.Value;
                Pawn.ExecuteHitStrategies(Pawn.Data.OnHitStrategies, target, ref attackDamage);
                target.Combat.ReceiveAttack(attackDamage);
                yield return null;
            }

            onComplete?.Invoke();
            Pawn.ExecuteStrategies(Pawn.Data.OnAttackStrategies);
        }

        public List<Tile> GetTilesInAttackRange()
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            var tileSet = new HashSet<Tile>();
            var edgeTiles = Pawn.TilemapHelper.GetEdges();

            foreach (var tile in edgeTiles)
            {
                if (tile == null) continue;
                var tilesInRange = tilemap.GetTilesInRange(tile, AttackRange, true);
                tileSet.UnionWith(tilesInRange);
            }

            return new List<Tile>(tileSet);
        }

        private List<PawnController> GetTargets()
        {
            var targets = new HashSet<PawnController>();
            var tilesInRange = GetTilesInAttackRange();
            AddPawnsToTargetList(tilesInRange, targets);
            return new List<PawnController>(targets);
        }

        private void AddPawnsToTargetList(IEnumerable<Tile> tiles, HashSet<PawnController> targets)
        {
            foreach (var tile in tiles)
            {
                var pawnAtTile = tile.Pawn;
                if (pawnAtTile && pawnAtTile.Owner != Pawn.Owner) targets.Add(pawnAtTile);
            }
        }

        public PawnController ChooseTarget()
        {
            // Gather all possible targets that are within this pawn’s reach
            var targets = GetTargets();
            if (targets == null || targets.Count == 0) return null;

            var isFriendly = Pawn.Owner == PawnOwner.Player; // Friendly = player-controlled
            var pawnPos = Pawn.transform.position;

            // Try to find a target to the preferred side
            PawnController preferredTarget = null;
            var bestDistance = float.MaxValue;

            foreach (var target in targets)
            {
                var deltaX = target.transform.position.x - pawnPos.x;

                // If we are friendly, look for targets to the right (positive X);
                // otherwise look for targets to the left (negative X)
                var isOnPreferredSide = isFriendly ? deltaX > 0f : deltaX < 0f;
                if (!isOnPreferredSide) continue;

                var distance = Mathf.Abs(deltaX);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    preferredTarget = target;
                }
            }

            return preferredTarget
                ? preferredTarget
                : // Found something on the preferred side
                // Nothing found on the preferred side – choose a random target from the list
                targets.SelectRandom();
        }
    }
}