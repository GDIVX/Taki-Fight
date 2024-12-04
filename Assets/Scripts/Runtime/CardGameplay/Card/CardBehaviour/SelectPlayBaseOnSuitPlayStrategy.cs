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


        public override void Play(PawnController caller, int potency)
        {
            var currentSuit = GameManager.Instance.CurrentSuit;

            var suitToPlay = _plays.FirstOrDefault(s => s.Suit == currentSuit);
            if (!suitToPlay.Strategy)
            {
                Debug.LogError($"No play strategy was chosen for {this}");
                return;
            }

            suitToPlay.Strategy.Play(caller, suitToPlay.Potency);
        }


        [System.Serializable]
        public struct MapSuitToPlay
        {
            public Suit Suit;
            public int Potency;
            public CardPlayStrategy Strategy;
        }
    }
}