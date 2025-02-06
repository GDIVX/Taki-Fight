using Runtime.Combat.Pawn;
using UnityEditor;
using UnityEngine;

namespace Runtime
{
    public class RunBuilder
    {
        public RunData Data { get; private set; }

        public RunBuilder CreateNewRun()
        {
            Data = ScriptableObject.CreateInstance<RunData>();
            return this;
        }

        public RunBuilder AddHeroData(PawnData pawnData)
        {
            Data.Hero = pawnData;
            return this;
        }
    }
}