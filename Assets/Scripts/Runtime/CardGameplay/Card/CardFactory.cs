using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Board;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    public class CardFactory : MonoBehaviour, ICardFactory
    {
        [SerializeField] private CardController Prefab;

        [SerializeField, TabGroup("Dependencies")]
        private HandController _handController;
        [SerializeField, TabGroup("Dependencies")]
        private BoardController _boardController;
        [SerializeField, TabGroup("Dependencies")]
        private GameManager _gameManager;
        
        private CardDependencies _cardDependencies;

        private readonly Stack<CardController> _objectPool = new();

        private void Start()
        {
            FetchDependencies();
        }

        private void FetchDependencies()
        {
            _cardDependencies = new CardDependencies(_handController, _boardController, _gameManager, this);
        }

        [Button]
        public CardController Create(CardData data, int number, Suit suit = Suit.Default)
        {
            CardController controller = GetController();
            controller.Init(data, number, suit, _cardDependencies);
            controller.gameObject.GetComponent<CardView>().Draw(data, number, suit);
            controller.gameObject.SetActive(true);
            return controller;
        }

        public CardController Create(CardInstance instance)
        {
            return Create(instance.Data, instance.Rank, instance.Suit);
        }

        /// <summary>
        /// Remove the card from the front end of the game. Does not destroy the card from all refrence points.
        /// </summary>
        public void Disable(CardController controller)
        {
            controller.gameObject.SetActive(false);
            _objectPool.Push(controller);
        }

        private CardController GetController()
        {
            return _objectPool.Count > 0
                ? _objectPool.Pop()
                :
                //Create a new instance
                Instantiate(Prefab);
        }
    }
}