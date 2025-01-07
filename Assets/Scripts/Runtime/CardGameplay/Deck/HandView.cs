using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn.Targeting;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    public class HandView : HorizontalCardListView
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, TabGroup("Modal")] private Vector3 _moveToWhenHiding;
        [SerializeField, TabGroup("Modal")] private float _hideMovementDuration;
        [SerializeField, TabGroup("Modal")] private Ease _ease;

        private Vector3 _originalPosition;

        private void Awake()
        {
            // Subscribe to events for card addition and removal if needed
            _handController.OnCardAdded += AddCard;
            _handController.OnCardRemoved += RemoveCard;

            PawnTargetingService.Instance.OnLookingForTarget +=
                Hide;
            PawnTargetingService.Instance.OnTargetFound += _ => { Show(); };
        }

        [Button]
        private void Show()
        {
            transform.DOLocalMove(_originalPosition, _hideMovementDuration).SetEase(_ease);
        }

        [Button]
        private void Hide()
        {
            _originalPosition = transform.localPosition;
            transform.DOLocalMove(_moveToWhenHiding, _hideMovementDuration).SetEase(_ease);
        }
    }
}