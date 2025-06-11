using System.Collections;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.AttackFeedback;

namespace TakiFight.Tests
{
    public class PawnCombatTests
    {
        [Test]
        public void Attack_ShouldInvokeFeedbackStrategy()
        {
            var attackerData = new PawnData { Damage = 1, Defense = 0, Attacks = 1 };
            var targetData = new PawnData { Damage = 0, Defense = 0, Attacks = 0 };
            var feedback = new TestAttackFeedbackStrategy();
            attackerData.AttackFeedbackStrategy = feedback;

            var attacker = new PawnController();
            attacker.Init(attackerData);
            var target = new PawnController();
            target.Init(targetData);

            IEnumerator enumerator = attacker.Combat.Attack(target, null);
            while (enumerator.MoveNext())
            {
            }

            Assert.That(feedback.Played, Is.True);
        }
    }
}
