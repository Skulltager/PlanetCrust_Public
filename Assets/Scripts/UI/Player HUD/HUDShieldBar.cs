
using UnityEngine;
using UnityEngine.UI;

public class HUDShieldBar : DataDrivenUI<ShipData_Health>
{
    [SerializeField] private Text shieldAmount = default;
    [SerializeField] private Image shieldImage = default;

    protected override void OnValueChanged_Data(ShipData_Health oldValue, ShipData_Health newValue)
    {
        if (oldValue != null)
        {
            oldValue.shield.onValueChange -= OnValueChanged_Shield;
            oldValue.maxShield.onValueChange -= OnValueChanged_MaxShield;
        }
        if (newValue != null)
        {
            newValue.shield.onValueChange += OnValueChanged_Shield;
            newValue.maxShield.onValueChange += OnValueChanged_MaxShield;
            UpdateBar();
        }
    }

    private void OnValueChanged_Shield(int oldValue, int newValue)
    {
        UpdateBar();
    }

    private void OnValueChanged_MaxShield(int oldValue, int newValue)
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        shieldImage.fillAmount = (float)data.shield.value / data.maxShield.value;
        shieldAmount.text = string.Format("{0} / {1}", data.shield.value, data.maxShield.value);
    }
}
