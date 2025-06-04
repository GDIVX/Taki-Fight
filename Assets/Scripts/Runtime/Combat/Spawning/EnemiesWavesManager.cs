using System.Collections.Generic;
using System.Linq;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Spawning
{
    public class EnemiesWavesManager
    {
        private readonly Queue<WaveConfig> _remainingWaves = new();
        private readonly Queue<PawnData> _waitList = new();

        private int _combatLength;

        private AnimationCurve _difficultyCurve;
        private WaveConfig _finalWave;

        private List<WaveConfig> _waves;
        private int _wavesSpawned;

        private static PawnFactory PawnFactory => ServiceLocator.Get<PawnFactory>();
        private static TilemapController Tilemap => ServiceLocator.Get<TilemapController>();

        public void Init(CombatConfig combatConfig)
        {
            _combatLength = combatConfig.CombatLength;
            _waves = combatConfig.Waves;
            _finalWave = combatConfig.FinalWave;
            _difficultyCurve = combatConfig.DifficultyCurve;

            ResetState();

            for (var i = 0; i < _combatLength; i++)
            {
                var wave = i < _combatLength - 1 ? SelectWave(i) : _finalWave;
                _remainingWaves.Enqueue(wave);
            }
        }

        public void OnTurnStart()
        {
            // If the final wave has already been spawned, exit early
            if (!_finalWave) return;

            var enemyOwnedTiles = Tilemap.GetEnemyOwnedTiles();

            // Use a fluid sequence to handle animations and gameplay logic step by step
            var sequence = new TaskRunner();

            sequence
                .Do(TakeOverTile)
                .WaitUntil(AreTileTakeoverAnimationsComplete)
                .Do(SpawnWave)
                .WaitUntil(AreSummonAnimationsComplete)
                .Execute();
        }

        private void SpawnWave()
        {
            var waveToSpawn = _remainingWaves.Count > 0 ? _remainingWaves.Dequeue() : null;
            if (!waveToSpawn) return;

            var unitsToSpawn = new List<PawnData>(_waitList);
            _waitList.Clear();
            unitsToSpawn.AddRange(waveToSpawn.Enemies);

            foreach (var pawnData in unitsToSpawn)
            {
                var tile = FindValidTileForUnit(pawnData);
                if (tile != null)
                    SpawnUnit(pawnData, tile);
                else
                    // Add to waitlist if no valid tile is found
                    _waitList.Enqueue(pawnData);
            }

            _wavesSpawned++;
        }

        private void TakeOverTile()
        {
            var enemyOwnedTiles = Tilemap.GetEnemyOwnedTiles();

            // Find potential tiles to take over
            var potentialTiles = enemyOwnedTiles
                .SelectMany(tile => Tilemap.GetTilesInRange(tile, 1))
                .Where(tile => tile.Owner == TileOwner.None && tile.IsEmpty)
                .ToList();

            if (potentialTiles.Count <= 0) return; // No tiles available to take over

            var tileToTakeOver = potentialTiles.SelectRandom();
            tileToTakeOver.SetOwner(TileOwner.Enemy);
        }

        private static void SpawnUnit(PawnData pawnData, Tile spawnTile)
        {
            var pawnController = PawnFactory.CreatePawn(pawnData, spawnTile);
            pawnController.Owner = PawnOwner.Enemy;

            if (!pawnController)
            {
                Debug.LogWarning($"Failed to summon {pawnData.name} at tile {spawnTile.Position}.");
                return;
            }

            // Play spawn animation
            pawnController.SpawnAtPosition(spawnTile);
        }

        private WaveConfig SelectWave(int turn)
        {
            var minDifficulty = _waves.Min(w => w.DifficultyLevel);
            var maxDifficulty = _waves.Max(w => w.DifficultyLevel);

            var turnProgress = (float)turn / _combatLength;

            var relativeDifficulty = _difficultyCurve.Evaluate(turnProgress);
            var mappedDifficulty = Mathf.Lerp(minDifficulty, maxDifficulty, relativeDifficulty);

            var wavesWithWeights = _waves
                .Select(wave =>
                {
                    var weight = 1f / (1f + Mathf.Abs(wave.DifficultyLevel - mappedDifficulty));
                    return (wave, weight);
                })
                .ToList();

            var totalWeight = wavesWithWeights.Sum(w => w.weight);
            var randomValue = Random.value * totalWeight;

            foreach (var (wave, weight) in wavesWithWeights)
            {
                if (randomValue < weight) return wave;
                randomValue -= weight;
            }

            return null;
        }

        private Tile FindValidTileForUnit(PawnData pawnData)
        {
            var enemyOwnedTiles = Tilemap.GetEnemyOwnedTiles()
                .OrderByDescending(tile => tile.Position.x)
                .ToList();

            foreach (var tile in enemyOwnedTiles)
            {
                if (IsFootprintAvailable(tile, pawnData.Size))
                {
                    return tile;
                }
            }

            return null;
        }

        private bool IsFootprintAvailable(Tile startTile, Vector2Int size)
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var tile = Tilemap.GetTile(startTile.Position + new Vector2Int(x, y));
                    if (tile == null || !tile.IsEmpty || tile.Owner != TileOwner.Enemy) return false;
                }
            }

            return true;
        }

        private void ResetState()
        {
            _wavesSpawned = 0;
            _remainingWaves?.Clear();
            _waitList?.Clear();
        }

        public void StopSpawning()
        {
            _finalWave = null;
            ResetState();
        }

        private bool AreTileTakeoverAnimationsComplete()
        {
            // Ask each TileView directly if its takeover animation is complete
            return Tilemap.GetEnemyOwnedTiles()
                .All(tile => !tile.View.IsAnimating());
        }

        private bool AreSummonAnimationsComplete()
        {
            // Ask each PawnView directly if its summon animation is complete
            return Tilemap.GetUnitsByOwnership(PawnOwner.Enemy)
                .All(pawn => !pawn.View.IsAnimating());
        }
    }
}