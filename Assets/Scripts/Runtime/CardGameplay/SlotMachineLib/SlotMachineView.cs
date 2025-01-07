using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.SlotMachineLib
{
    public class SlotMachineView : MonoBehaviour
    {
        [SerializeField] private Vector3 _hiddenPosition; // We'll compute this in Awake.
        [SerializeField] private float _animTime = 0.5f; // Duration of the slide animation
        [SerializeField] private Ease _ease = Ease.OutCubic; // Easing function

        [SerializeField, ReadOnly] private Vector3 _shownPosition; // We'll compute this in Awake.

        private Tweener _tweener;

        private bool _isShown;

        private void Awake()
        {
            // Cache the "shown" and "hidden" positions.
            // Assumes the UI is initially placed in the "shown" position in the editor.
            _shownPosition = transform.localPosition;

            // Optionally, if you want the UI to *start* hidden,
            // uncomment the following line:
            transform.localPosition = _hiddenPosition;
            _isShown = false;
        }

        /// <summary>
        /// Slides the UI into view from above.
        /// </summary>
        [Button]
        public void Show()
        {
            // Kill any ongoing tween so we don't conflict animations
            _tweener?.Kill();
            _tweener = transform.DOLocalMove(_shownPosition, _animTime)
                .SetEase(_ease);
            _isShown = true;
        }

        /// <summary>
        /// Slides the UI off screen (above).
        /// </summary>
        [Button]
        public void Hide()
        {
            // Kill any ongoing tween so we don't conflict animations
            _tweener?.Kill();
            _tweener = transform.DOLocalMove(_hiddenPosition, _animTime)
                .SetEase(_ease);
            _isShown = false;
        }

        public void Toggle()
        {
            if (_isShown)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// For convenience if you ever need to wait inside a Coroutine
        /// until the current show/hide animation finishes,
        /// you could call "yield return StartCoroutine(WaitForTween())".
        /// </summary>
        public IEnumerator WaitForTween()
        {
            if (_tweener == null) yield break;
            yield return _tweener.WaitForCompletion();
        }
    }
}