﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    public class IntentionsList : MonoBehaviour
    {
        [SerializeField] private Intention _prefab;

        [ShowInInspector, ReadOnly] private Queue<Intention> _shownIntentions = new();
        [ShowInInspector, ReadOnly] private Queue<Intention> _hiddenIntentions = new();

        public void Add(IntentionType intentionType, int potency, int repeats)
        {
            Intention intention = GetIntention();
            _shownIntentions.Enqueue(intention);
            intention.ShowIntention(intentionType, potency, repeats);
        }

        public void RemoveNext()
        {
            var intention = _shownIntentions.Dequeue();
            intention.Hide();
            _hiddenIntentions.Enqueue(intention);
        }

        private Intention GetIntention()
        {
            return _hiddenIntentions.Count == 0
                ? Instantiate(_prefab, transform)
                : _hiddenIntentions.Dequeue();
        }

        private void OnDestroy()
        {
            _shownIntentions.Clear();
            _hiddenIntentions.Clear();
        }
    }

    public enum IntentionType
    {
        Attack,
        Defense,
        Buff,
        Debuff,
        Unknown
    }
}