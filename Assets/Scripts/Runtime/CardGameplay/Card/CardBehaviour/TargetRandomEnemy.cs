using Runtime.Combat.Pawn;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Target Random Enemy", menuName = "Card/Strategy/Select/Random Enemy", order = 0)]
    public class TargetRandomEnemy : TargetingStrategy
    {
        public override PawnController GetTarget()
        {
            var enemies = GameManager.Instance.Enemies.Pawns;

            return enemies.SelectRandom();
        }
    }
}