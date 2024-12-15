using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.Tooltip
{
    public class CardTooltipSystem : MonoBehaviour
    {
        [SerializeField] private CardTooltipView _prefab;
        [SerializeField] private int _maxTooltipsCount;
        [ShowInInspector, ReadOnly] private Stack<CardTooltipView> _tooltipsPool = new();
        [ShowInInspector, ReadOnly] private Stack<CardTooltipView> _activeTooltips = new();

        [SerializeField] private LayoutGroup _layoutGroup;

        private void Start()
        {
            for (int i = 0; i < _maxTooltipsCount; i++)
            {
                var view = Instantiate(_prefab, transform);
                _tooltipsPool.Push(view);
                view.gameObject.SetActive(true);
                view.Hide();
            }

        }

        public void DrawTooltips(List<CardTooltipData> tooltips)
        {
            foreach (var tooltipData in tooltips)
            {
                if (_activeTooltips.Count >= _maxTooltipsCount) return;

                DrawTooltip(tooltipData);
                if (tooltipData.AttachedTooltips.Count > 0)
                {
                    DrawTooltips(tooltipData.AttachedTooltips);
                }
            }
        }


        private void DrawTooltip(CardTooltipData tooltipData)
        {
            CardTooltipView view = GetTooltipView();
            _activeTooltips.Push(view);
            view.Draw(tooltipData);
        }


        private CardTooltipView GetTooltipView()
        {
            return _tooltipsPool.Count > 0 ? _tooltipsPool.Pop() : Instantiate(_prefab, transform);
        }

        public void HideAllTooltips()
        {
            while (_activeTooltips.Count > 0)
            {
                var view = _activeTooltips.Pop();
                _tooltipsPool.Push(view);
                view.Hide();
            }
        }
    }
}