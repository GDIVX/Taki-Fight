using System;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Defend", order = 0)]
    public class DefenseCardPlay : CardPlayStrategy
    {
        [SerializeField] private int defensePoints;

        public override void Play(CardController card)
        {
            var hero = CombatManager.Instance.Hero;
            if (!hero)
            {
                throw new NullReferenceException("Hero pawn manager was not assigned to the combat manager");
            }

            hero.defense.Value += defensePoints;
        }
    }
}