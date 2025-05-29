using Runtime.CardGameplay.Deck;
using UnityEngine;

namespace Runtime.RunManagement
{
    public class RunBuilder
    {
        private readonly GameRunState _gameRunState = new();

        public GameRunState Build()
        {
            //build deck
            _gameRunState.Deck = new Deck(_gameRunState.Cards);
            return _gameRunState;
        }

        public RunBuilder WithPrimarySchool(School school)
        {
            _gameRunState.PrimarySchool = school;
            _gameRunState.Mage = school.Mage;

            //add all starter cards
            _gameRunState.Cards = school.StarterCards;

            return this;
        }

        public RunBuilder WithSecondarySchool(School school)
        {
            _gameRunState.SecondarySchool = school;

            //add all starter cards
            _gameRunState.Cards.AddRange(school.StarterCards);

            return this;
        }

        public RunBuilder WithSeed(int seed)
        {
            Random.InitState(seed);
            return this;
        }

        public RunBuilder WithRandomSeed()
        {
            var randomSeed = new System.Random().Next(); // Generate a random seed
            Random.InitState(randomSeed); // Initialize Unity's random state with the random seed
            return this;
        }
    }
}