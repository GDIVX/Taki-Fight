using UnityEngine;

namespace Runtime.Combat.StatusEffects
{
    public abstract class StatusEffectData : ScriptableObject
    {
        [SerializeField] private StatusEffectType _effectType;
        [SerializeField] private Sprite _icon;

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