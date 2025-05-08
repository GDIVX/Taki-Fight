using Runtime.Combat.Pawn;
using Runtime.Combat.Spawning;
using Runtime.Combat.Tilemap;
using Runtime.Combat;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class EnemiesWavesManager 
{
    private AnimationCurve _difficultyCurve;
    private List<WaveConfig> _waves;
    private WaveConfig _finalWave;
    private int _combatLength;

    private int _currentTurn;
    private int _wavesSpawned;
    private List<WaveConfig> _remainingWaves;
    private readonly Queue<PawnData> _waitList = new();

    private bool _canSpawn = true; // Flag to control spawning

    private PawnFactory _pawnFactory => ServiceLocator.Get<PawnFactory>();
    private TilemapController _tilemap => ServiceLocator.Get<TilemapController>();

    public void Init(CombatConfig combatConfig)
    {
        _combatLength = combatConfig.CombatLength;
        _waves = combatConfig.Waves;
        _finalWave = combatConfig.FinalWave;
        _difficultyCurve = combatConfig.DifficultyCurve;

        ResetState();
        _canSpawn = true; // Enable spawning when initialized
    }

    public void TrySpawnWave()
    {
        if (!_canSpawn) return; // Stop if spawning is disabled

        _currentTurn++;

        if (_wavesSpawned >= _combatLength && _finalWave != null)
        {
            AddWaveToWaitList(_finalWave);
            _finalWave = null;
        }

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
        int desiredDifficulty = Mathf.RoundToInt(_difficultyCurve.Evaluate(_currentTurn));
        var matchingWaves = _waves.Where(w => w.DifficultyLevel == desiredDifficulty).ToList();

        if (matchingWaves.Count == 0)
        {
            matchingWaves = _waves;
        }

        return matchingWaves[UnityEngine.Random.Range(0, matchingWaves.Count)];
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
            {
                _pawnFactory.CreatePawn(pawnData, tile);
            }
            else
            {
                _waitList.Enqueue(pawnData);
            }
        }
    }

    private Tile FindValidTileForUnit(PawnData pawnData)
    {
        var enemyOwnedTiles = _tilemap.GetEnemyOwnedTiles()
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
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var tile = _tilemap.GetTile(startTile.Position + new Vector2Int(x, y));
                if (tile == null || !tile.IsEmpty || tile.Owner != TileOwner.Enemy)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void AddWaveToWaitList(WaveConfig wave)
    {
        foreach (var pawnData in wave.Enemies)
        {
            _waitList.Enqueue(pawnData);
        }
    }

    private void ResetState()
    {
        _currentTurn = 0;
        _wavesSpawned = 0;
        _remainingWaves = _waves.OrderBy(w => w.DifficultyLevel).ToList();
        _waitList.Clear();
    }
}
