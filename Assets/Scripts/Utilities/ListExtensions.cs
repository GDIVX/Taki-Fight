using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class ListExtensions
    {
        /// <summary>
        /// Shuffles the list using the Fisher-Yates algorithm with Unity's Random.
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1); // Unity's Random.Range for inclusive upper bound
                (list[i], list[j]) = (list[j], list[i]); // Swap elements
            }
        }

        /// <summary>
        /// Picks a random element from the list using Unity's Random.
        /// </summary>
        public static T PickRandom<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default; // Return default if the list is empty or null
            }

            int index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}