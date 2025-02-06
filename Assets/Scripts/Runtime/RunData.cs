using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime
{
    [CreateAssetMenu(fileName = "Run Save File", menuName = "Game/RunData", order = 0)]
    public class RunData : ScriptableObject
    {
        public PawnData Hero;
    }
}