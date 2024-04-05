
using UnityEngine;
using UnityEngine.UI;

public class HUDEnergyBar : DataDrivenUI<ShipData_Energy>
{
    [SerializeField] private Text energyAmount = default;
    [SerializeField] private Image energyImage = default;

    protected override void OnValueChanged_Data(ShipData_Energy oldValue, ShipData_Energy newValue)
    {
        if (oldValue != null)
        {
            oldValue.energy.onValueChange -= OnValueChanged_Energy;
            oldValue.maxEnergy.onValueChange -= OnValueChanged_MaxEnergy;
        }
        if (newValue != null)
        {
            newValue.energy.onValueChange += OnValueChanged_Energy;
            newValue.maxEnergy.onValueChange += OnValueChanged_MaxEnergy;
            UpdateBar();
        }
    }

    private void OnValueChanged_Energy(float oldValue, float newValue)
    {
        UpdateBar();
    }

    private void OnValueChanged_MaxEnergy(int oldValue, int newValue)
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        energyImage.fillAmount = data.energy.value / data.maxEnergy.value;
        int energy = (int)data.energy.value;
        int maxEnergy = data.maxEnergy.value;
        energyAmount.text = string.Format("{0} / {1}", energy, maxEnergy);
    }
}
