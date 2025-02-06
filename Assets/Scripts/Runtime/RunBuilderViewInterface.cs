using System;
using UnityEngine;

namespace Runtime
{
    public class RunBuilderViewInterface : MonoBehaviour
    {
        private RunBuilder _runBuilder;

        private void Awake()
        {
            _runBuilder = new RunBuilder();
        }
        
        
    }
}