using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Enemy;
using Runtime.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Runtime.Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField, Required] private PawnFactory _pawnFactory;
        [SerializeField, Required] private Battlefield _battlefield;
        private static GameManager GameManager => GameManager.Instance;

        public Battlefield Battlefield => _battlefield;
        public event Action OnStartTurn, OnEndTurn;
        private EventListener<GameStateEvent> _onGameStateChanged;

        public void OnEnable()
        {
            _onGameStateChanged = new EventListener<GameStateEvent>(e =>
            {
                switch (e.GameState)
                {
                    case GameState.Menu:
                        break;
                    case GameState.RunStart:
                        break;
                    case GameState.Pause:
                        break;
                    case GameState.Exploration:
                        break;
                    case GameState.RoomEntered:
                        break;
                    case GameState.RoomResolved:
                        break;
                    case GameState.Combat:
                        break;
                    case GameState.CombatEnd:
                        EndCombat();
                        break;
                    case GameState.RunEnd:
                        EndCombat();
                        break;
                    case GameState.Hub:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void OnDisable()
        {
            _onGameStateChanged.Disable();
        }

        private void Awake()
        {
            ServiceLocator.Register(_pawnFactory);
        }

        public void InitializeHero(PawnData data)
        {
            //TEMPT for testing
            var hero = Battlefield.GetLaneSide(true, 0).AddPawn(data);
            hero.Health.OnDead += (sender, args) =>
            {
                GameManager.BannerViewManager.WriteMessage(0, "Defeat", Color.red);
                GameManager.SetGameState(GameState.RunEnd);
            };
        }

        [Button]
        public void StartCombat(CombatConfig combatConfig)
        {
            //TEMPT for testing
            Battlefield.GetLaneSide(false, 0).SpawnPawnsForCombat(combatConfig, () =>
            {
                GameManager.OnCombatStart();
                StartTurn();
                GameManager.Instance.SetGameState(GameState.Combat);
            });
        }

        [Button]
        public void EndCombat()
        {
            Battlefield.Clear(false);

            //Clear all status effects from the player
            Battlefield.Hero.ClearStatusEffects();
        }

        private void StartTurn()
        {
            GameManager.BannerViewManager.WriteMessage(1, "Player Turn", Color.white);
            GameManager.BannerViewManager.Clear();
            SetupEnemies();

            GameManager.Hand.DrawHand();

            OnStartTurn?.Invoke();
        }

        [Button]
        public void EndTurn()
        {
            GameManager.GemsBag.Clear();
            OnEndTurn?.Invoke();

            PlayEnemiesTurn(() =>
            {
                //Reset the player defense 
                GameManager.BannerViewManager.Clear();
                StartTurn();
            });
        }

        private void SetupEnemies()
        {
            //TEMPT
            foreach (var enemy in Battlefield.GetLaneSide(false, 0).Pawns)
            {
                if (enemy is EnemyController enemyController)
                {
                    enemyController.ChoosePlayStrategy();
                }
                else
                {
                    Debug.LogError($"Failed to cast enemy {enemy} into interface {nameof(EnemyController)}");
                }
            }
        }

        private void PlayEnemiesTurn(Action onComplete)
        {
            GameManager.BannerViewManager.WriteMessage(1, "Enemies Turn", Color.yellow);

            //TODO:
            var enemies = new Queue<PawnController>(Battlefield.GetLaneSide(false, 0).Pawns);
            ProcessNextEnemy(enemies, onComplete);
        }

        private void ProcessNextEnemy(Queue<PawnController> enemies, Action onComplete)
        {
            if (enemies.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            var enemy = enemies.Dequeue();
            enemy.Defense.Value = 0;

            if (enemy is EnemyController enemyController)
            {
                enemyController.PlayTurn(() => ProcessNextEnemy(enemies, onComplete));
            }
            else
            {
                Debug.LogError($"Failed to cast enemy {enemy} into interface {nameof(EnemyController)}");
                ProcessNextEnemy(enemies, onComplete);
            }
        }
    }
}