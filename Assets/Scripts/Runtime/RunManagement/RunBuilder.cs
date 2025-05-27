using Runtime.CardGameplay.Deck;
using UnityEngine;

namespace Runtime.RunManagement
{
    public class RunBuilder
    {
        private readonly RunState _runState = new();

        public RunState Build()
        {
            //build deck
            _runState.Deck = new Deck(_runState.Cards);
            return _runState;
        }

        public RunBuilder WithPrimarySchool(School school)
        {
            _runState.PrimarySchool = school;
            _runState.Mage = school.Mage;

            //add all starter cards
            _runState.Cards = school.StarterCards;

            return this;
        }

        public RunBuilder WithSecondarySchool(School school)
        {
            _runState.SecondarySchool = school;

            //add all starter cards
            _runState.Cards.AddRange(school.StarterCards);

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