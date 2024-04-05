
using FMOD;
using System;
using Unity.Collections;
using Unity.Entities;

public class ShipData
{
    public static EventList<ShipData> allShipDatas { private set; get; }
    public static EventVariable<ShipData, ShipData> controllingShip { private set; get; }
    public static NativeList<Entity> allShipEntities { private set; get; }

    public readonly EventList<WeaponData> weaponDatas;
    public readonly EventVariable<ShipData, bool> showHealthBar;

    public readonly ShipData_Transform transformData;
    public readonly ShipData_Health healthData;
    public readonly ShipData_Velocity velocityData;
    public readonly ShipData_Energy energyData;
    public readonly ShipData_TargetPosition targetPositionData;

    public readonly Entity entity;
    public int index { private set; get; }

    public event Action<ShipEventData_TakeDamage> onTakeDamage;

    public static void Initialize()
    {
        allShipEntities = new NativeList<Entity>(Allocator.Persistent);
        controllingShip = new EventVariable<ShipData, ShipData>(null, null);
        allShipDatas = new EventList<ShipData>();
        controllingShip.onValueChange += OnValueChanged_ControllingShip;
    }

    public void TriggerOnTakeDamage(ShipEventData_TakeDamage eventData)
    {
        if (onTakeDamage != null)
            onTakeDamage(eventData);
    }

    public ShipData(Entity entity)
    {
        this.entity = entity;
        transformData = new ShipData_Transform();
        healthData = new ShipData_Health();
        velocityData = new ShipData_Velocity();
        energyData = new ShipData_Energy();
        targetPositionData = new ShipData_TargetPosition();
        showHealthBar = new EventVariable<ShipData, bool>(this, true);

        weaponDatas = new EventList<WeaponData>();
        index = allShipDatas.Count;
        allShipDatas.Add(this);
        allShipEntities.Add(entity);
        allShipDatas.onRemove += OnRemove_ShipData;
    }

    private static void OnValueChanged_ControllingShip(ShipData oldValue, ShipData newValue)
    {
        if(oldValue != null)
            oldValue.showHealthBar.value = true;

        if (newValue != null)
            newValue.showHealthBar.value = false;
    }

    private void OnRemove_ShipData(ShipData item)
    {
        if (item.index > index)
            return;

        index--;
    }
    
    public void Destroy()
    {
        healthData.hull.value = 0;
        healthData.armor.value = 0;
        healthData.shield.value = 0;
        energyData.energy.value = 0;

        if (controllingShip.value == this)
            controllingShip.value = null;

        allShipDatas.onRemove -= OnRemove_ShipData;

        allShipEntities.RemoveAt(index);
        allShipDatas.Remove(this);
    }

    public static void Cleanup()
    {
        controllingShip.onValueChange -= OnValueChanged_ControllingShip;
    }
}