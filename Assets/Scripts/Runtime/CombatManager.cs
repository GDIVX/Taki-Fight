using System;
using System.Collections.Generic;
using Runtime.Combat;
using Runtime.Combat.Pawn;
using Runtime.Combat.Pawn.Enemy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime
{
    public class CombatManager
    {
        private GameManager _gameManager;
        [SerializeField, Required] private CombatLane _enemiesLane;

        [SerializeField, TabGroup("Dependencies"), Required]
        private PawnController _heroPawn;

        public CombatManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public PawnController Hero
        {
            get => _heroPawn;
            set => _heroPawn = value;
        }

        public CombatLane Enemies => _enemiesLane;

        private void SetupOnEnemyRemove()
        {
            _enemiesLane.OnPawnRemoved += () =>
            {
                if (!_heroPawn.isActiveAndEnabled) return;
                if (_enemiesLane.Pawns.Count > 0) return;

                _gameManager.BannerViewManager.WriteMessage(0, "Victory", Color.green);
                _gameManager.GameOver();
            };
        }

        private void InitializeHero(PawnData data)
        {
            _heroPawn.gameObject.SetActive(true);
            _heroPawn.Init(data);
            _heroPawn.Health.OnDead += (sender, args) =>
            {
                _gameManager.BannerViewManager.WriteMessage(0, "Defeat", Color.red);
                _gameManager.GameOver();
            };
        }

        [Button]
        public void StartCombat(CombatConfig combatConfig)
        {
            _enemiesLane.SpawnPawnsForCombat(combatConfig);
            _gameManager.SetupCardGameplay();
        }

        [Button]
        public void EndCombat()
        {
            _enemiesLane.Clear();
            _heroPawn.gameObject.SetActive(false);
        }

        private void StartTurn()
        {
            _gameManager.BannerViewManager.WriteMessage(1, "Player Turn", Color.white);
            _gameManager.GemsBag.OnTurnStart(() =>
            {
                _gameManager.BannerViewManager.Clear();
                _heroPawn.OnTurnStart();
                SetupEnemies();

                _gameManager.Hand.DrawHand();
            });
        }

        [Button]
        public void EndTurn()
        {
            _heroPawn.OnTurnEnd();
            _gameManager.GemsBag.OnTurnEnd();

            PlayEnemiesTurn(() =>
            {
                //Reset the player defense 
                Hero.Defense.Value = 0;
                _gameManager.BannerViewManager.Clear();
                StartTurn();
            });
        }

        private void SetupEnemies()
        {
            foreach (var enemy in _enemiesLane.Pawns)
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
            if (Hero.Health.IsDead()) return;

            _gameManager.BannerViewManager.WriteMessage(1, "Enemies Turn", Color.yellow);

            var enemies = new Queue<PawnController>(_enemiesLane.Pawns);
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