using System;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Attack Play", menuName = "Card/Strategy/Play/Defend", order = 0)]
    public class DefenseCardPlay : CardPlayStrategy
    {
        [SerializeField] private int _defensePoints;

        public override void Play(PawnController caller)
        {
            Play(caller, _defensePoints);
        }

        public override void Play(PawnController caller, int value)
        {
            var hero = GameManager.Instance.Hero;
            if (!hero)
            {
                throw new NullReferenceException("Hero pawn manager was not assigned to the combat manager");
            }

            caller.Defense.Value += value;
        }
    }
}