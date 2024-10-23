using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    public class EnemyController : PawnController
    {
        private AIPlayTable _table;

        public void InitAI(AIPlayTable playTable)
        {
            _table = playTable;
        }


        public IEnumerator ChoseAndPlayStrategy()
        {
            var chosenStrategy = _table.ChoseRandomPlayStrategy();
            chosenStrategy.Play(this);
            yield return new WaitForSeconds(chosenStrategy.Duration);
        }
    }
}