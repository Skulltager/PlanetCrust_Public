
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private float visibleDuration = default;
    [SerializeField] private float fadeDuration = default;
    [SerializeField] private Text text = default;
    [SerializeField] private RectTransform damageFlyOffContainer = default;

    private float totalVisibleDuration => visibleDuration + fadeDuration;
    private EnemyHealthBar healthBar;
    private float timeVisibleRemaining;
    private int damageWhileVisible;

    private void Awake()
    {
        healthBar = GetComponentInParent<EnemyHealthBar>();
        healthBar.shipData.onValueChange += OnValueChanged_ShipData;
        timeVisibleRemaining = 0;
    }

    private void OnValueChanged_ShipData(ShipData oldValue, ShipData newValue)
    {
        if (oldValue != null)
        {
            oldValue.onTakeDamage -= OnEvent_TakeDamage;
        }

        if (newValue != null)
        {
            newValue.onTakeDamage += OnEvent_TakeDamage;
        }
    }

    private void OnEvent_TakeDamage(ShipEventData_TakeDamage eventData)
    {
        if (eventData.hullDamage > 0)
            AddDamageValue(eventData.hullDamage, ShipBarType.Hull);

        if (eventData.armorDamage > 0)
            AddDamageValue(eventData.armorDamage, ShipBarType.Armor);

        if (eventData.shieldDamage > 0)
            AddDamageValue(eventData.shieldDamage, ShipBarType.Shield);
    }

    private void AddDamageValue(int amount, ShipBarType barType)
    {
        timeVisibleRemaining = totalVisibleDuration;
        damageWhileVisible += amount;
        text.text = damageWhileVisible.ToString();
        DamageFlyOff instance = PrefabPoolManager.instance.GetDamageFlyOff(damageFlyOffContainer);
        gameObject.SetActive(true);
        instance.Initialize(healthBar, amount, DamageType.Physical, barType);
    }

    private void Update()
    {
        timeVisibleRemaining -= Time.deltaTime;

        if (timeVisibleRemaining <= 0)
        {
            gameObject.SetActive(false);
            damageWhileVisible = 0;
            return;
        }

        if (timeVisibleRemaining < fadeDuration)
            canvasGroup.alpha = timeVisibleRemaining / fadeDuration;
        else
            canvasGroup.alpha = 1;
    }

    private void OnDestroy()
    {
        healthBar.shipData.onValueChange -= OnValueChanged_ShipData;
    }
}