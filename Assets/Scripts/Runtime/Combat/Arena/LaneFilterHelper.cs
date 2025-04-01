using System;

namespace Runtime.Combat.Arena
{
    public static class LaneFilterHelper
    {
        public enum LaneSelectionMode
        {
            Ally,
            Enemy,
            Both
        }

        public static bool FilterLaneSide(LaneSide laneSide, LaneSelectionMode selectionMode)
        {
            return selectionMode switch
            {
                LaneSelectionMode.Both => true,
                LaneSelectionMode.Ally => laneSide.IsAllySide,
                LaneSelectionMode.Enemy => !laneSide.IsAllySide,
                _ => throw new ArgumentOutOfRangeException(nameof(selectionMode), selectionMode, null)
            };
        }
    }
}