﻿using System.Collections.Generic;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    public class CardFactory : MonoService<CardFactory>
    {
        [SerializeField] private CardController _prefab;

        [SerializeField, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, TabGroup("Dependencies")]
        private Energy.Energy _energy;


        [SerializeField, TabGroup("Dependencies")]
        private Transform _discardToLocation, _drawFromLocation;

        private readonly Stack<CardController> _objectPool = new();


        private CardDependencies _cardDependencies;


        [Button]
        public CardController Create(CardData data)
        {
            var instance = new CardInstance(data);
            return Create(instance);
        }

        public CardController Create(CardInstance instance)
        {
            CardController controller = GetController();
            if (controller.Init(instance))
            {
                controller.gameObject.SetActive(true);
                return controller;
            }

            return null;
        }

        /// <summary>
        /// Remove the card from the front end of the game. Does not destroy the card from all reference points.
        /// </summary>
        public void ReturnToPool(CardController controller)
        {
            controller.gameObject.SetActive(false);
            _objectPool.Push(controller);
        }

        private CardController GetController()
        {
            return _objectPool.Count > 0
                ? _objectPool.Pop()
                : Instantiate(_prefab); //Create a new instance
        }
    }
}