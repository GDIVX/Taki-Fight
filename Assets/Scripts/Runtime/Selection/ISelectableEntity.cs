using System;

namespace Runtime.Selection
{
    public interface ISelectableEntity
    {
        public void TryToSelect();
        public void OnSelected();
        public void OnDeselected();
    }
}
