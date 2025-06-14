using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Attack", order = 0)]
    public class AttackTargetCardPlay : CardPlayStrategy
    {
        private GetPawnsParams _params;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            PawnHelper.SelectPawnsAndInvokeAction(_params, pawn => HandleAttack(pawn, Potency),
                cardController.transform.position, onComplete);
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
            target.Combat.ReceiveAttack(finalDamage);
        }

        public override string GetDescription()
        {
            return _params.TargetsCount > 1
                ? $"Deal {Potency} damage to {_params.TargetsCount} targets."
                : $"Deal {Potency} damage.";
        }

        public override void Initialize(PlayStrategyData playStrategyData)
        {
            _params = playStrategyData.Parameters as GetPawnsParams;
            base.Initialize(playStrategyData);
        }
    }
}