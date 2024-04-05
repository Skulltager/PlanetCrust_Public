using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class WeaponIndicator : DataDrivenUI<WeaponData_Transform>
{
    [SerializeField] private RectTransform indicatorImage = default;
    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private float sizePerDistanceSpread = default;
    [SerializeField] private float minSize = default;
    [SerializeField] private float chasePositionFactor = default;
    [SerializeField] private float chasePositionMinDistance = default;
    [SerializeField] private float chaseSizeFactor = default;
    [SerializeField] private float chaseSizeMinDistance = default;

    private RectTransform canvasRectTransform;

    private BuildPhysicsWorld buildPhysicsWorld;
    private CollisionFilter collisionFilter;

    private void Awake()
    {
        canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;

        buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        collisionFilter = new CollisionFilter()
        {
            BelongsTo = (uint)LayerFlags.Projectiles,
            CollidesWith = (uint)(LayerFlags.Walls),
        };
    }

    protected override void OnValueChanged_Data(WeaponData_Transform oldValue, WeaponData_Transform newValue)
    {
        if (newValue == null)
            return;

        Vector3 targetPosition = newValue.weaponData.shipData.targetPositionData.targetPosition.value;
        Vector3 weaponPosition = newValue.position.value;
        Vector3 forward = newValue.rotation.value * Vector3.forward;
        float firingAngle = newValue.weaponData.fireData.firingAngle.value;
        float spread = newValue.weaponData.fireData.spread.value;
        PhysicsWorld physicsWorld = buildPhysicsWorld.PhysicsWorld;

        Vector3 desiredFireDirection = targetPosition - weaponPosition;
        Vector3 fireDirection = Vector3.RotateTowards(forward, desiredFireDirection, firingAngle * Mathf.Deg2Rad, 0);

        if (Vector3.Dot(fireDirection, Camera.main.transform.forward) <= 0)
        {
            canvasGroup.alpha = 0;
            return;
        }
        canvasGroup.alpha = 1;

        Vector3 rotatedFireDirection = new Vector3(fireDirection.y, fireDirection.z, fireDirection.x);
        Vector3 spreadFireDirection = Vector3.RotateTowards(fireDirection, rotatedFireDirection, spread * Mathf.Deg2Rad, 0);

        float aimDistance = desiredFireDirection.magnitude;
        Vector3 aimPosition = weaponPosition + fireDirection * aimDistance;
        RaycastInput input = new RaycastInput
        {
            Start = weaponPosition,
            End = aimPosition,
            Filter = collisionFilter,
        };
        
        Vector3 centerShotEndPosition;
        float distanceForSpreadShot;
        if (physicsWorld.CastRay(input, out Unity.Physics.RaycastHit hit))
        {
            centerShotEndPosition = hit.Position;
            distanceForSpreadShot = hit.Fraction * aimDistance;
        }
        else
        {
            centerShotEndPosition = aimPosition;
            distanceForSpreadShot = aimDistance;
        }

        Vector3 maxSpreadShotEndPosition = weaponPosition + spreadFireDirection * distanceForSpreadShot;

        Vector2 screenPointCenter = Camera.main.WorldToScreenPoint(centerShotEndPosition);
        screenPointCenter.x = screenPointCenter.x / Screen.width * canvasRectTransform.sizeDelta.x;
        screenPointCenter.y = screenPointCenter.y / Screen.height * canvasRectTransform.sizeDelta.y;

        Vector2 screenPointSpread = Camera.main.WorldToScreenPoint(maxSpreadShotEndPosition);
        screenPointSpread.x = screenPointSpread.x / Screen.width * canvasRectTransform.sizeDelta.x;
        screenPointSpread.y = screenPointSpread.y / Screen.height * canvasRectTransform.sizeDelta.y;

        float size = (screenPointCenter - screenPointSpread).magnitude * sizePerDistanceSpread;
        size = Mathf.Max(minSize, size);
        indicatorImage.sizeDelta = new Vector2(size, size);
        rectTransform.anchoredPosition = screenPointCenter;
    }

    public void UpdateIndicator()
    {
        Vector3 targetPosition = data.weaponData.shipData.targetPositionData.targetPosition.value;
        Vector3 weaponPosition = data.position.value;
        Vector3 forward = data.rotation.value * Vector3.forward;
        float firingAngle = data.weaponData.fireData.firingAngle.value;
        float spread = data.weaponData.fireData.spread.value;
        PhysicsWorld physicsWorld = buildPhysicsWorld.PhysicsWorld;

        Vector3 differenceToTarget = targetPosition - weaponPosition;
        Vector3 directionToTarget = differenceToTarget.normalized;
        Vector3 fireDirection = Vector3.RotateTowards(forward, differenceToTarget, firingAngle * Mathf.Deg2Rad, 0);

        if (Vector3.Dot(fireDirection, Camera.main.transform.forward) <= 0)
        {
            canvasGroup.alpha = 0;
            return;
        }
        canvasGroup.alpha = 1;

        Vector3 rotatedFireDirection = new Vector3(fireDirection.y, fireDirection.z, fireDirection.x);
        Vector3 spreadFireDirection = Vector3.RotateTowards(fireDirection, rotatedFireDirection, spread * Mathf.Deg2Rad, 0);

        float aimDistance = differenceToTarget.magnitude;
        Vector3 aimPosition = weaponPosition + fireDirection * aimDistance;
        RaycastInput input = new RaycastInput
        {
            Start = weaponPosition,
            End = aimPosition,
            Filter = collisionFilter,
        };

        Vector3 centerShotEndPosition;
        float distanceForSpreadShot;
        if (physicsWorld.CastRay(input, out Unity.Physics.RaycastHit hit))
        {
            centerShotEndPosition = hit.Position;
            distanceForSpreadShot = hit.Fraction * aimDistance;
        }
        else
        {
            centerShotEndPosition = aimPosition;
            distanceForSpreadShot = aimDistance;
        }

        Vector3 maxSpreadShotEndPosition = weaponPosition + spreadFireDirection * distanceForSpreadShot;

        Vector2 screenPointCenter = Camera.main.WorldToScreenPoint(centerShotEndPosition);
        screenPointCenter.x = screenPointCenter.x / Screen.width * canvasRectTransform.sizeDelta.x;
        screenPointCenter.y = screenPointCenter.y / Screen.height * canvasRectTransform.sizeDelta.y;

        Vector2 screenPointSpread = Camera.main.WorldToScreenPoint(maxSpreadShotEndPosition);
        screenPointSpread.x = screenPointSpread.x / Screen.width * canvasRectTransform.sizeDelta.x;
        screenPointSpread.y = screenPointSpread.y / Screen.height * canvasRectTransform.sizeDelta.y;

        float size = (screenPointCenter - screenPointSpread).magnitude * sizePerDistanceSpread;
        size = Mathf.Max(minSize, size);

        float differenceToCurrentSize = size - indicatorImage.sizeDelta.x;
        float sizeChange;
        if (differenceToCurrentSize > 0)
            sizeChange = Mathf.Max(chaseSizeMinDistance, differenceToCurrentSize * chaseSizeFactor) * Time.fixedDeltaTime;
        else
            sizeChange = -Mathf.Max(chaseSizeMinDistance, -differenceToCurrentSize * chaseSizeFactor) * Time.fixedDeltaTime;

        if(Mathf.Abs(sizeChange) > differenceToCurrentSize)
        {
            indicatorImage.sizeDelta = new Vector2(size, size);
        }
        else
        {
            float newSize = indicatorImage.sizeDelta.x + sizeChange;
            indicatorImage.sizeDelta = new Vector2(newSize, newSize);
        }

        Vector2 differenceToCurrentPosition = screenPointCenter - rectTransform.anchoredPosition;
        float positionDifference = Mathf.Max(chasePositionMinDistance, differenceToCurrentPosition.magnitude * chasePositionFactor) * Time.fixedDeltaTime;
        if(positionDifference < differenceToCurrentPosition.magnitude)
        {
            Vector2 newPosition = rectTransform.anchoredPosition + differenceToCurrentPosition.normalized * positionDifference;
            rectTransform.anchoredPosition = newPosition;
        }
        else
        {
            rectTransform.anchoredPosition = screenPointCenter;
        }
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}