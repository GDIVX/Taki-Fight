using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    /// <summary>
    ///     Allow cards to invoke a pawn ability on a selected pawn.
    /// </summary>
    [CreateAssetMenu(fileName = "Invoke Ability", menuName = "Card/Strategy/Play/Invoke Pawn Ability", order = 0)]
    public class InvokePawnAbilityPlayStrategy : CardPlayStrategy
    {
        private InvokePawnAbilityPlayStrategyParams _params;

        public override void Play(CardController cardController, Action<bool> onComplete)
        {
            PawnHelper.SelectPawnsAndInvokeAction(_params.PawnOwner, _params.TargetsCount,
                InvokeAbility, cardController.transform.position,
                onComplete);
        }

        private void InvokeAbility(PawnController pawn, Action<bool> onComplete)
        {
            _params.PawnStrategyData.Strategy.Play(pawn, onComplete);
        }

        public override void Initialize(PlayStrategyData playStrategyData)
        {
            base.Initialize(playStrategyData);
            _params = playStrategyData.Parameters as InvokePawnAbilityPlayStrategyParams;
            if (_params != null) _params.PawnStrategyData.Strategy.Initialize(_params.PawnStrategyData);
        }
    }

    [Serializable]
    public class InvokePawnAbilityPlayStrategyParams : StrategyParams
    {
        [SerializeReference] public PawnStrategyData PawnStrategyData;
        public int TargetsCount;
        public PawnOwner PawnOwner;
    }
}