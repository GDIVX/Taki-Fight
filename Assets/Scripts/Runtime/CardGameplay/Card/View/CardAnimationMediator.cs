using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.View
{
    public class CardAnimationMediator : MonoService<CardAnimationMediator>
    {
        private readonly Dictionary<CardView, Tween> _activeTweens = new();

        public Tween Animate(CardView view, Vector3 position, Vector3 rotation, float duration, Ease ease)
        {
            if (_activeTweens.TryGetValue(view, out var tween) && tween.IsActive())
            {
                tween.Kill();
            }

            var sequence = DOTween.Sequence()
                .Join(view.transform.DOLocalMove(position, duration).SetEase(ease))
                .Join(view.transform.DOLocalRotate(rotation, duration).SetEase(ease))
                .OnComplete(() => _activeTweens.Remove(view));

            _activeTweens[view] = sequence;
            return sequence;
        }

        public void Stop(CardView view)
        {
            if (_activeTweens.TryGetValue(view, out var tween) && tween.IsActive())
            {
                tween.Kill();
                _activeTweens.Remove(view);
            }
        }
    }
}
