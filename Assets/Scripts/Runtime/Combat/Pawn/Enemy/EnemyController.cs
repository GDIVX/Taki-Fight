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

            _intentionsList.Add(_playTableEntry.Sprite, _playTableEntry.Color, finalPotency.ToString());
        }

        public IEnumerator PlayTurn()
        {
            OnTurnStart();
            yield return new WaitForSeconds(0.2f);
            _playTableEntry.Strategy.Play(this, _playTableEntry.Potency);
            _intentionsList.RemoveNext();
            OnTurnEnd();
        }
    }
}