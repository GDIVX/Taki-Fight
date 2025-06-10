using System;

namespace Runtime.Selection
{
    public class SelectionResult
    {
        public ISelectableEntity SelectableEntity;
        public Action<ISelectableEntity> OnComplete;
        public SelectionState State;
    }
}
