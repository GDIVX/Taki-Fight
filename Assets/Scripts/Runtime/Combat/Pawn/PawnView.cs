using CodeMonkey.HealthSystemCM;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace Runtime.Combat.Pawn
{
    public class PawnView : MonoBehaviour
    {
        [SerializeField, BoxGroup("Health")] private HealthBarUI healthBar;
        [SerializeField, BoxGroup("Health")] private HealthBarTextUI healthBarText;

        public void InitHealth(HealthSystem healthSystem)
        {
            healthBar.SetHealthSystem(healthSystem);
            healthBarText.SetHealthSystem(healthSystem);
        }
    }
}