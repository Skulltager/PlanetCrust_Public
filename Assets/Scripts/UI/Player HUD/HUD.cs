
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD instance { private set; get; }

    [SerializeField] private WeaponIndicatorManager weaponIndicatorManager = default;
    [SerializeField] private HUDShieldBar shieldBar = default;
    [SerializeField] private HUDArmorBar armorBar = default;
    [SerializeField] private HUDHullBar hullBar = default;
    [SerializeField] private HUDEnergyBar energyBar = default;

    public void Initialize()
    {
        instance = this;
    }

    private void Awake()
    {
        ShipData.controllingShip.onValueChangeImmediate += OnValueChanged_ControllingShip;
    }

    private void OnValueChanged_ControllingShip(ShipData oldValue, ShipData newValue)
    {
        if (newValue != null)
        {
            shieldBar.data = newValue.healthData;
            armorBar.data = newValue.healthData;
            hullBar.data = newValue.healthData;
            energyBar.data = newValue.energyData;
        }
        else
        {
            shieldBar.data = null;
            armorBar.data = null;
            hullBar.data = null;
            energyBar.data = null;
        }
        weaponIndicatorManager.data = newValue;
    }

    private void OnDestroy()
    {
        ShipData.controllingShip.onValueChangeImmediate -= OnValueChanged_ControllingShip;
    }
}