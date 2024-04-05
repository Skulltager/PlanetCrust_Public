
using UnityEngine;
using UnityEngine.UI;

public class HUDHullBar : DataDrivenUI<ShipData_Health>
{
    [SerializeField] private Text hullAmount = default;
    [SerializeField] private Image hullImage = default;

    protected override void OnValueChanged_Data(ShipData_Health oldValue, ShipData_Health newValue)
    {
        if (oldValue != null)
        {
            oldValue.hull.onValueChange -= OnValueChanged_Hull;
            oldValue.maxHull.onValueChange -= OnValueChanged_MaxHull;
        }
        if (newValue != null)
        {
            newValue.hull.onValueChange += OnValueChanged_Hull;
            newValue.maxHull.onValueChange += OnValueChanged_MaxHull;
            UpdateBar();
        }
    }

    private void OnValueChanged_Hull(int oldValue, int newValue)
    {
        UpdateBar();
    }

    private void OnValueChanged_MaxHull(int oldValue, int newValue)
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        hullImage.fillAmount = (float)data.hull.value / data.maxHull.value;
        hullAmount.text = string.Format("{0} / {1}", data.hull.value, data.maxHull.value);
    }
}
