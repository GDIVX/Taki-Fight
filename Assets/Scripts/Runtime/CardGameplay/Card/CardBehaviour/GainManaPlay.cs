using Runtime.CardGameplay.ManaSystem;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "GainManaStrategy", menuName = "Card/Strategy/Play/ManaGain", order = 0)]
    public class GainManaPlay : CardPlayStrategy
    {
        [SerializeField] private ManaDefinition _manaDefinition;

        public override void Play(PawnController caller, int potency)
        {
            for (int i = 0; i < potency; i++)
            {
                GameManager.Instance.ManaInventory.Add(_manaDefinition.InstantiateMana());
            }
        }
    }
}