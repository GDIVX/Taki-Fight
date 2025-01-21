using System;
using DG.Tweening;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour.Feedback
{
    [CreateAssetMenu(fileName = "Shake Feedback", menuName = "Card/Strategy/Feedback/Shake", order = 0)]
    public class ShakeFeedback : FeedbackStrategy
    {
        [SerializeField] private float _animationTime;
        [SerializeField] private Vector3 _strength;
        [SerializeField] private int _vibrato;
        [SerializeField] private Ease _ease;

        public override void Animate(PawnController caller, Action onComplete)
        {
            caller.transform.DOShakePosition(_animationTime, _strength, _vibrato).SetEase(_ease)
                .SetLoops(3, LoopType.Incremental).onComplete += () => onComplete?.Invoke();
        }
    }
}