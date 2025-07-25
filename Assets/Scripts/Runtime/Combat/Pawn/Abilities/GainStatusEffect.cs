﻿using System;
using Runtime.CardGameplay.Card;
using UnityEngine;

namespace Runtime.Combat.Pawn.Abilities
{
    [CreateAssetMenu(fileName = "Gain Status Effect", menuName = "Pawns/Abilities/Gain Status Effect", order = 0)]
    public class GainStatusEffect : PawnPlayStrategy
    {
        private StatusEffectParams _params;

        public override void Play(PawnController pawn, Action<bool> onComplete)
        {
            var statusEffect = _params.StatusEffect;
            if (!statusEffect)
            {
                Debug.LogError("Status effect is null.");
                onComplete(false);
                return;
            }

            pawn.ApplyStatusEffect(_params.StatusEffect, Potency);
        }

        public override void Initialize(PawnStrategyData data)
        {
            _params = data.Parameters as StatusEffectParams;
            //validate params
            if (_params == null)
            {
                Debug.LogError("GainStatusEffectParams is null.");
                return;
            }

            if (!_params.StatusEffect)
            {
                Debug.LogError("Status effect is null.");
                return;
            }

            base.Initialize(data);
        }

        public override string GetDescription()
        {
            var keyword = _params.StatusEffect.Keyword;
            if (keyword == null)
            {
                return $"Gain a status effect with a stack of {Potency}. *Error: missing keyword.*";
            }

            var builder = new DescriptionBuilder();
            return builder.WithLine("Gain ").WithKeyword(keyword, false).Append($" {Potency}").ToString();
        }
    }
}