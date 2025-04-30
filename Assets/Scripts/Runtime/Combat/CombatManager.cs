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

namespace Runtime.Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings")] private Vector2Int _arenaSize;
        [SerializeField, BoxGroup("Settings")] private bool _discardHandOnTurnEnd = true;
        [SerializeField, Required] Button _endTurnBtn;
        [SerializeField, Required] private TilemapView _arenaView;

        private static GameManager GameManager => GameManager.Instance;

        public TilemapController Tilemap { get; private set; }
        public event Action OnStartTurn, OnEndTurn, OnCombatStart;

        internal void Init()
        {
            Tilemap = new TilemapController(_arenaSize.x, _arenaSize.y, _arenaView);
            ServiceLocator.Register(Tilemap);
            _endTurnBtn.onClick.AddListener(EndTurn);
        }

        [Button]
        public void StartCombat(CombatConfig combatConfig)
        {
            Tilemap.Enable();
            GameManager.OnCombatStart();
            StartTurn();
            OnCombatStart?.Invoke();
        }

        [Button]
        public void EndCombat()
        {
            Tilemap.Clear();
        }

        private void StartTurn()
        {
            var bannerView = ServiceLocator.Get<BannerViewManager>();
            bannerView.WriteMessage(1, "Player Turn", Color.white);
            bannerView.Clear();

            ServiceLocator.Get<HandController>().DrawHand();
            ServiceLocator.Get<Energy>().GainEnergyPerIncome();

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