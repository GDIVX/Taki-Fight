using System;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Combat.Tilemap;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Energy;
using Runtime.Combat.Tilemap;
using Runtime.Combat.Pawn;
using Runtime.Events;
using Runtime.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat
{
    public class CombatManager : MonoBehaviour
    {

        [SerializeField, BoxGroup("Settings")] private Vector2Int _arenaSize;
        [SerializeField, BoxGroup("Settings")] private bool _discardHandOnTurnEnd = true;
        [SerializeField, Required] TilemapView _arenaView;

        private static GameManager GameManager => GameManager.Instance;

        public TilemapController ArenaController { get; private set; }
        public event Action OnStartTurn, OnEndTurn, OnCombatStart;


        internal void Init()
        {
            ArenaController = new TilemapController(_arenaSize.x, _arenaSize.y, _arenaView);
            //ArenaController.Clear();
            ServiceLocator.Register(ArenaController);
        }

        [Button]
        public void StartCombat(CombatConfig combatConfig)
        {
            ArenaController.Enable();
            GameManager.OnCombatStart();
            StartTurn();
            OnCombatStart?.Invoke();
        }

        [Button]
        public void EndCombat()
        {
            ArenaController.Clear();

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
            ServiceLocator.Get<Energy>().Clear();
            if (_discardHandOnTurnEnd)
            {
                ServiceLocator.Get<HandController>().DiscardHand();
            }

            OnEndTurn?.Invoke();

            //ArenaController.PlayTurn(StartTurn);
            //TODO have a dedicated turn manager
        }

    }
}