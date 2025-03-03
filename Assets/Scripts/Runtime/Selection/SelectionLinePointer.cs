using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Selection
{
    public class SelectionLinePointer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Camera _camera;
        private bool _isActive;

        private void Awake()
        {
            _lineRenderer ??= GetComponentInChildren<LineRenderer>();
            _camera ??= Camera.main;
        }

        private void Start()
        {
            Hide();
        }

        private void Update()
        {
            if (!_isActive) return;

            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = _camera.nearClipPlane;

            Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(mouseScreenPosition);
            _lineRenderer.SetPosition(1, mouseWorldPosition);
        }

        [Button]
        public void Show(Vector3 pointerOrigin)
        {
            Vector3 worldPosition =
                _camera.ScreenToWorldPoint(new Vector3(pointerOrigin.x, pointerOrigin.y, _camera.nearClipPlane));
            _lineRenderer.SetPosition(0, worldPosition);
            _lineRenderer.enabled = true;
            _isActive = true;
        }

        [Button]
        public void Hide()
        {
            _lineRenderer.enabled = false;
            _isActive = false;
        }
    }
}