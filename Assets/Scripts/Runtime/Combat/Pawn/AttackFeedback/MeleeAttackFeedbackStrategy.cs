using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Combat.Pawn.AttackFeedback
{
    [CreateAssetMenu(fileName = "Melee Attack Feedback", menuName = "Pawns/Attack Feedback/Melee")]
    public class MeleeAttackFeedbackStrategy : AttackFeedbackStrategy
    {
        private MeleeAttackFeedbackParams _params;

        public override void Initialize(AttackFeedbackStrategyData data)
        {
            _params = data.Parameters as MeleeAttackFeedbackParams;
        }

        public override void Play(PawnController attacker, PawnController target, Action onComplete)
        {
            if (!attacker || !target)
            {
                onComplete?.Invoke();
                return;
            }

            var origin = attacker.TilemapHelper.AnchorTile.Position;
            var destination = target.TilemapHelper.AnchorTile.Position;

            attacker.View.MoveToPosition(destination, () =>
            {
                PlayEffects(attacker, _params);
                attacker.View.MoveToPosition(origin, onComplete);
            });
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
