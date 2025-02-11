using UnityEngine;

namespace Runtime.RunManagement
{
    public class RunBuilderViewInterface : MonoBehaviour
    {
        private RunBuilder _runBuilder;

        private void OnEnable()
        {
            _runBuilder = GameManager.Instance.RunBuilder;
        }

        public void AbandonCurrentRun()
        {
            _runBuilder.Reset();
        }
    }
}