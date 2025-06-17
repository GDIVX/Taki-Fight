using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.AttackMod;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        private AttackParams _params;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            PawnHelper.SelectPawnsAndInvokeAction(_params, pawn => HandleAttack(pawn, Potency),
                cardController.transform.position, onComplete);
        }

        public override void BlindPlay(CardController cardController, Action<bool> onComplete)
        {
            PawnController pawn = PawnHelper.FindRandomPawn(_params.PawnOwner);
            HandleAttack(pawn, Potency);
        }


        private void HandleAttack(PawnController target, int potency)
        {
            //TODO: Magic power stat
            if (target == null)
            {
                Debug.LogError($"Card '{name}' tried to attack a null target.");
                return;
            }

            if (target.Health.IsDead())
            {
                Debug.LogWarning($"Card '{name}' tried to attack a dead target. Might be expected.");
                return;
            }

            var finalDamage = potency;
            target.Combat.HandleDamage(finalDamage, _params.DamageHandler);
        }

        public override string GetDescription()
        {
            var relation = _params.PawnOwner == PawnOwner.Player ? "Allied Familiar" : "Hostile Familiar";
            var str = _params.TargetsCount > 1
                ? $"Deal {Potency} {_params.DamageHandler.GetDescription()} to {_params.TargetsCount} {relation}s."
                : $"Deal {Potency} {_params.DamageHandler.GetDescription()} to a {relation}.";
            return $"{base.GetDescription()} {str}";
        }

        public override void Initialize(PlayStrategyData playStrategyData, CardController cardController)
        {
            _params = playStrategyData.Parameters as AttackParams;
            base.Initialize(playStrategyData, cardController);
        }

        public class AttackPawnParams : GetPawnsParams
        {
            [SerializeReference] public IDamageHandler DamageType;
        }
    }
}