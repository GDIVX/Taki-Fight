using System;
using Runtime.CardGameplay.Card.View;
using Runtime.Combat.Pawn;
using Runtime.UI.Tooltip;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.StatusEffects
{
    [CreateAssetMenu(fileName = "StatusEffectData", menuName = "StatusEffects/StatusEffectData")]
    public class StatusEffectData : ScriptableObject
    {
        [SerializeField, Required] private StatusEffectStrategy _effect;
        [SerializeField, Required] private Sprite _icon;
        [SerializeField, Required] private Keyword _keyword;

        public Sprite Icon => _icon;

        public Keyword Keyword => _keyword;

        public IStatusEffect CreateStatusEffect(int stacks)
        {
            if (_effect.Clone() is not IStatusEffect clone)
            {
                Debug.LogError($"{nameof(StatusEffectStrategy)} is not of type {nameof(IStatusEffect)}");
                return null;
            }

            clone.Stack = new Observable<int>(stacks);
            clone.Keyword = _keyword;
            return clone;
        }
    }

    public abstract class StatusEffectStrategy : ScriptableObject, IStatusEffect
    {
        public abstract Observable<int> Stack { get; set; }
        public Keyword Keyword { get; set; }
        public abstract void OnTurnStart(PawnController pawn);
        public abstract void OnAdded(PawnController pawn);
        public abstract void Remove(PawnController pawn);
        public abstract object Clone();
    }
}