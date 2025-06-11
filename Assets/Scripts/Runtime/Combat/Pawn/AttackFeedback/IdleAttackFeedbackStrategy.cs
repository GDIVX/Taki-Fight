using System;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [CreateAssetMenu(fileName = "Idle Attack Feedback", menuName = "Pawns/Attack Feedback/Idle")]
    public class IdleAttackFeedbackStrategy : AttackFeedbackStrategy
    {
        [SerializeField] private AttackFeedbackParams _params;

        public override void Play(PawnController attacker, PawnController target, Action onComplete)
        {
            if (!attacker)
            {
                onComplete?.Invoke();
                return;
            }

            PlayEffects(attacker, _params);
            onComplete?.Invoke();
        }

        private static void PlayEffects(PawnController pawn, AttackFeedbackParams parameters)
        {
            if (parameters == null)
            {
                return;
            }

            if (parameters.Animation)
            {
                var animator = pawn.GetComponent<Animator>();
                animator?.Play(parameters.Animation.name);
            }

            if (parameters.VfxPrefab)
            {
                var vfx = Instantiate(parameters.VfxPrefab, pawn.transform.position, Quaternion.identity);
                Destroy(vfx, 2f);
            }
        }
    }
}
