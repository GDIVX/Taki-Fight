using System;
using System.Collections;
using Runtime.CardGameplay.Card.View;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI.Tooltip
{
    public class TextTooltipCaller : TooltipCallerBase<Keyword>
    {
        [SerializeField, Required] private TMP_Text _textField;

        private void OnValidate()
        {
            _textField ??= GetComponent<TMP_Text>();
            if (!_textField)
            {
                Debug.LogError("TextTooltipCaller requires a TMP_Text component!");
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

        private IEnumerator CheckHoveredWord()
        {
            while (true)
            {
                int hoveredWordIndex = TMP_TextUtilities.FindIntersectingWord(_textField, Input.mousePosition, null);

                if (hoveredWordIndex != -1)
                {
                    var hoveredWord = _textField.textInfo.wordInfo[hoveredWordIndex].GetWord();
                    if (KeywordDictionary.Contain(hoveredWord))
                    {
                        var data = KeywordDictionary.GetFormated(hoveredWord);

                        if (!data)
                        {
                            Debug.LogWarning($"Keyword {hoveredWord} not found!");
                            yield break;
                        }

                        ShowTooltip(data);
                    }
                }
                else if (CurrentTooltip)
                {
                    TooltipPool.ReturnTooltip<TooltipData>(CurrentTooltip);
                    CurrentTooltip = null;
                }

                yield return null;
            }
        }
    }
}