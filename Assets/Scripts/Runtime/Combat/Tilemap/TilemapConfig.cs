using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Combat.Tilemap
{
    [CreateAssetMenu(fileName = "TilemapConfig", menuName = "Game/Settings/TilemapConfig", order = 1)]
    public class TilemapConfig : ScriptableObject
    {
        [SerializeField] int rows = 3;

        [SerializeField] List<TileOwner> colums = new List<TileOwner>();

        public int Rows => rows;
        public List<TileOwner> Colums => colums;
    }
}
