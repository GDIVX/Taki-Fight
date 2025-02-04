using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
    public class ArrangeInLine : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;

        [SerializeField] private List<GameObject> gameObjectsToArrange;
        [SerializeField] private float padding = 0f;

        public void Add(GameObject child)
        {
            if (child == null) return;
            gameObjectsToArrange.Add(child);
            ArrangeObjects();
        }

        public void Remove(GameObject child)
        {
            if (child == null) return;
            gameObjectsToArrange.Remove(child);
            ArrangeObjects();
        }

        [Button]
        private void ArrangeObjects()
        {
            if (gameObjectsToArrange == null || gameObjectsToArrange.Count == 0)
            {
                Debug.LogWarning("No GameObjects specified to arrange.");
                return;
            }

            if (pointA == null || pointB == null)
            {
                Debug.LogWarning("Please assign both pointA and pointB transforms.");
                return;
            }

            Vector3 direction = pointB.position - pointA.position;
            Vector3 unitDirection = direction.normalized;
            float totalDistance = direction.magnitude;

            if (totalDistance < padding * 2)
            {
                Debug.LogWarning("Padding is too large for the distance between pointA and pointB.");
                return;
            }

            float distanceBetweenObjects = gameObjectsToArrange.Count == 1
                ? 0
                : (totalDistance + padding * 2) / (gameObjectsToArrange.Count - 1);

            for (int i = 0; i < gameObjectsToArrange.Count; i++)
            {
                if (gameObjectsToArrange[i] == null)
                {
                    gameObjectsToArrange.RemoveAt(i);
                    i--;
                    continue;
                }

                Vector3 targetPosition = pointA.position + (unitDirection * (padding + distanceBetweenObjects * i));
                gameObjectsToArrange[i].transform.position = targetPosition;
            }
        }
    }
}