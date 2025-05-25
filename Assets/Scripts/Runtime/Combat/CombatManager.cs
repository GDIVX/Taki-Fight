using System;
using System.Collections;
using System.Linq;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat.Pawn;
using Runtime.Combat.Spawning;
using Runtime.Combat.Tilemap;
using Runtime.UI.OnScreenMessages;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Runtime.Combat
{
    public class CombatManager : MonoService<CombatManager>
    {
        [SerializeField, BoxGroup("Settings")] private bool _discardHandOnTurnEnd = true;
        [SerializeField, BoxGroup("Settings")] private TilemapConfig _tilemapConfig;
        [SerializeField, Required] private Button _endTurnBtn;
        [SerializeField] [Required] private TilemapView _tilemapView;
        [SerializeField, BoxGroup("Settings")] private CombatConfig _combatConfig; // Temporary field for testing

        [SerializeField] [BoxGroup("Objective")]
        private PawnData _defenseObjectiveData;

        private EnemiesWavesManager _wavesManager;
        public int CurrentTurn { get; private set; }

        private CastleHealthManager DefenseCastle { get; set; }

        private static GameManager GameManager => ServiceLocator.Get<GameManager>();

        private TilemapController Tilemap { get; set; }

        public event Action OnStartTurn, OnEndTurn, OnCombatStart;

        internal void Init()
        {
            // Initialize the waves manager
            _wavesManager = new EnemiesWavesManager();
            ServiceLocator.Register(_wavesManager);

            //end turn btn
            _endTurnBtn.onClick.AddListener(EndTurn);

            //defense target
            DefenseCastle = ServiceLocator.Get<CastleHealthManager>().Init(_defenseObjectiveData);


            // Recreate TilemapController with the fresh scene tilemap view
            TilemapGenerator.GenerateTilemap(_tilemapConfig, _tilemapView, tiles =>
            {
                Tilemap = new TilemapController(tiles, _tilemapView);

                // Initialize the defense pawn
                DefenseCastle.CreatePawn(new Vector2Int(0, 1));

                // Start combat when ready
                StartCombat();
            });
        }

        [Button]
        public void StartCombat()
        {
            if (_combatConfig == null)
            {
                Debug.LogError("CombatConfig is not assigned!");
                return;
            }

            CurrentTurn = 0;

            //Place the defense objective
            DefenseCastle.CreatePawn(new Vector2Int(0, 0));

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
            DefenseCastle?.RemovePawn();
            Tilemap?.Clear(); // Clear any tilemap references

            // Ensure TilemapController reference is removed
            ServiceLocator.Unregister(Tilemap);
        }


        private void StartTurn()
        {
            CurrentTurn++;

            ServiceLocator.Get<MessageManager>().ShowMessage($"Turn {CurrentTurn}", MessageType.Notification);

            // Check if the player has cards to play
            var handController = ServiceLocator.Get<HandController>();
            handController.DrawHand();

            // Automatically skip if no cards
            if (handController.HandIsEmpty())
            {
                Debug.Log("No cards left – skipping turn.");
                EndTurn();
                return;
            }

            ServiceLocator.Get<Energy>().GainEnergyPerIncome();

            // If this is the first turn, skip the wave spawning
            if (CurrentTurn != 1)
                // Try spawning a wave at the start of the turn
                _wavesManager.OnTurnStart();

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