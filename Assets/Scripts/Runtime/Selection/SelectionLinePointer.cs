using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Selection
{
    public class SelectionLinePointer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Camera _camera;

        [Title("Curve Settings")]
        [SerializeField, Min(2)] private int _curveResolution = 20;
        [SerializeField, Range(0f, 5f)] private float _curveStrength = 1f;
        [SerializeField, Range(0f, 1f)] private float _curveBias = 0.5f; // 0 = control point near origin, 1 = near target

        private bool _isActive;
        private Vector3 _origin;

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
            Vector3 target = _camera.ScreenToWorldPoint(mouseScreenPosition);

            UpdateCurve(_origin, target);
        }

        private void UpdateCurve(Vector3 start, Vector3 end)
        {
            _lineRenderer.positionCount = _curveResolution;

            // Direction and perpendicular influence
            Vector3 direction = end - start;
            float upwardness = Vector3.Dot(direction.normalized, Vector3.up);
            float curvatureFactor = (1f - Mathf.Clamp01(upwardness)) * _curveStrength;

            // Bias determines control point's position between start and end
            Vector3 control = Vector3.Lerp(start, end, _curveBias) + Vector3.up * curvatureFactor;

            for (int i = 0; i < _curveResolution; i++)
            {
                float t = i / (float)(_curveResolution - 1);
                Vector3 point = CalculateQuadraticBezierPoint(t, start, control, end);
                _lineRenderer.SetPosition(i, point);
            }
        }

        private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            return u * u * p0 + 2 * u * t * p1 + t * t * p2;
        }

        [Button]
        public void Show(Vector3 pointerOrigin)
        {
            _origin = _camera.ScreenToWorldPoint(new Vector3(pointerOrigin.x, pointerOrigin.y, _camera.nearClipPlane));
            _isActive = true;
            _lineRenderer.enabled = true;
        }

        [Button]
        public void Hide()
        {
            _isActive = false;
            _lineRenderer.enabled = false;
        }
    }
}
