using System;
using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Charge Ability", menuName = "Pawns/Abilities/Combat/Charge", order = 0)]
    public class ChargeAbility : PawnPlayStrategy
    {
        private ChargeAbilityParams _params;

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            var tilemap = ServiceLocator.Get<TilemapController>();
            if (tilemap == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            var forward = pawn.Owner == PawnOwner.Player ? Vector2Int.right : Vector2Int.left;
            var currentTile = pawn.TilemapHelper.AnchorTile;
            var steps = _params.MovementDistance;

            for (var i = 0; i < steps; i++)
            {
                var nextPos = currentTile.Position + forward;
                var nextTile = tilemap.GetTile(nextPos);
                if (nextTile == null)
                {
                    break;
                }

                if (nextTile.IsOccupied)
                {
                    PawnHelper.Knockback(nextTile.Pawn, _params.KnockbackStrength, 0, forward);
                }

                if (nextTile.IsOccupied)
                {
                    break;
                }

                currentTile = nextTile;
            }

            if (currentTile != pawn.TilemapHelper.AnchorTile)
            {
                pawn.MoveToPosition(currentTile, () => onComplete?.Invoke(true));
            }
            else
            {
                onComplete?.Invoke(false);
            }
        }

        public override string GetDescription()
        {
            return $"Charge {_params.MovementDistance} and knockback {_params.KnockbackStrength}";
        }

        public override void Initialize(PawnStrategyData data)
        {
            _params = data.Parameters as ChargeAbilityParams;
            base.Initialize(data);
        }
    }
}
