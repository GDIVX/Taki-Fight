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
            PawnHelper.SelectPawnsAndInvokeAction(_params, pawn =>
                {
                    if (!pawn)
                    {
                        onComplete?.Invoke(false);
                        return;
                    }

                    HandleAttack(pawn, Potency);
                },
                cardController.transform.position, onComplete);
        }

        public override void BlindPlay(CardController cardController, Action<bool> onComplete)
        {
            PawnController pawn = PawnHelper.FindRandomPawn(_params.PawnOwner);
            if (!pawn)
            {
                onComplete?.Invoke(false);
                return;
            }

            HandleAttack(pawn, Potency);
        }


        private void HandleAttack(PawnController target, int potency)
        {
            if (!target)
            {
                Debug.LogWarning($"Card '{name}' tried to attack a null target.");
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
            var builder = new DescriptionBuilder();

            if (_params.TargetsCount == 1)
            {
                return builder.WithLine("Deal ").AppendBold(Potency.ToString()).WithSpace()
                    .Append(_params.DamageHandler).Append(" to ").WithRelations(_params.PawnOwner, true).ToString();
            }

            return builder.WithLine("Deal ").AppendBold(Potency.ToString()).WithSpace().Append(_params.DamageHandler)
                .Append(" to ").AppendBold(_params.TargetsCount.ToString()).WithRelations(_params.PawnOwner, false)
                .ToString();
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