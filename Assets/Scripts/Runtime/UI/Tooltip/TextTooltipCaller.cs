using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Runtime.UI.Tooltip
{
    public class TextTooltipCaller : TooltipCallerBase
    {
        [SerializeField, Required] private KeywordDictionary _keywordDictionary;
        [SerializeField, Required] private TMP_Text _textField;
        private Camera _camera;

        private void OnValidate()
        {
            _textField ??= GetComponent<TMP_Text>();
        }

        protected void Awake()
        {
            _camera = Camera.main;
            if (_textField == null)
            {
                Debug.LogError("TextTooltipCaller requires a TMP_Text component!");
            }

            if (_keywordDictionary == null)
            {
                Debug.LogError("TooltipDictionary is not assigned!");
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(CheckHoveredWord());
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            StopAllCoroutines();
        }

        private System.Collections.IEnumerator CheckHoveredWord()
        {
            while (true)
            {
                int hoveredWordIndex = TMP_TextUtilities.FindIntersectingWord(_textField, Input.mousePosition, null);

                if (hoveredWordIndex != -1)
                {
                    string hoveredWord = _textField.textInfo.wordInfo[hoveredWordIndex].GetWord();
                    TooltipData data = _keywordDictionary.GetTooltip(hoveredWord);

                    RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
                    ShowTooltip(data, transform, canvasRect, BaseOffset); // Adjust offset
                }
                else if (CurrentTooltip)
                {
                    TooltipPool.ReturnTooltip(CurrentTooltip);
                    CurrentTooltip = null;
                }

                yield return null;
            }
        }
    }
}