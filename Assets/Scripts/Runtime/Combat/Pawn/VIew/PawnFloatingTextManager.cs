using DamageNumbersPro;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Combat.Pawn.VIew
{
    public class PawnFloatingTextManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("Damage")] private DamageNumber _damageNumberPrefab;

        public void ShowDamageNumber(int rawDamage, float currentHealth)
        {
            var floatingText = _damageNumberPrefab.Spawn(transform.position, rawDamage, transform);

            //set the color and size in relation to the current health
            floatingText.scaleByNumberSettings.toNumber = currentHealth;
            floatingText.colorByNumberSettings.toNumber = currentHealth;
        }
    }
}