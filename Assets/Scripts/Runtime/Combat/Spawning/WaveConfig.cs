using Runtime.Combat.Pawn;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Combat.Spawning
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Combat/WaveConfig", order = 0)]
    public class WaveConfig : ScriptableObject
    {
        [Tooltip("List of enemy units to spawn in this wave.")]
        public List<PawnData> Enemies;

        [Tooltip("Difficulty level of this wave (e.g., 1 = weak, 3 = strong).")]
        public int DifficultyLevel;
    }
}
