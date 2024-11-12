using System.Collections.Generic;
using System.Linq;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card.CardBehaviour
{
    [CreateAssetMenu(fileName = "Adapt Play", menuName = "Card/Strategy/Play/Adapt", order = 0)]
    public class SelectPlayBaseOnSuitPlayStrategy : CardPlayStrategy
    {
        [SerializeField] private List<MapSuitToPlay> _plays;


        public override void Play(PawnController caller)
        {
            var currentSuit = GameManager.Instance.CurrentSuit;

            var suitToPlay = _plays.FirstOrDefault(s => s.Suit == currentSuit);
            if (suitToPlay.Strategy != null)
            {
                suitToPlay.Strategy.Play(caller, suitToPlay.Value);
            }
        }

        public override void Play(PawnController caller, int value)
        {
            Play(caller);
        }


        [System.Serializable]
        public struct MapSuitToPlay
        {
            public Suit Suit;
            public int Value;
            public CardPlayStrategy Strategy;
        }
    }
}