using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField, Required] private PawnController pawnController;
        private AIPlayTable _table;

        public void Init(AIPlayTable playTable)
        {
            _table = playTable;
        }


        public IEnumerator ChoseAndPlayStrategy()
        {
            var chosenStrategy = _table.ChoseRandomPlayStrategy();
            chosenStrategy.Play(pawnController);
            yield return new WaitForSeconds(chosenStrategy.Duration);
        }
    }
}