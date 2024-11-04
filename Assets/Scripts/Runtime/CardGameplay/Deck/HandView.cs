using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Card;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class HandView : HorizontalCardListView
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        private void Awake()
        {
            // Subscribe to events for card addition and removal if needed
            _handController.OnCardAdded += OnCardAdded;
            _handController.OnCardRemoved += OnCardRemoved;
        }
    }
}