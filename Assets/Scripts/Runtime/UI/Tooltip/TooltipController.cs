using UnityEngine;
using Sirenix.OdinInspector;

namespace Runtime.UI.Tooltip
{
    public interface ITooltipController
    {
        void SetTooltip(ITooltipSource source);
        void ShowTooltip();
        void HideTooltip();
        void Reset();
    }

    [RequireComponent(typeof(RectTransform))]
    public abstract class TooltipController : MonoBehaviour, ITooltipController
    {
        [SerializeField] private Vector2 _pointerOffset = new Vector2(5f, -5f);

        public abstract void SetTooltip(ITooltipSource source);
        public abstract void ShowTooltip();
        public abstract void HideTooltip();

        public abstract void Reset();

        [SerializeField, Required] private RectTransform _rectTransform;

        // private void OnValidate()
        // {
        //     _rectTransform ??= GetComponent<RectTransform>();
        // }

        protected virtual void CalculatePosition()
        {
            Vector2 mousePosition = Input.mousePosition;
            float pivotX = mousePosition.x / Screen.width;
            float pivotY = mousePosition.y / Screen.height;


            _rectTransform ??= GetComponent<RectTransform>();
            _rectTransform.pivot = new Vector2(pivotX, pivotY);

            // Apply offset
            Vector2 newPosition = mousePosition + _pointerOffset;

            // Determine tooltip’s scaled size
            float scaledWidth = _rectTransform.rect.width * _rectTransform.lossyScale.x;
            float scaledHeight = _rectTransform.rect.height * _rectTransform.lossyScale.y;

            // Clamp so we stay fully on-screen
            newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width - scaledWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, scaledHeight, Screen.height);

            transform.position = newPosition;
        }
    }
}