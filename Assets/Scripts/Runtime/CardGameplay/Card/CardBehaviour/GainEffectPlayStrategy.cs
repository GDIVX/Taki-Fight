using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Gain Effect Play Strategy", menuName = "Card/Strategy/Play/Gain Effect", order = 0)]
    public class GainEffectPlayStrategy : CardPlayStrategy
    {
        public override void Play(PawnController caller, int potency)
        {
            throw new System.NotImplementedException();
        }
    }
}