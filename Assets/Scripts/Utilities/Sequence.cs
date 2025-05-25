using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class Sequence
    {
        private readonly Queue<Func<IEnumerator>> _steps = new();
        private Action _onComplete;

        public Sequence Do(Action action)
        {
            _steps.Enqueue(() => RunAction(action));
            return this;
        }

        public Sequence Wait(float seconds)
        {
            _steps.Enqueue(() => WaitCoroutine(seconds));
            return this;
        }

        public Sequence WaitUntil(Func<bool> condition)
        {
            _steps.Enqueue(() => WaitUntilCoroutine(condition));
            return this;
        }

        public void Execute(Action onComplete = null)
        {
            _onComplete = onComplete;
            CoroutineRunner.Instance.StartCoroutine(RunSequence());
        }

        private IEnumerator RunSequence()
        {
            while (_steps.Count > 0)
            {
                var step = _steps.Dequeue();
                yield return step();
            }

            _onComplete?.Invoke();
        }

        private static IEnumerator RunAction(Action action)
        {
            action?.Invoke();
            yield return null;
        }

        private static IEnumerator WaitCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        private static IEnumerator WaitUntilCoroutine(Func<bool> condition)
        {
            yield return new WaitUntil(condition);
        }
    }
}