using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    public interface ICardController
    {
        Suit Suit { get; set; }
        int Rank { get; set; }
        int EnergyCost { get; set; }
        CardInstance Instance { get; }
        Transform Transform { get; }
        CardView View { get; }
        void OnDiscard();
    }
}