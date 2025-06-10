using TMPro;
using UnityEngine;

namespace Runtime.CardGameplay.Energy
{
    public class EnergyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _amountText, _incomeText;

        public void Initialize(Energy energy)
        {
            _amountText.text = energy.Amount.ToString();
            _incomeText.text = FormatIncomeText(energy.Income);

            energy.OnAmountChanged += amount => _amountText.text = amount.ToString();
            energy.OnIncomeChanged += income => FormatIncomeText(income);
        }

        private string FormatIncomeText(int income)
        {
            return _incomeText.text = $"+{income}";
        }
    }
}
