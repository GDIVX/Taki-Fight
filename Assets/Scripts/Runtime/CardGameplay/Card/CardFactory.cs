﻿using System;
using System.Collections.Generic;
using Runtime.CardGameplay.Card.View;
using Runtime.CardGameplay.Deck;
using Runtime.Combat.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.CardGameplay.Card
{
    public class CardFactory : MonoBehaviour
    {
        [SerializeField] private CardController _prefab;

        [SerializeField, TabGroup("Dependencies")]
        private HandController _handController;

        [SerializeField, TabGroup("Dependencies")]
        private Energy.Energy _energy;

        [SerializeField, TabGroup("Dependencies")]
        private Transform _discardToLocation, _drawFromLocation;


        private CardDependencies _cardDependencies;

        private readonly Stack<CardController> _objectPool = new();

        public void Init()
        {
        }

        [Button]
        public CardController Create(CardData data)
        {
            CardController controller = GetController();
            controller.Init(data, _cardDependencies);
            controller.gameObject.GetComponent<CardView>()
                .Init(_drawFromLocation, _discardToLocation)
                .Draw(controller);
            controller.gameObject.SetActive(true);
            return controller;
        }

        public CardController Create(CardInstance instance)
        {
            return Create(instance.Data);
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
                Instantiate(_prefab);
        }
    }
}