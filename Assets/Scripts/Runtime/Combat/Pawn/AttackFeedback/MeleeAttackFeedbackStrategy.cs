using System;
using DG.Tweening;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [CreateAssetMenu(fileName = "Melee Attack Feedback", menuName = "Pawns/Attack Feedback/Melee")]
    public class MeleeAttackFeedbackStrategy : AttackFeedbackStrategy
    {
        [SerializeField] private MeleeAttackFeedbackParams _params;

        public override void Play(PawnController attacker, PawnController target, Action onComplete)
        {
            if (!attacker || !target)
            {
                onComplete?.Invoke();
                return;
            }

            var origin = attacker.transform.position;
            var sequence = DOTween.Sequence();
            sequence.Append(attacker.transform.DOMove(target.transform.position, _params.MoveDuration));
            sequence.AppendCallback(() => PlayEffects(attacker, _params));
            sequence.Append(attacker.transform.DOMove(origin, _params.MoveDuration));
            sequence.OnComplete(() => onComplete?.Invoke());
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
                var vfx = Object.Instantiate(parameters.VfxPrefab, pawn.transform.position, Quaternion.identity);
                Object.Destroy(vfx, 2f);
            }
        }
    }
}
