using Runtime.CardGameplay.GemSystem;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Find Gem", menuName = "Card/Strategy/Alchemy/Find", order = 0)]
    public class FindGemPlay : CardPlayStrategy
    {
        [SerializeField] private GemType _gemType;

        public override void Play(PawnController caller, int potency)
        {
            //The draw method would check if the gems exist at each iteration, so we don't need to do any safety check in here
            GameManager.Instance.GemsBag.Draw(_gemType, potency);
        }
    }
}