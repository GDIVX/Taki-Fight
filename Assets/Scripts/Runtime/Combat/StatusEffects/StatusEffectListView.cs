using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Combat.StatusEffects
{
    [RequireComponent(typeof(LayoutGroup))]
    public class StatusEffectListView : MonoBehaviour
    {
        [SerializeField] private StatusEffectView _prefab;
        private readonly Dictionary<IStatusEffect, StatusEffectView> _statusEffectViews = new();

        public void Add(IStatusEffect effect, Sprite icon)
        {
            var view = Instantiate(_prefab, transform);
            _statusEffectViews[effect] = view;
            view.Init(effect, icon);
        }

        public void Remove(IStatusEffect effect)
        {
            var view = _statusEffectViews[effect];
            _statusEffectViews.Remove(effect);
            Destroy(view.gameObject);
        }
    }
}