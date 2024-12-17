using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
            _intentionsList.Add(_playTableEntry.Sprite, _playTableEntry.Color, _playTableEntry.Potency.ToString());
        }

        public IEnumerator PlayTurn()
        {
            OnTurnStart();
            yield return new WaitForSeconds(0.5f);
            _playTableEntry.Strategy.Play(this, _playTableEntry.Potency);
            yield return new WaitForSeconds(_playTableEntry.Strategy.Duration);
            _intentionsList.RemoveNext();
            OnTurnEnd();
        }
    }
}