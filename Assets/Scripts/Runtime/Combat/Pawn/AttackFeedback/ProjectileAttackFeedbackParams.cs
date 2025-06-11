using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [Serializable]
    public class ProjectileAttackFeedbackParams : AttackFeedbackParams
    {
        public GameObject ProjectilePrefab;
        public GameObject ImpactVfxPrefab;
        public float ProjectileDuration = 0.25f;
    }
}
