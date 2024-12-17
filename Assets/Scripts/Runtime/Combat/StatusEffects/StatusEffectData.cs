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

        public TooltipData TooltipData => _tooltipData;
        public StatusEffectType Type => _effectType;
        public Sprite Icon => _icon;

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