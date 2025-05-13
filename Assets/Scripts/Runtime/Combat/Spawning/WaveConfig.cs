using System.Collections.Generic;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.Combat.Spawning
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Combat/WaveConfig", order = 0)]
    public class WaveConfig : ScriptableObject
    {
        [Tooltip("List of enemy units to spawn in this wave.")]
        public List<PawnData> Enemies;

        [Range(0, 1)] public float DifficultyLevel;
    }
}