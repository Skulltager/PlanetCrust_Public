
using UnityEngine;
using UnityEngine.UI;

public class ShipArmorBar : DataDrivenUI<ShipData_Health>
{
    [SerializeField] private Image armorImage = default;

    protected override void OnValueChanged_Data(ShipData_Health oldValue, ShipData_Health newValue)
    {
        if (oldValue != null)
        {
            oldValue.armor.onValueChange -= OnValueChanged_Armor;
            oldValue.maxArmor.onValueChange -= OnValueChanged_MaxArmor;
        }
        if (newValue != null)
        {
            newValue.armor.onValueChange += OnValueChanged_Armor;
            newValue.maxArmor.onValueChange += OnValueChanged_MaxArmor;
            UpdateBar();
        }
    }

    private void OnValueChanged_Armor(int oldValue, int newValue)
    {
        UpdateBar();
    }

    private void OnValueChanged_MaxArmor(int oldValue, int newValue)
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        armorImage.fillAmount = (float)data.armor.value / data.maxArmor.value;
    }
}
