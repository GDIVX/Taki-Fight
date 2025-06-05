using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Combat.Tilemap
{
    [CreateAssetMenu(fileName = "TilemapConfig", menuName = "Game/Settings/TilemapConfig", order = 1)]
    public class TilemapConfig : ScriptableObject
    {
        [SerializeField] int rows = 3;

        [FormerlySerializedAs("colums")]
        [SerializeField] List<TileOwner> columns = new List<TileOwner>();

        public int Rows => rows;
        public List<TileOwner> Columns => columns;
    }
}
