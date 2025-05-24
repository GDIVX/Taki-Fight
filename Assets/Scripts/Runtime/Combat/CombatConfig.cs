using System.Collections.Generic;
using Runtime.Combat.Spawning;
using UnityEngine;

namespace Runtime.Combat
{
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "Combat/CombatConfig", order = 0)]
    public class CombatConfig : ScriptableObject
    {
        [Tooltip("Number of waves to spawn before the final wave.")]
        public int CombatLength = 5;

        [Tooltip("List of waves available for this combat.")]
        public List<WaveConfig> Waves;

        [Tooltip("The final wave to spawn at the end of the combat.")]
        public WaveConfig FinalWave;

        [Tooltip("Defines difficulty progression over turns.")]
        public AnimationCurve DifficultyCurve; // X-axis: Turn, Y-axis: Difficulty
    }
}