using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class TaskRunner : IDisposable
    {
        private readonly Queue<Func<IEnumerator>> _steps = new();

        private bool _executed;
        private Action _onComplete;

        public void Dispose()
        {
            // Clear all the steps in the queue
            _steps.Clear();

            // Cleanup the onComplete callback
            _onComplete = null;
        }

        public TaskRunner Do(Action action)
        {
            _steps.Enqueue(() => RunAction(action));
            return this;
        }

        public TaskRunner Wait(float seconds)
        {
            _steps.Enqueue(() => WaitCoroutine(seconds));
            return this;
        }

        public TaskRunner OnComplete(Action action)
        {
            _onComplete = action;
            return this;
        }

        public TaskRunner WaitUntil(Func<bool> condition)
        {
            _steps.Enqueue(() => WaitUntilCoroutine(condition));
            return this;
        }

        public void Execute()
        {
            if (_executed)
            {
                Debug.LogWarning("Sequence already executed.");
                return;
            }

            _executed = true;
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
