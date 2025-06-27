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

            tile.Pawn.Combat.HandleDamage(Potency, _params.DamageHandler);
        }

        public override void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            _params = playStrategyData.Parameters as AOEDamageParams;
            base.Initialize(playStrategyData, cardController);
        }

        public override string GetDescription()
        {
            var builder = new DescriptionBuilder();
            return builder.WithLine("Deal ").AppendBold(Potency.ToString())
                .Append($" {_params.DamageHandler.GetDescription()} in a ").StartBlueHighlight()
                .Append($"{_params.AreaSize.x}x{_params.AreaSize.y} area").EndHighlight().ToString();
        }
        
    }
}