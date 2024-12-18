using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.StatusEffects
{
    public class StatusEffectHandler : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private List<IStatusEffect> _statusEffects;
        [SerializeField] private StatusEffectListView _statusEffectView;
        [SerializeField] private PawnController _pawn;

        private void OnValidate()
        {
            _statusEffectView ??= GetComponentInChildren<StatusEffectListView>();
            _pawn ??= GetComponentInChildren<PawnController>();
        }

        private void Start()
        {
            _statusEffects = new List<IStatusEffect>();
        }

        public void Add(IStatusEffect newEffect, Sprite icon)
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
                _statusEffectView.Add(newEffect, icon);
            }
        }

        private void Remove(IStatusEffect effect)
        {
            effect.Remove(_pawn);
            _statusEffects.Remove(effect);
            _statusEffectView.Remove(effect);
        }

        public void OnTurnStart()
        {
            StartCoroutine(HandleTurnStart());
        }

        public void OnTurnEnd()
        {
            StartCoroutine(HandleTurnEnd());
        }

        private IEnumerator HandleTurnStart()
        {
            foreach (IStatusEffect statusEffect in _statusEffects)
            {
                statusEffect.OnTurnStart(_pawn);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator HandleTurnEnd()
        {
            // We copy the list because we might remove effects as we go
            var effectsSnapshot = new List<IStatusEffect>(_statusEffects);

            foreach (var effect in effectsSnapshot)
            {
                effect.Stack.Value--;

                if (effect.Stack.Value <= 0)
                {
                    Remove(effect);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}