﻿using System;
using Runtime.Combat.Pawn;
using Runtime.Combat.StatusEffects;
using Runtime.Selection;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour.Feedback
{
    [CreateAssetMenu(fileName = "Loop Per stack", menuName = "Card/Strategy/Loop per Status Effect", order = 0)]
    public class LoopPerStatusEffect : CardPlayStrategy
    {
        [SerializeField] private PawnSelectMode _pawnSelectMode;
        [SerializeField] private StatusEffectData _statusEffectData;
        [SerializeField] private CardPlayStrategy _nestedStrategy;

        // public override void Play(PawnController caller, int potency)
        // {
        //     //TODO: Refactor
        //     // var type = _statusEffectData.GetStatusEffectType();
        //     //
        //     // var pawn = _pawnSelectMode switch
        //     // {
        //     //     PawnSelectMode.Target => SelectionService.Instance.TargetedPawn.Controller,
        //     //     PawnSelectMode.Hero => GameManager.Instance.Hero,
        //     //     PawnSelectMode.Caller => caller,
        //     //     _ => caller
        //     // };
        //     //
        //     // var stacks = pawn.GetStatusEffectStacks(_statusEffectData.GetStatusEffectType());
        //     //
        //     // for (int i = 0; i < stacks; i++)
        //     // {
        //     //     _nestedStrategy.Play(caller, potency);
        //     // }
        // }

        public override void Play(PawnController caller, int potency, Action onComplete)
        {
            
        }
    }
}