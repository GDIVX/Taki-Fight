﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Combat.Pawn;
using Runtime.UI.Tooltip;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.StatusEffects
{
    public class StatusEffectHandler : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private List<IStatusEffect> _statusEffects;
        [SerializeField] private StatusEffectListView _statusEffectView;
        private PawnController _pawn;

        private void OnValidate()
        {
            _statusEffectView ??= GetComponentInChildren<StatusEffectListView>();
            _pawn ??= GetComponentInChildren<PawnController>();
        }

        public void Init(PawnController pawnController)
        {
            _statusEffects = new List<IStatusEffect>();
            _pawn = pawnController;
        }

        public void Add(IStatusEffect newEffect, Sprite icon, TooltipData tooltipData)
        {
            // Check if we already have the same effect
            var existingEffect = _statusEffects.Find(e => e.GetType() == newEffect.GetType());
            if (existingEffect != null)
            {
                // Increase stacks instead of adding a new one
                existingEffect.Stack.Value += newEffect.Stack.Value;
            }
            else
            {
                // Add new effect
                _statusEffects.Add(newEffect);
                newEffect.OnAdded(_pawn);
                _statusEffectView.Add(newEffect, icon, tooltipData);
            }
        }

        public T Get<T>() where T : IStatusEffect
        {
            return (T)_statusEffects.FirstOrDefault(effect => effect is T);
        }

        public IStatusEffect Get(Type type)
        {
            return _statusEffects.FirstOrDefault(type.IsInstanceOfType);
        }

        private void Remove(IStatusEffect effect)
        {
            effect.Remove(_pawn);
            _statusEffects.Remove(effect);
            _statusEffectView.Remove(effect);
        }

        public void OnTurnStart()
        {
            foreach (IStatusEffect statusEffect in _statusEffects)
            {
                statusEffect.OnTurnStart(_pawn);
            }
        }

        public void OnTurnEnd()
        {
            // We copy the list because we might remove effects as we go
            var effectsSnapshot = new List<IStatusEffect>(_statusEffects);

            foreach (var effect in effectsSnapshot.Where(effect => effect.Stack.Value <= 0))
            {
                Remove(effect);
            }
        }

        public void Clear()
        {
            foreach (IStatusEffect effect in _statusEffects)
            {
                Remove(effect);
            }
        }
    }
}