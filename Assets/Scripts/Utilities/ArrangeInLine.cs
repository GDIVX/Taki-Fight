using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
    public class ArrangeInLine : MonoBehaviour
    {
        public enum ArrangementMode
        {
            EvenSpread,
            StartToEnd,
            EndToStart
        }

        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;

        [SerializeField] private List<GameObject> _gameObjectsToArrange;
        [SerializeField] private float _padding = 0f;

        [SerializeField] private ArrangementMode _arrangementMode = ArrangementMode.EvenSpread;

        protected void Add(GameObject child)
        {
            if (child == null) return;
            _gameObjectsToArrange.Add(child);
            ArrangeObjects();
        }

        protected void Remove(GameObject child)
        {
            if (child == null) return;
            _gameObjectsToArrange.Remove(child);
            ArrangeObjects();
        }

        [Button]
        private void ArrangeObjects()
        {
            if (_gameObjectsToArrange == null || _gameObjectsToArrange.Count == 0)
            {
                Debug.LogWarning("No GameObjects specified to arrange.");
                return;
            }

            if (_pointA == null || _pointB == null)
            {
                Debug.LogWarning("Please assign both pointA and pointB transforms.");
                return;
            }

            Vector3 direction = _pointB.position - _pointA.position;
            Vector3 unitDirection = direction.normalized;
            float totalDistance = direction.magnitude;

            if (totalDistance < _padding * 2)
            {
                Debug.LogWarning("Padding is too large for the distance between pointA and pointB.");
                return;
            }

            switch (_arrangementMode)
            {
                case ArrangementMode.EvenSpread:
                    ArrangeEvenSpread(totalDistance, unitDirection);
                    break;
                case ArrangementMode.StartToEnd:
                    ArrangeSequential(unitDirection, fromStart: true);
                    break;
                case ArrangementMode.EndToStart:
                    ArrangeSequential(unitDirection, fromStart: false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ArrangeEvenSpread(float totalDistance, Vector3 unitDirection)
        {
            float distanceBetweenObjects = _gameObjectsToArrange.Count == 1
                ? 0
                : (totalDistance + _padding * 2) / (_gameObjectsToArrange.Count - 1);

            for (int i = 0; i < _gameObjectsToArrange.Count; i++)
            {
                if (_gameObjectsToArrange[i] == null)
                {
                    _gameObjectsToArrange.RemoveAt(i);
                    i--;
                    continue;
                }

                Vector3 targetPosition = _pointA.position + unitDirection * (_padding + distanceBetweenObjects * i);
                _gameObjectsToArrange[i].transform.position = targetPosition;
            }
        }

        private void ArrangeSequential(Vector3 unitDirection, bool fromStart)
        {
            for (int i = 0; i < _gameObjectsToArrange.Count; i++)
            {
                if (_gameObjectsToArrange[i] == null)
                {
                    _gameObjectsToArrange.RemoveAt(i);
                    i--;
                    continue;
                }

                int index = fromStart ? i : _gameObjectsToArrange.Count - 1 - i;
                Vector3 basePoint = fromStart ? _pointA.position : _pointB.position;
                Vector3 targetPosition = basePoint + unitDirection * ((_padding + 1f) * index);
                _gameObjectsToArrange[i].transform.position = targetPosition;
            }
        }
    }
}
