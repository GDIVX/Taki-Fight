using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    public class CardFactory : Singleton<CardFactory>
    {
        [SerializeField] private CardController prefab;

        private readonly Stack<CardController> _objectPool = new();

        public CardController Create(CardData data, int number, Suit suit)
        {
            CardController controller = GetController();
            controller.Init(data, number, suit);
            controller.gameObject.GetComponent<CardView>().Draw(data, number);
            controller.gameObject.SetActive(true);
            return controller;
        }

        public CardController Create(CardInstance instance)
        {
            return Create(instance.Data, instance.Number, instance.Suit);
        }

        /// <summary>
        /// Remove the card from the front end of the game. Does not destroy the card from all refrence points.
        /// </summary>
        public void DestroyController(CardController controller)
        {
            controller.gameObject.SetActive(false);
            _objectPool.Push(controller);
        }

        private CardController GetController()
        {
            if (_objectPool.Count > 0)
            {
                return _objectPool.Pop();
            }

            //Create a new instance
            return Instantiate(prefab);
        }
    }
}