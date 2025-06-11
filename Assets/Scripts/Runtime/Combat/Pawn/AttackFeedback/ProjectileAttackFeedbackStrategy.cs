using System;
using DG.Tweening;
using UnityEngine;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [CreateAssetMenu(fileName = "Projectile Attack Feedback", menuName = "Pawns/Attack Feedback/Projectile")]
    public class ProjectileAttackFeedbackStrategy : AttackFeedbackStrategy
    {
        [SerializeField] private ProjectileAttackFeedbackParams _params;

        public override void Play(PawnController attacker, PawnController target, Action onComplete)
        {
            if (!attacker || !target || _params == null)
            {
                onComplete?.Invoke();
                return;
            }

            GameObject projectile = null;
            if (_params.ProjectilePrefab)
            {
                projectile = Instantiate(_params.ProjectilePrefab, attacker.transform.position, Quaternion.identity);
            }

            var sequence = DOTween.Sequence();
            if (projectile)
            {
                sequence.Append(projectile.transform.DOMove(target.transform.position, _params.ProjectileDuration));
            }
            else
            {
                sequence.AppendInterval(_params.ProjectileDuration);
            }

            sequence.AppendCallback(() =>
            {
                if (_params.ImpactVfxPrefab)
                {
                    var impact = Instantiate(_params.ImpactVfxPrefab, target.transform.position, Quaternion.identity);
                    Destroy(impact, 2f);
                }

                if (projectile)
                {
                    Destroy(projectile);
                }
            });

            sequence.OnComplete(() => onComplete?.Invoke());
        }
    }
}
