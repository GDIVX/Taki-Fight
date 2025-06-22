using System;
using Runtime.Combat.Pawn;
using Runtime.CardGameplay.Card;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Knockback Play", menuName = "Card/Strategy/Play/Knockback", order = 0)]
    public class KnockbackTargetCardPlay : CardPlayStrategy
    {
        private const int DamagePerTile = 3;
        private KnockbackParams _params;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            PawnHelper.SelectPawnsAndInvokeAction(_params,
                pawn => HandleKnockback(pawn, Potency),
                cardController.transform.position, onComplete);
        }

        private void HandleKnockback(PawnController target, int potency)
        {
            if (target == null)
            {
                Debug.LogError($"Card '{name}' tried to knockback a null target.");
                return;
            }

            var direction = CalculateDirection();
            PawnHelper.Knockback(target, potency, DamagePerTile, direction);
        }

        private Vector2Int CalculateDirection()
        {
            var forward = _params.PawnOwner == PawnOwner.Player ? Vector2Int.right : Vector2Int.left;
            return _params.Direction switch
            {
                MovementDirection.Forward => forward,
                MovementDirection.Backward => -forward,
                MovementDirection.Up => Vector2Int.up,
                MovementDirection.Down => Vector2Int.down,
                MovementDirection.RandomUpOrDown => Random.value > 0.5f ? Vector2Int.up : Vector2Int.down,
                MovementDirection.RandomForwardOrBackward => Random.value > 0.5f ? forward : -forward,
                MovementDirection.Random => InsideUnitTaxicabCircle(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector2Int InsideUnitTaxicabCircle()
        {
            var x = Random.value * 2 - 1;
            var y = Random.value * 2 - 1;

            var intX = Mathf.FloorToInt(x);
            var intY = Mathf.FloorToInt(y);
            return new Vector2Int(intX, intY);
        }

        public override string GetDescription()
        {
            var relation = _params.PawnOwner == PawnOwner.Player ? "Allied Familiar" : "Hostile Familiar";
            var builder = new DescriptionBuilder();
            builder.WithKeyword("Knockback");
            var directionText = GetDirectionDescription();
            var knockbackText = $"{builder.ToString()} {Potency} tiles {directionText}";

            return _params.TargetsCount > 1
                ? $"{knockbackText} {_params.TargetsCount} {relation}s"
                : $"{knockbackText} a {relation}";
        }

        private string GetDirectionDescription()
        {
            return _params.Direction switch
            {
                MovementDirection.Forward => "forward",
                MovementDirection.Backward => "backward",
                MovementDirection.Up => "up",
                MovementDirection.Down => "down",
                MovementDirection.RandomUpOrDown => "up or down",
                MovementDirection.RandomForwardOrBackward => "forward or backward",
                MovementDirection.Random => "in a random direction",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            _params = playStrategyData.Parameters as KnockbackParams;
            base.Initialize(playStrategyData, cardController);
        }
    }
}
