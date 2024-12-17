using Runtime.CardGameplay.Tooltip;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.StatusEffects
{
    public abstract class StatusEffectData : ScriptableObject
    {
        [SerializeField] private TooltipData _tooltipData;
        [SerializeField] private StatusEffectType _effectType;
        [SerializeField] private Sprite _icon;

        public abstract IStatusEffect CreateStatusEffect(int stacks);
    }

    public enum StatusEffectType
    {
        Effect,
        Buff,
        Debuff,
        Trigger
    }
}