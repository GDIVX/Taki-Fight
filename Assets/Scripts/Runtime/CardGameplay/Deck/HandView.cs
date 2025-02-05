using System.Collections.Generic;
using DG.Tweening;
using Runtime.CardGameplay.Card;
using Runtime.Combat.Pawn.Targeting;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Deck
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HandView : HorizontalCardListView
    {
        [SerializeField, Required, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, TabGroup("Modal")] private float _fadeDuration;
        [SerializeField, TabGroup("Modal")] private Ease _fadeEase;
        [SerializeField] private CanvasGroup _canvasGroup;


        private void Awake()
        {
            // Subscribe to events for card addition and removal if needed
            _handController.OnCardAdded += AddCard;
            _handController.OnCardRemoved += RemoveCard;

            PawnTargetingService.Instance.OnLookingForTarget +=
                Hide;
            PawnTargetingService.Instance.OnTargetFound += _ => { Show(); };

            GameManager.Instance.CombatManager.OnStartTurn += Show;
            GameManager.Instance.CombatManager.OnEndTurn += Hide;

            _canvasGroup ??= GetComponent<CanvasGroup>();
        }

        [Button]
        private void Show()
        {
            _canvasGroup.DOFade(1, _fadeDuration).SetEase(_fadeEase);
            _canvasGroup.interactable = true;
        }

        [Button]
        private void Hide()
        {
            _canvasGroup.DOFade(0, _fadeDuration).SetEase(_fadeEase);
            _canvasGroup.interactable = false;
        }
    }
}