
using System.Collections.Generic;
using UnityEngine;

public class WeaponIndicatorManager : DataDrivenUI<ShipData>
{
    [SerializeField] private WeaponIndicator weaponIndicatorPrefab = default;
    [SerializeField] private Transform weaponIndicatorParent = default;

    private readonly List<WeaponIndicator> weaponIndicatorInstances;

    private WeaponIndicatorManager()
    {
        weaponIndicatorInstances = new List<WeaponIndicator>();
    }

    protected override void OnValueChanged_Data(ShipData oldValue, ShipData newValue)
    {
        if(oldValue != null)
        {
            oldValue.weaponDatas.onAdd -= OnAdd_WeaponData;
            oldValue.weaponDatas.onRemove -= OnRemove_WeaponData;

            foreach (WeaponData weaponData in oldValue.weaponDatas)
                OnRemove_WeaponData(weaponData);
        }

        if(newValue != null)
        {
            newValue.weaponDatas.onAdd += OnAdd_WeaponData;
            newValue.weaponDatas.onRemove += OnRemove_WeaponData;

            foreach (WeaponData weaponData in newValue.weaponDatas)
                OnAdd_WeaponData(weaponData);
        }
    }

    private void OnAdd_WeaponData(WeaponData item)
    {
        for(int i = 0; i < item.transformDatas.Length; i++)
        {
            WeaponData_Transform transformData = item.transformDatas[i];
            WeaponIndicator instance = GameObject.Instantiate(weaponIndicatorPrefab, weaponIndicatorParent);
            instance.data = transformData;
            weaponIndicatorInstances.Add(instance);
        }
    }

    private void OnRemove_WeaponData(WeaponData item)
    {
        for (int i = 0; i < item.transformDatas.Length; i++)
        {
            WeaponData_Transform transformData = item.transformDatas[i];
            WeaponIndicator instance = weaponIndicatorInstances.Find(i => i.data == transformData);
            weaponIndicatorInstances.Remove(instance);
            GameObject.Destroy(instance.gameObject);
        }
    }

    public void UpdateIndicators()
    {
        foreach (WeaponIndicator weaponIndicator in weaponIndicatorInstances)
            weaponIndicator.UpdateIndicator();
    }
}