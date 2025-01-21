using System;
using System.Collections;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    public class EnemyController : PawnController
    {
        [SerializeField] private IntentionsList _intentionsList;
        private PlayTableEntry _playTableEntry;
        private AIPlayTable _table;

        private int _currPotency = 0;

        public void Init(AIPlayTable playTable)
        {
            _table = playTable;
            Health.OnDead += OnDead;
        }

        private void OnDead(object sender, EventArgs args)
        {
            Destroy(gameObject, WaitBeforeDestroyingObjectOnDeath);
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
            _intentionsList.Add(_playTableEntry.Sprite, _playTableEntry.Color, finalPotency.ToString());
        }

        public IEnumerator PlayTurn()
        {
            OnTurnStart();
            yield return new WaitForSeconds(0.2f);
            var feedbackStrategy = _playTableEntry.FeedbackStrategy;
            if (feedbackStrategy)
            {
                feedbackStrategy.Animate(this, () => _playTableEntry.Strategy.Play(this, _currPotency));
            }
            else
            {
                _playTableEntry.Strategy.Play(this, _currPotency);
            }

            _intentionsList.RemoveNext();
            OnTurnEnd();
        }
    }
}