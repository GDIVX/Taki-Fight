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
        private readonly Queue<PawnData> _waitList = new();

        private bool _canSpawn = true; // Flag to control spawning
        private int _combatLength;

        private AnimationCurve _difficultyCurve;
        private WaveConfig _finalWave;
        private List<WaveConfig> _remainingWaves;

        private int _turnsDelayBetweenWaves = 1; // Configurable turn delay (default: 1)
        private int _turnsSinceLastWave; // New field to track the delay since the last wave
        private List<WaveConfig> _waves;
        private int _wavesSpawned;

        private PawnFactory _pawnFactory => ServiceLocator.Get<PawnFactory>();
        private TilemapController _tilemap => ServiceLocator.Get<TilemapController>();

        public void Init(CombatConfig combatConfig)
        {
            _combatLength = combatConfig.CombatLength;
            _waves = combatConfig.Waves;
            _finalWave = combatConfig.FinalWave;
            _difficultyCurve = combatConfig.DifficultyCurve;
            _turnsDelayBetweenWaves = combatConfig.TurnsDelayBetweenWaves; // Initialize turn delay

            ResetState();
            _canSpawn = true; // Enable spawning when initialized
        }

        public void TrySpawnWave()
        {
            if (!_canSpawn) return; // Stop if spawning is disabled 

            _turnsSinceLastWave++;

            if (_turnsSinceLastWave <= _turnsDelayBetweenWaves) return;

            _turnsSinceLastWave = 0;

            // Check if it's time to spawn the final wave
            if (_wavesSpawned >= _combatLength && _finalWave != null)
            {
                SpawnWave(_finalWave); // Spawn the final wave
                _finalWave = null; // Ensure it's only spawned once
                return;
            }

            // Select and spawn a random wave with difficulty bias
            var waveToSpawn = SelectWave();
            if (waveToSpawn != null)
            {
                SpawnWave(waveToSpawn);
                _wavesSpawned++;
            }
        }

        public void StopSpawning()
        {
            _canSpawn = false; // Disable spawning
        }

        private WaveConfig SelectWave()
        {
            // Step 1: Get the range of wave difficulties
            var minDifficulty = _waves.Min(w => w.DifficultyLevel);
            var maxDifficulty = _waves.Max(w => w.DifficultyLevel);

            // Step 2: Calculate turn progress (X-axis: 0 to 1)
            var currentTurn = ServiceLocator.Get<CombatManager>().CurrentTurn;
            var turnProgress = (float)currentTurn / _combatLength;

            // Step 3: Evaluate the curve (Y-axis: 0 to 1)
            var relativeDifficulty = _difficultyCurve.Evaluate(turnProgress);

            // Step 4: Map relative difficulty to actual difficulties
            var mappedDifficulty = Mathf.Lerp(minDifficulty, maxDifficulty, relativeDifficulty);

            // Step 5: Assign weights and select a wave based on weighted randomness
            var wavesWithWeights = _waves
                .Select(wave =>
                {
                    var weight =
                        1f / (1f + Mathf.Abs(wave.DifficultyLevel - mappedDifficulty)); // Closer = heavier weight
                    return (wave, weight);
                })
                .ToList();

            var totalWeight = wavesWithWeights.Sum(w => w.weight);
            var randomValue = Random.value * totalWeight;

            foreach (var (wave, weight) in wavesWithWeights)
            {
                if (randomValue < weight) return wave; // Return the wave matching the random selection

                randomValue -= weight;
            }

            return null; // Fallback, should never reach here if `_waves` is defined
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
                    _pawnFactory.CreatePawn(pawnData, tile);
                else
                    _waitList.Enqueue(pawnData);
            }
        }

        private Tile FindValidTileForUnit(PawnData pawnData)
        {
            var enemyOwnedTiles = _tilemap.GetEnemyOwnedTiles()
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
            for (var y = 0; y < size.y; y++)
            {
                var tile = _tilemap.GetTile(startTile.Position + new Vector2Int(x, y));
                if (tile == null || !tile.IsEmpty || tile.Owner != TileOwner.Enemy) return false;
            }

            return true;
        }

        private void AddWaveToWaitList(WaveConfig wave)
        {
            foreach (var pawnData in wave.Enemies) _waitList.Enqueue(pawnData);
        }

        private void ResetState()
        {
            _wavesSpawned = 0;
            _turnsSinceLastWave = 0; // Reset the turns delay counter
            _remainingWaves = _waves.OrderBy(w => w.DifficultyLevel).ToList();
            _waitList.Clear();
        }
    }
}