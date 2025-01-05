using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CardGameplay.SlotMachineLib
{
    public class ReelView : MonoBehaviour
    {
        [SerializeField] private ReelController _controller;
        [SerializeField] private Image _image;

        private void OnValidate()
        {
            _controller ??= GetComponent<ReelController>();
            _image ??= GetComponentInChildren<Image>();
        }

        private void Awake()
        {
            _controller.OnSpin += (c) => AnimateSpin();
        }

        private void AnimateSpin()
        {
            var sprite = _controller.CurrentSymbol.Sprite;
            _image.sprite = sprite;
        }
    }
}