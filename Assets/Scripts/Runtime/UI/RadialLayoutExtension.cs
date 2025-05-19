using Sirenix.OdinInspector;
using Utilities;

namespace Runtime.UI
{
    public static class RadialLayoutExtension
    {
        /// <summary>
        ///     Flips the angles of the radial layout by 180 degrees and normalizes them to the range [0, 360).
        /// </summary>
        [Button]
        public static void Flip(this RadialLayout layout)
        {
            var min = layout.MinAngle;
            var max = layout.MaxAngle;
            layout.MinAngle = max;
            layout.MaxAngle = min;
            layout.StartAngle = NormalizeAngle(layout.StartAngle);
        }

        private static float NormalizeAngle(float angle)
        {
            return (angle - 180f) % 360f;
        }
    }
}