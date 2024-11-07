using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    public class EnemyController : PawnController, IAiBrain
    {
        private AIPlayTable _table;

        public void InitAI(AIPlayTable playTable)
        {
            _table = playTable;
        }


        public IEnumerator ChoseAndPlayStrategy()
        {
            Debug.Log("Stating to play enemy");
            var chosenStrategy = _table.ChoseRandomPlayStrategy();
            chosenStrategy.Play(this);
            yield return new WaitForSeconds(chosenStrategy.Duration);
            Debug.Log($"Enemy {gameObject} had played {chosenStrategy}");
        }
    }
}