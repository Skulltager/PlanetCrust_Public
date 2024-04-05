using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarManager : MonoBehaviour
{
    [SerializeField] private EnemyHealthBar healthBarPrefab = default;
    [SerializeField] private float destructionDelay = default;

    private readonly List<EnemyHealthBar> healthBars;
    private int healthBarsUsed;

    public RectTransform rectTransform { private set; get; }

    private EnemyHealthBarManager()
    {
        healthBars = new List<EnemyHealthBar>();
    }

    public void Initialize()
    {
        rectTransform = transform as RectTransform;
    }

    private void OnEnable()
    {
        ShipData.allShipDatas.onAdd += OnAdd_Ship;
        ShipData.allShipDatas.onRemove += OnRemove_Ship;
        foreach (ShipData shipData in ShipData.allShipDatas)
            OnAdd_Ship(shipData);
    }

    public void Reset()
    {
        foreach (EnemyHealthBar healthBar in healthBars)
            GameObject.Destroy(healthBar.gameObject);
        healthBarsUsed = 0;
        healthBars.Clear();

        StopAllCoroutines();
    }

    private void OnAdd_Ship(ShipData shipData)
    {
        shipData.showHealthBar.onValueChangeSource += OnValueChanged_ShowHealthBar;
        if(shipData.showHealthBar.value)
            SetHealthBar(shipData);

    }

    private void OnValueChanged_ShowHealthBar(ShipData source, bool oldValue, bool newValue)
    {
        if (newValue)
            SetHealthBar(source);
        else if (!newValue)
            FreeUpHealthBar(source);

    }    

    private void OnRemove_Ship(ShipData shipData)
    {
        shipData.showHealthBar.onValueChangeSource -= OnValueChanged_ShowHealthBar;
        if (shipData.showHealthBar.value && isActiveAndEnabled)
            StartCoroutine(Routine_FreeUpHealthBar(shipData));
    }

    private void SetHealthBar(ShipData shipData)
    {
        if (healthBars.Count == healthBarsUsed)
        {
            EnemyHealthBar instance = GameObject.Instantiate(healthBarPrefab, transform);
            healthBars.Add(instance);
        }

        EnemyHealthBar healthBar = healthBars[healthBarsUsed];
        healthBar.poolingIndex = healthBarsUsed;
        healthBar.gameObject.SetActive(true);
        healthBar.shipData.value = shipData;
        healthBarsUsed++;
    }

    private IEnumerator Routine_FreeUpHealthBar(ShipData shipData)
    {
        yield return new WaitForSeconds(destructionDelay);
        FreeUpHealthBar(shipData);
    }

    private void FreeUpHealthBar(ShipData shipData)
    {
        EnemyHealthBar healthBar = healthBars.Find(i => i.shipData.value == shipData);
        healthBar.ReturnToPool();
        healthBar.shipData.value = null;
        healthBar.gameObject.SetActive(false);
        healthBarsUsed--;

        if (healthBar.poolingIndex == healthBarsUsed)
            return;

        EnemyHealthBar temp = healthBars[healthBarsUsed];
        healthBars[healthBarsUsed] = healthBar;
        temp.poolingIndex = healthBar.poolingIndex;
        healthBars[temp.poolingIndex] = temp;
    }

    private void OnDisable()
    {
        ShipData.allShipDatas.onAdd -= OnAdd_Ship;
        ShipData.allShipDatas.onRemove -= OnRemove_Ship;
        foreach (ShipData shipData in ShipData.allShipDatas)
            OnRemove_Ship(shipData);
    }
}
