using System.Collections.Generic;
using System.Linq;
using Runtime.Combat.Pawn;
using Runtime.Combat.Tilemap;
using Runtime.UI.OnScreenMessages;
using UnityEngine;
using Utilities;

namespace Runtime.Combat.Spawning
{
    public class EnemiesWavesManager
    {
        private readonly Queue<PawnData> _waitList = new();

        private int _combatLength;

        private AnimationCurve _difficultyCurve;
        private WaveConfig _finalWave;
        private List<WaveConfig> _remainingWaves;

        private List<WaveConfig> _waves;
        private int _wavesSpawned;

        private PawnFactory PawnFactory => ServiceLocator.Get<PawnFactory>();
        private TilemapController Tilemap => ServiceLocator.Get<TilemapController>();

        public void Init(CombatConfig combatConfig)
        {
            _combatLength = combatConfig.CombatLength;
            _waves = combatConfig.Waves;
            _finalWave = combatConfig.FinalWave;
            _difficultyCurve = combatConfig.DifficultyCurve;

            ResetState();
        }

        public void OnTurnStart()
        {
            // If the final wave has already been spawned, exit early
            if (!_finalWave) return;

            var enemyOwnedTiles = Tilemap.GetEnemyOwnedTiles();

            // Check if spawning is allowed based on the number of controlled tiles
            if (Tilemap.GetUnitsByOwnership(PawnOwner.Enemy).Count < enemyOwnedTiles.Count)
                // Attempt to spawn a wave
                SpawnWave();
            else
                // If spawning is not possible, attempt tile takeover
                TakeOverTile();
        }

        private void SpawnWave()
        {
            // Check if it's time to spawn the final wave
            if (_wavesSpawned >= _combatLength && _finalWave)
            {
                SpawnWave(_finalWave); // Spawn the final wave
                // Display final wave notification
                var onScreenMessageManager = ServiceLocator.Get<MessageManager>();
                onScreenMessageManager.ShowMessage("Final Wave", MessageType.Notification);

                // Stop spawning after the final wave has been spawned
                StopSpawning();
                return;
            }

            // Select and spawn a random wave with difficulty bias
            var waveToSpawn = SelectWave();
            if (waveToSpawn)
            {
                SpawnWave(waveToSpawn);
                _wavesSpawned++;
            }
        }

        private void TakeOverTile()
        {
            var enemyOwnedTiles = Tilemap.GetEnemyOwnedTiles();

            // Find potential tiles to take over
            var potentialTiles = enemyOwnedTiles
                .SelectMany(tile => Tilemap.GetTilesInRange(tile, 1)) // Get adjacent tiles
                .Where(tile => tile.Owner == TileOwner.None && tile.IsEmpty) // Unowned and unoccupied tiles
                .ToList();

            // Take over the first available tile
            if (potentialTiles.Count <= 0) return;
            var tileToTakeOver = potentialTiles.SelectRandom();
            tileToTakeOver.Owner = TileOwner.Enemy; // Claim the tile for the enemy
        }

        private WaveConfig SelectWave()
        {
            var minDifficulty = _waves.Min(w => w.DifficultyLevel);
            var maxDifficulty = _waves.Max(w => w.DifficultyLevel);

            var currentTurn = ServiceLocator.Get<CombatManager>().CurrentTurn;
            var turnProgress = (float)currentTurn / _combatLength;

            var relativeDifficulty = _difficultyCurve.Evaluate(turnProgress);
            var mappedDifficulty = Mathf.Lerp(minDifficulty, maxDifficulty, relativeDifficulty);

            var wavesWithWeights = _waves
                .Select(wave =>
                {
                    var weight =
                        1f / (1f + Mathf.Abs(wave.DifficultyLevel - mappedDifficulty));
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

        private void SpawnWave(WaveConfig wave)
        {
            var unitsToSpawn = new List<PawnData>(_waitList);
            _waitList.Clear();
            unitsToSpawn.AddRange(wave.Enemies);

            foreach (var pawnData in unitsToSpawn)
            {
                var tile = FindValidTileForUnit(pawnData);
                if (tile != null)
                    PawnFactory.CreatePawn(pawnData, tile);
                else
                    _waitList.Enqueue(pawnData); // Add to waitlist if no tile is found
            }
        }

        private Tile FindValidTileForUnit(PawnData pawnData)
        {
            var enemyOwnedTiles = Tilemap.GetEnemyOwnedTiles()
                .OrderByDescending(tile => tile.Position.x)
                .ToList();

            foreach (var tile in enemyOwnedTiles)
                if (IsFootprintAvailable(tile, pawnData.Size))
                    return tile;

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
            _remainingWaves = _waves.OrderBy(w => w.DifficultyLevel).ToList();
            _waitList.Clear();
        }

        public void StopSpawning()
        {
            _finalWave = null;
            _wavesSpawned = 0;
            _remainingWaves.Clear();
            _waitList.Clear();
        }
    }
}