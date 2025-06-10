using System;
using Runtime.UI.Tooltip;
using UnityEngine;

namespace Runtime.Combat.StatusEffects
{
    public abstract class StatusEffectData : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private TooltipData _tooltip;

        public Sprite Icon => _icon;

        public TooltipData Tooltip => _tooltip;

        public abstract IStatusEffect CreateStatusEffect(int stacks);

        public abstract Type GetStatusEffectType();
    }

}
