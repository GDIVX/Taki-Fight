using Runtime.CardGameplay.GemSystem;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Gain Gem Play Strategy", menuName = "Card/Strategy/Play/Alchemy/Gain Gem", order = 0)]
    public class GainGemPlay : CardPlayStrategy
    {
        [SerializeField] private GemType _gemType;
        [SerializeField] private bool _drawGemsAfterAdding;

        public override void Play(PawnController caller, int potency)
        {
            GameManager.Instance.GemsBag.Add(_gemType, potency);
            if (_drawGemsAfterAdding)
            {
                GameManager.Instance.GemsBag.Draw(_gemType, potency);
            }
        }
    }
}