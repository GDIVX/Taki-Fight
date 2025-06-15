using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI.Tooltip
{
    public class TextTooltipCaller : TooltipCallerBase
    {
        [SerializeField, Required] private TMP_Text _textField;
        private Camera _camera;

        protected void Awake()
        {
            _camera = Camera.main;
            if (_textField == null)
            {
                Debug.LogError("TextTooltipCaller requires a TMP_Text component!");
            }
        }

        private void OnValidate()
        {
            _textField ??= GetComponent<TMP_Text>();
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

        private IEnumerator CheckHoveredWord()
        {
            while (true)
            {
                int hoveredWordIndex = TMP_TextUtilities.FindIntersectingWord(_textField, Input.mousePosition, null);

                if (hoveredWordIndex != -1)
                {
                    string hoveredWord = _textField.textInfo.wordInfo[hoveredWordIndex].GetWord();
                    var data = KeywordDictionary.GetFormated(hoveredWord);

                    ShowTooltip(data);
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