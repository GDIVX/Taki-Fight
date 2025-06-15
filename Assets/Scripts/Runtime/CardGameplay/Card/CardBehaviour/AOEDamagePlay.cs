using System;
using Runtime.Combat.Tilemap;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "AOE Damage Play", menuName = "Card/Strategy/Play/AOE Damage", order = 0)]
    public class AOEDamagePlay : AOECardPlayStrategy
    {
        private AOEDamageParams _params;

        protected override void ApplyEffect(Tile tile)
        {
            if (tile.Pawn == null)
            {
                return;
            }

            tile.Pawn.Combat.HandleDamage(_params.Damage, _params.DamageHandler);
        }

        public override void Initialize(PlayStrategyData playStrategyData)
        {
            _params = playStrategyData.Parameters as AOEDamageParams;
            base.Initialize(playStrategyData);
        }

        public override string GetDescription()
        {
            return $"Deal {_params.Damage} {_params.DamageHandler.GetDescription()} in a {_params.AreaSize.x}x{_params.AreaSize.y} area";
        }
    }
}

