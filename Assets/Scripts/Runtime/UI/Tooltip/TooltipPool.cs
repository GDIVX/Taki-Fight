using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Utilities;
using Debug = UnityEngine.Debug;

namespace Runtime.UI.Tooltip
{
    public class TooltipPool : MonoService<TooltipPool>
    {
        [SerializeField] private int _initialSize = 5;
        [SerializeField, Min(1)] private int _minimalSize = 1;

        private readonly Dictionary<Type, Queue<TooltipController>> _tooltips = new();
        private readonly Dictionary<Type, TooltipController> _prefabs = new();

        public void Populate<T>(TooltipController prefab) where T : ITooltipSource
        {
            var type = typeof(T);

            if (!_tooltips.ContainsKey(type))
            {
                _tooltips[type] = new Queue<TooltipController>();
                _prefabs[type] = prefab;
            }

            while (_tooltips[type].Count < _initialSize)
                CreateNewTooltip<T>();
        }

        public TooltipController GetTooltip<T>() where T : ITooltipSource
        {
            var type = typeof(T);

            if (!_tooltips.ContainsKey(type))
            {
                Debug.LogError($"[{type.Name}] not registered.");
                return null;
            }

            // DumpPool<T>("Before Dequeue");

            var pool = _tooltips[type];
            if (pool.Count < _minimalSize)
            {
                Debug.Log($"[{type.Name}] below minimal ({pool.Count} < {_minimalSize}), creating new.");
                var fresh = CreateNewTooltip<T>(true);
                // DumpPool<T>("After Create");
                return fresh;
            }

            var instance = pool.Dequeue();
            // Debug.Log($"[{type.Name}] Dequeued instance: {instance.name}");
            // DumpPool<T>("After Dequeue");
            return instance;
        }

        public void ReturnTooltip<T>(TooltipController instance) where T : ITooltipSource
        {
            var type = typeof(T);

            // DumpPool<T>("Before Return");
            instance.HideTooltip();
            AddToPool<T>(instance);
            Debug.Log($"[{type.Name}] Returned instance: {instance.name}");
            // DumpPool<T>("After Return");
        }

        private TooltipController CreateNewTooltip<T>(bool registerToPool = true) where T : ITooltipSource
        {
            var type = typeof(T);

            if (!_prefabs.TryGetValue(type, out var prefab) || prefab == null)
            {
                Debug.LogError($"[{type.Name}] prefab not found.");
                return null;
            }

            var instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);

            if (registerToPool)
                AddToPool<T>(instance);

            Debug.Log($"[{type.Name}] Created new instance: {instance.name}");
            return instance;
        }

        private void AddToPool<T>(TooltipController instance) where T : ITooltipSource
        {
            var type = typeof(T);

            if (!_tooltips.TryGetValue(type, out var pool))
            {
                pool = new Queue<TooltipController>();
                _tooltips[type] = pool;
            }

            pool.Enqueue(instance);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        private void DumpPool<T>(string s) where T : ITooltipSource
        {
            var type = typeof(T);
            Debug.Log(_tooltips.TryGetValue(type, out var q)
                ? $"[{type.Name}] {s}: pool count = {q.Count}"
                : $"[{type.Name}] {s}: no pool");
        }
    }
}