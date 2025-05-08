using System;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat.Tilemap;
using Runtime.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using Runtime.Combat.Pawn;
using Runtime.Combat.Spawning;

namespace Runtime.Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings")] private bool _discardHandOnTurnEnd = true;
        [SerializeField, BoxGroup("Settings")] private TilemapConfig _tilemapConfig;
        [SerializeField, Required] private Button _endTurnBtn;
        [SerializeField, Required] private TilemapView _arenaView;
        [SerializeField, BoxGroup("Settings")] private CombatConfig _combatConfig; // Temporary field for testing

        private static GameManager GameManager => GameManager.Instance;

        public TilemapController Tilemap { get; private set; }
        private EnemiesWavesManager _wavesManager;

        public event Action OnStartTurn, OnEndTurn, OnCombatStart;

        internal void Init()
        {
            // Initialize the tilemap
            var tiles = TilemapGenerator.GenerateTilemap(_tilemapConfig, _arenaView);
            Tilemap = new TilemapController(tiles, _arenaView);
            ServiceLocator.Register(Tilemap);

            // Initialize the waves manager
            _wavesManager = new EnemiesWavesManager();
            ServiceLocator.Register(_wavesManager);

            _endTurnBtn.onClick.AddListener(EndTurn);
        }

        [Button]
        public void StartCombat()
        {
            if (_combatConfig == null)
            {
                Debug.LogError("CombatConfig is not assigned!");
                return;
            }

            // Initialize the waves manager with the combat config
            _wavesManager.Init(_combatConfig);

            // Enable the tilemap and start combat
            Tilemap.Enable();
            GameManager.OnCombatStart();
            StartTurn();
            OnCombatStart?.Invoke();
        }

        [Button]
        public void EndCombat()
        {
            _wavesManager.StopSpawning(); // Stop spawning waves
            Tilemap.Clear();
        }


        private void StartTurn()
        {
            var bannerView = ServiceLocator.Get<BannerViewManager>();
            bannerView.WriteMessage(1, "Player Turn", Color.white);
            bannerView.Clear();

            ServiceLocator.Get<HandController>().DrawHand();
            ServiceLocator.Get<Energy>().GainEnergyPerIncome();

            // Try spawning a wave at the start of the turn
            _wavesManager.TrySpawnWave();

            OnStartTurn?.Invoke();
        }

        [Button]
        public void EndTurn()
        {
            // Clear energy and discard hand if needed
            ServiceLocator.Get<Energy>().Clear();
            if (_discardHandOnTurnEnd)
            {
                ServiceLocator.Get<HandController>().DiscardHand();
            }

            OnEndTurn?.Invoke();

            // Start the unit phase
            StartCoroutine(ProcessUnitPhase());
        }

        private IEnumerator ProcessUnitPhase()
        {
            // Retrieve all units from the arena
            var units = Tilemap.GetAllUnits();

            // Sort units by turn order
            var sortedUnits = units
                .OrderBy(u => u.Owner == PawnOwner.Enemy) // Enemy units go second
                .ThenBy(u => !u.IsAgile) // Non-agile units go last
                .ToList();

            // Process each unit's turn
            foreach (var unit in sortedUnits)
            {
                unit.OnTurn();

                // Wait until the unit finishes its turn
                yield return new WaitUntil(() => !unit.IsProcessingTurn);
            }

            // After all units have acted, start a new turn
            StartTurn();
        }
    }
}