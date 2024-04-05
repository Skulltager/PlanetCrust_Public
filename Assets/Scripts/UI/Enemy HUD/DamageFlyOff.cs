
using UnityEngine;
using UnityEngine.UI;

public class DamageFlyOff : PoolableBehaviour
{
    [SerializeField] private Text text = default;
    [SerializeField] private Image icon = default;
    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private float initialSpeed = default;
    [SerializeField] private float drag = default;
    [SerializeField] private float maxAngle = default;
    [SerializeField] private float visibleDuration = default;
    [SerializeField] private float fadeDuration = default;
    [SerializeField] private Color armorColor = default;
    [SerializeField] private Color shieldColor = default;
    [SerializeField] private Color hullColor = default;
    [SerializeField] private Sprite lightningImage = default;
    [SerializeField] private Sprite fireImage = default;
    [SerializeField] private Sprite corrosiveImage = default;
    [SerializeField] private Sprite physicalImage = default;
    [SerializeField] private Sprite impactImage = default;

    private EnemyHealthBar healthBar;
    private RectTransform rectTransform;
    private float timeVisible;
    private Vector2 currentSpeed;

    public override void Reset(PoolableBehaviour basePrefab)
    {
    }

    private void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void Initialize(EnemyHealthBar healthBar, int damage, DamageType damageType, ShipBarType barType)
    {
        this.healthBar = healthBar;
        healthBar.onReturnToPool += OnEvent_ReturnToPool;

        timeVisible = 0;
        text.text = "-" + damage.ToString();
        float rotationAngle = Random.Range(-maxAngle, maxAngle);
        currentSpeed = Quaternion.Euler(0, 0, rotationAngle) * Vector2.right * initialSpeed;
        rectTransform.anchoredPosition = Vector2.zero;
        switch (damageType)
        {
            case DamageType.Corrosive:
                icon.sprite = corrosiveImage;
                break;
            case DamageType.Fire:
                icon.sprite = fireImage;
                break;
            case DamageType.Impact:
                icon.sprite = impactImage;
                break;
            case DamageType.Lightning:
                icon.sprite = lightningImage;
                break;
            case DamageType.Physical:
                icon.sprite = physicalImage;
                break;
        }

        switch(barType)
        {
            case ShipBarType.Armor:
                text.color = armorColor;
                break;
            case ShipBarType.Hull:
                text.color = hullColor;
                break;
            case ShipBarType.Shield:
                text.color = shieldColor;
                break;
        }
    }

    private void FixedUpdate()
    {
        timeVisible += Time.fixedDeltaTime;
        currentSpeed *= 1 - (drag * Time.fixedDeltaTime);
        rectTransform.anchoredPosition += currentSpeed * Time.fixedDeltaTime;

        if (timeVisible < visibleDuration)
            canvasGroup.alpha = 1;
        else if (timeVisible - visibleDuration < fadeDuration)
            canvasGroup.alpha = 1 - ((timeVisible - visibleDuration) / fadeDuration);
        else
            ReturnToPool();
    }

    private void OnEvent_ReturnToPool()
    {
        ReturnToPool();
    }

    public override void ReturnToPool()
    {
        healthBar.onReturnToPool -= OnEvent_ReturnToPool;
        base.ReturnToPool();
    }
}