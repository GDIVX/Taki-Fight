using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Utilities
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            int listCount = list.Count;
            for (int i = listCount - 1; i > 0; i--)
            {
                int randomSelection = Random.Range(0, i + 1);
                //Swap the elements
                (list[i], list[randomSelection]) = (list[randomSelection], list[i]);
            }
        }
        
        public static T SelectRandom<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }

            int index = Random.Range(0, list.Count);
            return list[index];
        }
        
        public static T WeightedSelectRandom<T>(this List<T> list, Func<T, float> weightSelector)
        {
            float totalWeight = CalculateTotalWeight(list, weightSelector);
            float randomValue = GetRandomValue(totalWeight);
            return GetItemBasedOnWeight(list, weightSelector, randomValue);
        }

        private static float CalculateTotalWeight<T>(List<T> list, Func<T, float> weightSelector)
        {
            float totalWeight = 0f;
            foreach (var item in list)
            {
                totalWeight += weightSelector(item);
            }

            return totalWeight;
        }

        private static float GetRandomValue(float maxValue)
        {
            return Random.Range(0, maxValue);
        }

        private static T GetItemBasedOnWeight<T>(List<T> list, Func<T, float> weightSelector, float randomValue)
        {
            float cumulativeWeight = 0f;
            foreach (var item in list)
            {
                cumulativeWeight += weightSelector(item);
                if (randomValue < cumulativeWeight)
                {
                    return item;
                }
            }

            return default;
        }
    }
}
