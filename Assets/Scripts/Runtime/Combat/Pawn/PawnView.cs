using CodeMonkey.HealthSystemCM;
using Sirenix.OdinInspector;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Runtime.Combat.Pawn
{
    public class PawnView : MonoBehaviour
    {
        [SerializeField, BoxGroup("Animation")]
        private Animator animator;

        [SerializeField, BoxGroup("Health")] private Image defenseImage;
        [SerializeField, BoxGroup("Health")] private TextMeshProUGUI defenseCount;

        [SerializeField, BoxGroup("Health")] private HealthBarUI healthBar;
        [SerializeField, BoxGroup("Health")] private HealthBarTextUI healthBarText;
        [SerializeField, BoxGroup("Sprite")] private SpriteRenderer spriteRenderer;

        public void Init(HealthSystem healthSystem, TrackedProperty<int> defense, PawnData data)
        {
            healthBar.SetHealthSystem(healthSystem);
            healthBarText.SetHealthSystem(healthSystem);

            defense.OnValueChanged += UpdateDefenseUI;
            UpdateDefenseUI(defense.Value);

            spriteRenderer.sprite = data.Sprite;
        }

        private void UpdateDefenseUI(int defensePoints)
        {
            defenseImage.gameObject.SetActive(defensePoints != 0);
            defenseCount.text = defensePoints.ToString();
        }
    }
}