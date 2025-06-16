using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Combat.Tilemap
{
    using Runtime.Combat.Pawn;

    public class Tile
    {
        public Vector2Int Position { get; }
        public bool IsOccupied { get; set; }
        public PawnController Pawn { get; set; }

        public Tile(Vector2Int position)
        {
            Position = position;
        }
    }

    public class TilemapController
    {
        private readonly Dictionary<Vector2Int, Tile> _tiles = new();

        public void SetTile(Tile tile)
        {
            _tiles[tile.Position] = tile;
        }

        public Tile GetTile(Vector2Int pos)
        {
            return _tiles.TryGetValue(pos, out var t) ? t : null;
        }
    }
}

namespace Runtime.Combat.Pawn
{
    using Runtime.Combat.StatusEffects;
    using Utilities;

    public class PawnTilemapHelper
    {
        public Tile AnchorTile { get; set; }
    }

    public class PawnData
    {
        public List<PawnStrategyData> OnMoveStrategies { get; } = new();
    }

    public struct PawnStrategyData
    {
        public PawnPlayStrategy Strategy;
        public StrategyParams Parameters;
        public int Potency;
    }

    public class PawnCombat
    {
        public Stat Damage { get; } = new Stat();
    }

    public class PawnController : MonoBehaviour
    {
        public PawnOwner Owner { get; set; }
        public PawnData Data { get; set; } = new PawnData();
        public PawnTilemapHelper TilemapHelper { get; set; } = new PawnTilemapHelper();
        public PawnCombat Combat { get; } = new PawnCombat();

        public bool MoveCalled;
        public Tile LastMoveTile;
        public void MoveToPosition(Tile tile, Action onComplete)
        {
            MoveCalled = true;
            LastMoveTile = tile;
            onComplete?.Invoke();
        }

        public List<PawnStrategyData> ExecutedStrategies { get; private set; }
        public void ExecuteStrategies(List<PawnStrategyData> strategies)
        {
            ExecutedStrategies = strategies;
        }

        public bool CaptureReturn { get; set; }
        public int CapturePotency { get; private set; }
        public bool Capture(int potency)
        {
            CapturePotency = potency;
            return CaptureReturn;
        }

        public StatusEffectData AppliedStatus;
        public int AppliedStacks;
        public void ApplyStatusEffect(StatusEffectData data, int stack)
        {
            AppliedStatus = data;
            AppliedStacks = stack;
        }
    }

    public static class PawnHelper
    {
        public static PawnController LastTarget;
        public static int LastMagnitude;
        public static int LastDamage;
        public static Vector2Int LastDirection;

        public static void Knockback(PawnController pawn, int magnitude, int damagePerTile, Vector2Int direction, Action<bool> onComplete = null)
        {
            LastTarget = pawn;
            LastMagnitude = magnitude;
            LastDamage = damagePerTile;
            LastDirection = direction;
            onComplete?.Invoke(true);
        }

        public static void Knockback(PawnController pawn, Vector2Int force, int damagePerTile, Action<bool> onComplete = null)
        {
            LastTarget = pawn;
            LastMagnitude = Math.Abs(force.x) + Math.Abs(force.y);
            LastDamage = damagePerTile;
            LastDirection = force;
            onComplete?.Invoke(true);
        }
    }
}

namespace Utilities
{
    using System.Collections.Generic;

    public class Stat
    {
        public Dictionary<object, int> Modifiers { get; } = new();

        public void SetModifier(object source, int modifier)
        {
            Modifiers[source] = modifier;
        }
    }
}

namespace Runtime.CardGameplay.Card.View
{
    using UnityEngine;

    public class Keyword : ScriptableObject
    {
        public string FormattedText { get; set; }
    }
}

namespace Runtime.Combat.StatusEffects
{
    using Runtime.CardGameplay.Card.View;
    using UnityEngine;

    public class StatusEffectData : ScriptableObject
    {
        public Keyword Keyword { get; set; }
    }
}

namespace Runtime
{
    public class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();
        public bool EndRunCalled;
        public bool WinCombatCalled;

        public void EndRun()
        {
            EndRunCalled = true;
        }

        public void WinCombat()
        {
            WinCombatCalled = true;
        }
    }
}
