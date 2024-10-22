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
        private void Awake()
        {
            // Subscribe to events for card addition and removal if needed
            HandController.Instance.OnCardAdded += OnCardAdded;
            HandController.Instance.OnCardRemoved += OnCardRemoved;
        }
    }
}