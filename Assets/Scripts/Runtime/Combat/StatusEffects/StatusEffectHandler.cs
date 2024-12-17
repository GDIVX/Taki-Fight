using System.Collections;
using System.Collections.Generic;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.StatusEffects
{
    public class StatusEffectHandler : MonoBehaviour
    {
        private List<IStatusEffect> _statusEffects;

        private PawnController _pawn;

        public void Init(PawnController pawnController)
        {
            _pawn = pawnController;
            _statusEffects = new List<IStatusEffect>();
        }

        public void Add(IStatusEffect effect)
        {
            _statusEffects.Add(effect);
            effect.OnAdded(_pawn);
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
                effect.Apply(_pawn);

                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator HandleTurnEnd()
        {
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                var effect = _statusEffects[i];
                effect.Stack--;

                if (effect.Stack <= 0)
                {
                    effect.Remove(_pawn);
                    _statusEffects.RemoveAt(i);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}