using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public readonly EventVariable<EnemyHealthBar, ShipData> shipData;

    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private ShipHullBar hullBar = default;
    [SerializeField] private ShipArmorBar armorBar = default;
    [SerializeField] private ShipShieldBar shieldBar = default;
    [SerializeField] private float visibleDuration = default;
    [SerializeField] private float fadeDuration = default;

    [NonSerialized] public int poolingIndex;

    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private float totalVisibleDuration => visibleDuration + fadeDuration;
    private float visibleTimeRemaining;
    public event Action onReturnToPool;

    private EnemyHealthBar()
    {
        shipData = new EventVariable<EnemyHealthBar, ShipData>(this, null);
    }

    private void Awake()
    {
        canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
        shipData.onValueChangeImmediate += OnValueChanged_ShipData;
        rectTransform = transform as RectTransform;
    }

    private void OnValueChanged_ShipData(ShipData oldValue, ShipData newValue)
    {
        if(oldValue != null)
        {
            hullBar.data = null;
            armorBar.data = null;
            shieldBar.data = null;
            oldValue.onTakeDamage -= OnEvent_TakeDamage;
        }

        if(newValue != null)
        {
            hullBar.data = newValue.healthData;
            armorBar.data = newValue.healthData;
            shieldBar.data = newValue.healthData;
            newValue.onTakeDamage += OnEvent_TakeDamage;
        }
    }

    private void OnEvent_TakeDamage(ShipEventData_TakeDamage eventData)
    {
        visibleTimeRemaining = totalVisibleDuration;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (ReferenceEquals(shipData.value, null))
            return;

        visibleTimeRemaining -= Time.deltaTime;

        if(visibleTimeRemaining <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 difference = shipData.value.transformData.position.value - Camera.main.transform.position;

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(shipData.value.transformData.position.value);
        screenPoint.x = screenPoint.x / Screen.width * canvasRectTransform.sizeDelta.x;
        screenPoint.y = screenPoint.y / Screen.height * canvasRectTransform.sizeDelta.y;

        //screenPoint.y += shipData.value.HudOffset * Mathf.Min(1, 1 / difference.magnitude);
        rectTransform.anchoredPosition = screenPoint;
        canvasGroup.alpha = Vector3.Dot(Camera.main.transform.forward, difference) > 0 ? 1 : 0;
    }

    public void ReturnToPool()
    {
        shipData.value = null;
        if (onReturnToPool != null)
            onReturnToPool();
    }

    private void OnDestroy()
    {
        shipData.onValueChangeImmediate -= OnValueChanged_ShipData;
    }
}