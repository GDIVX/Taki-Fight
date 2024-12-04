using Runtime.CardGameplay.Board;
using Runtime.Combat.Pawn;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    public abstract class CardPlayStrategy : ScriptableObject
    {
        [SerializeField] private float _duration = 1f;
        public float Duration => _duration;
        

        public abstract void Play(PawnController caller, int potency);

        public virtual void PostPlay(IBoardController boardController, IHandController handController,
            ICardController cardController)
        {
            boardController.UpdateCurrentSuitAndRank(cardController);
            handController.DiscardCard(cardController);
        }
        
        
    }
}