using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.Tooltip
{
    [ExecuteInEditMode]
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _headerField;
        [SerializeField] private TMP_Text _secondHeaderField;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _background;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private int _characterWrapLimit;


        // --------------------------------------------------------------------
        //  Set up basic text & icon
        // --------------------------------------------------------------------
        public void SetTooltip(string title, string secondHeader = "", string description = "",
            Color backgroundColor = default,
            Sprite icon = null)
        {
            _headerField.text = title;
            _secondHeaderField.text = secondHeader;
            _descriptionText.text = description;
            _background.color = backgroundColor;

            int headerLength = _headerField.text.Length;
            int secondHeaderLength = _secondHeaderField.text.Length;
            int contentLength = _descriptionText.text.Length;
            _layoutElement.enabled = headerLength > _characterWrapLimit
                                     || secondHeaderLength > _characterWrapLimit ||
                                     contentLength > _characterWrapLimit;

            if (icon != null)
            {
                _iconImage.sprite = icon;
                _iconImage.gameObject.SetActive(true);
            }
            else
            {
                _iconImage.gameObject.SetActive(false);
            }
        }

        // --------------------------------------------------------------------
        //  Main entry point for tooltip positioning
        // --------------------------------------------------------------------
        public void PositionTooltip(Transform callerTransform, RectTransform canvasRect, Vector2 staticOffset)
        {
            if (callerTransform == null) return;

            // 1) Grab the mouse’s screen position
            Vector2 mouseScreenPos = Input.mousePosition;

            // 2) Convert that screen position to a local position in the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, mouseScreenPos, null, out Vector2 localMousePos);

            // 3) Apply a basic offset so it’s not directly under the cursor
            //    pivot is top-left, so we offset down-right
            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = localMousePos + staticOffset;

            // 4) Check if tooltip overlaps the caller. If so, nudge it above/below
            RectTransform callerRect = callerTransform.GetComponent<RectTransform>();
            if (callerRect != null && RectOverlaps(rectTransform, callerRect, canvasRect))
            {
                // Move the tooltip up by half the caller + half the tooltip
                float overlapFix = (callerRect.sizeDelta.y + rectTransform.sizeDelta.y) * 0.5f;
                rectTransform.anchoredPosition += new Vector2(0f, overlapFix);
            }

            // 5) Finally ensure we stay within the screen/canvas
            EnsureTooltipIsOnScreen(canvasRect);
        }

        // --------------------------------------------------------------------
        //  Keep the tooltip inside the visible area of the canvas
        // --------------------------------------------------------------------
        private void EnsureTooltipIsOnScreen(RectTransform canvasRect)
        {
            RectTransform rectTransform = (RectTransform)transform;
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            // Convert corners to local space in the canvas
            Vector2 canvasSize = canvasRect.sizeDelta;
            float halfWidth = canvasSize.x * 0.5f;
            float halfHeight = canvasSize.y * 0.5f;

            // We’ll track how much we need to move
            Vector2 adjustment = Vector2.zero;

            for (int i = 0; i < 4; i++)
            {
                // Corner in canvas local coords
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, corners[i], null, out Vector2 cornerLocal);

                // Compare cornerLocal to boundaries
                if (cornerLocal.x < -halfWidth)
                    adjustment.x = Mathf.Max(adjustment.x, -halfWidth - cornerLocal.x);
                else if (cornerLocal.x > halfWidth)
                    adjustment.x = Mathf.Min(adjustment.x, halfWidth - cornerLocal.x);

                if (cornerLocal.y > halfHeight)
                    adjustment.y = Mathf.Min(adjustment.y, halfHeight - cornerLocal.y);
                else if (cornerLocal.y < -halfHeight)
                    adjustment.y = Mathf.Max(adjustment.y, -halfHeight - cornerLocal.y);
            }

            // Apply final shift
            rectTransform.anchoredPosition += adjustment;
        }

        // --------------------------------------------------------------------
        //  Checks if two RectTransforms overlap in the local space of the canvas
        // --------------------------------------------------------------------
        private bool RectOverlaps(RectTransform rectA, RectTransform rectB, RectTransform canvasRect)
        {
            // Get A corners in canvas space
            Vector3[] aCorners = new Vector3[4];
            rectA.GetWorldCorners(aCorners);
            Vector2 aMin = Vector2.positiveInfinity;
            Vector2 aMax = Vector2.negativeInfinity;

            for (int i = 0; i < 4; i++)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, aCorners[i], null, out Vector2 lc);
                aMin = Vector2.Min(aMin, lc);
                aMax = Vector2.Max(aMax, lc);
            }

            // Get B corners in canvas space
            Vector3[] bCorners = new Vector3[4];
            rectB.GetWorldCorners(bCorners);
            Vector2 bMin = Vector2.positiveInfinity;
            Vector2 bMax = Vector2.negativeInfinity;

            for (int i = 0; i < 4; i++)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, bCorners[i], null, out Vector2 lc);
                bMin = Vector2.Min(bMin, lc);
                bMax = Vector2.Max(bMax, lc);
            }

            // Check for overlap in local coords
            bool separateX = aMax.x < bMin.x || aMin.x > bMax.x;
            bool separateY = aMax.y < bMin.y || aMin.y > bMax.y;
            return !(separateX || separateY);
        }

        // --------------------------------------------------------------------
        //  Show/Hide tooltip with DOFade
        // --------------------------------------------------------------------
        public void ShowTooltip()
        {
            gameObject.SetActive(true); // turn on first
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------
        //  Reset tooltip UI
        // --------------------------------------------------------------------
        public void Reset()
        {
            _headerField.text = string.Empty;
            _descriptionText.text = string.Empty;
            _iconImage.sprite = null;
            _iconImage.gameObject.SetActive(false);
        }
    }
}