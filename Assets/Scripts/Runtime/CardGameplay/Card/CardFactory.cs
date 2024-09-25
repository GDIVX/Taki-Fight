using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.CardGameplay.Card
{
    public class CardFactory : Singleton<CardFactory>
    {
        [SerializeField] private CardController prefab;

        private readonly Stack<CardController> _objectPool = new();

        [Button]
        public CardController Create(CardData data, int number, Suit suit)
        {
            CardController controller = GetController();
            controller.Init(data, number, suit);
            controller.gameObject.GetComponent<CardView>().Draw(data, number, suit);
            controller.gameObject.SetActive(true);
            return controller;
        }

        public CardController Create(CardInstance instance)
        {
            return Create(instance.data, instance.number, instance.Suit);
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
                Instantiate(prefab);
        }
    }
}