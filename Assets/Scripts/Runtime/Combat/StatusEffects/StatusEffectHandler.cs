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

        public void Add(IStatusEffect effect, Sprite icon)
        {
            _statusEffects.Add(effect);
            effect.OnAdded(_pawn);
            _statusEffectView.Add(effect, icon);
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
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                var effect = _statusEffects[i];
                effect.OnTurnStart(_pawn);

                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator HandleTurnEnd()
        {
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                var effect = _statusEffects[i];
                effect.Stack.Value--;

                if (effect.Stack.Value <= 0)
                {
                    Remove(effect);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}