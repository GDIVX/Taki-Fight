using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class MonoBehaviourExtension
    {
        public static void Timer(this MonoBehaviour gameObject, float waitTime, Action onTimesUp)
        {
            gameObject.StartCoroutine(TimerCoroutine(waitTime, onTimesUp));
        }

        private static IEnumerator TimerCoroutine(float waitTime, Action onTimesUp)
        {
            yield return new WaitForSeconds(waitTime);
            onTimesUp?.Invoke();
        }
    }
}