using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Loop", menuName = "Card/Strategy/Loop", order = 0)]
    public class LoopPlayStrategy : CardPlayStrategy
    {
        [SerializeField] private CardPlayStrategy _nestedStrategy;
        [SerializeField] private int _repeats;

        public override void Play(PawnController caller, int potency)
        {
            for (int i = 0; i < _repeats; i++)
            {
                _nestedStrategy.Play(caller, potency);
            }
        }
    }
}