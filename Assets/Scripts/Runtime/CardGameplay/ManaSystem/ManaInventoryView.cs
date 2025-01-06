using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.ManaSystem
{
    public class ManaInventoryView : MonoBehaviour
    {
        [SerializeField] private SymbolListView _listView;
        [SerializeField] private RectTransform _container;
        [SerializeField] private float _scalePerImage;
        [SerializeField] private float _scaleAnimationTime;
        [SerializeField] private Ease _scaleAnimationEase;
        [SerializeField] private TextMeshProUGUI _capacityCountField;
        [SerializeField] private ManaInventory _inventory;

        private void Start()
        {
            _inventory.OnCapacityModifiedEvent += OnCapacityModified;
            _inventory.OnInventoryModifiedEvent += OnInventoryModified;

            OnCapacityModified(_inventory.Capacity);
            OnInventoryModified(_inventory.Inventory);
        }

        private void OnInventoryModified(List<Mana> content)
        {
            _listView.Draw(content);
            var totalSize = _scalePerImage * content.Count;

            _container.DOScaleX(totalSize, _scaleAnimationTime).SetEase(_scaleAnimationEase);
        }

        private void OnCapacityModified(int newValue)
        {
            _capacityCountField.text = newValue.ToString();
        }
    }
}