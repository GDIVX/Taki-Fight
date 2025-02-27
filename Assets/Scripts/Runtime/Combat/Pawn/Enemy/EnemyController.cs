using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    public class EnemyController : PawnController
    {
        [SerializeField] private IntentionsList _intentionsList;
        private PlayTableEntry _playTableEntry;
        private AIPlayTable _table;

        private bool _isPlaying = false;
        private int _currPotency = 0;

        public void Init(AIPlayTable playTable)
        {
            _table = playTable;
            _isPlaying = false;

            _intentionsList ??= GetComponentInChildren<IntentionsList>();
        }


        public void ChoosePlayStrategy()
        {
            _playTableEntry = _table.ChoseRandomPlayStrategy();
            var finalPotency = _playTableEntry.Potency;
            if (_playTableEntry.AddAttackMod)
            {
                finalPotency += AttackModifier.Value;
            }

            if (_playTableEntry.AddDefenseMod)
            {
                finalPotency += DefenseModifier.Value;
            }

            if (_playTableEntry.AddHealingMod)
            {
                finalPotency += HealingModifier.Value;
            }

            _currPotency = finalPotency;
            _intentionsList.Add(_playTableEntry.IntentionType, finalPotency, _playTableEntry.Repeats);
        }

        public void PlayTurn(Action onComplete)
        {
            if (_isPlaying)
            {
                onComplete?.Invoke();
                return;
            }

            try
            {
                _isPlaying = true;
                OnTurnStart();

                var feedbackStrategy = _playTableEntry.FeedbackStrategy;
                for (int i = 0; i < _playTableEntry.Repeats; i++)
                {
                    if (feedbackStrategy)
                    {
                        feedbackStrategy.Animate(this, () => { Play(onComplete); });
                    }
                    else
                    {
                        Play(onComplete);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during enemy turn: {e}");
                _isPlaying = false;
                onComplete?.Invoke();
            }
        }

        private void Play(Action onComplete)
        {
            _playTableEntry.Strategy.Play(this, _currPotency, () =>
            {
                _intentionsList.RemoveNext();
                OnTurnEnd();
                _isPlaying = false;
                onComplete?.Invoke();
            });
        }
    }
}