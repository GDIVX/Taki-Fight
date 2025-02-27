using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.StatusEffects;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Gain Effect Play Strategy", menuName = "Card/Strategy/Play/Gain Effect", order = 0)]
    public class GainEffectPlayStrategy : CardPlayStrategy
    {
        [SerializeField] private StatusEffectData _effectData;

        public override void Play(PawnController caller, int potency, Action onComplete)
        {
            caller.ApplyStatusEffect(_effectData, potency);
            onComplete?.Invoke();
        }
    }
}