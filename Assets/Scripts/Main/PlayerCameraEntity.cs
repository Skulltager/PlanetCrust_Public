
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class PlayerCameraEntity : MonoBehaviour
{
    [SerializeField] private float cameraSensitivity = default;
    [SerializeField] private float maxCameraDrift = default;
    [SerializeField] private float cameraPositionCatchupFactor = default;
    [SerializeField] private Transform targetCameraRotation = default;
    [SerializeField] private Transform targetCameraPosition = default;
    [SerializeField] private Transform cameraChaseRotation = default;
    [SerializeField] private Transform cameraChasePosition = default;

    [SerializeField] private Vector3 cameraZoom = default;
    [SerializeField] private Vector3 cameraZoomOffset = default;
    [SerializeField] private float cameraDistanceFromWall = default;
    [SerializeField] private float cameraZoomCatchupFactor = default;
    [SerializeField] private int startCameraZoom = default;
    [SerializeField] private int minimumCameraZoom = default;
    [SerializeField] private int maximumCameraZoom = default;
    [SerializeField] private float cameraZoomScale = default;

    private readonly EventVariable<PlayerCameraEntity, int> cameraZoomFactor;

    private Transform cameraPosition;
    private Vector3 mouseMove;
    private Vector3 shipPosition;
    private Vector3 targetCameraZoomPosition;

    private BuildPhysicsWorld buildPhysicsWorld;
    private EntityEvent_LateFixedUpdate lateFixedUpdate;

    private int Check_CameraZoomFactor(int value)
    {
        return Mathf.Clamp(value, minimumCameraZoom, maximumCameraZoom);
    }

    private PlayerCameraEntity()
    {
        cameraZoomFactor = new ControlledEventVariable<PlayerCameraEntity, int>(this, 0, Check_CameraZoomFactor);
    }

    private void Awake()
    {
        buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        cameraPosition = Camera.main.transform;
        cameraZoomFactor.value = startCameraZoom;

        ShipData.controllingShip.onValueChangeImmediate += OnValueChanged_ControllingShip;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraZoomFactor.onValueChangeImmediate += OnValueChanged_CameraZoomFactor;
        targetCameraPosition.localPosition = targetCameraZoomPosition;
        cameraChasePosition.localPosition = targetCameraZoomPosition;
        cameraPosition.position = targetCameraPosition.position;
        cameraPosition.rotation = targetCameraRotation.rotation;

        lateFixedUpdate = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EntityEvent_LateFixedUpdate>();
        lateFixedUpdate.onLateFixedUpdate += OnLateFixedUpdate;
    }

    private void OnValueChanged_ControllingShip(ShipData oldValue, ShipData newValue)
    {
        if (oldValue != null)
        {
            oldValue.transformData.position.onValueChange -= OnValueChanged_Position;
            AudioObjectTracker.instance.followPosition.value = null;
            AudioObjectTracker.instance.followRotation.value = null;
        }

        if (newValue != null)
        {
            newValue.transformData.position.onValueChange += OnValueChanged_Position;
            AudioObjectTracker.instance.followPosition.value = targetCameraRotation;
            AudioObjectTracker.instance.followRotation.value = targetCameraRotation;
        }
    }

    private void OnValueChanged_CameraZoomFactor(int oldValue, int newValue)
    {
        targetCameraZoomPosition = cameraZoomOffset + (cameraZoom * Mathf.Pow(newValue, cameraZoomScale));
    }

    private void OnValueChanged_Position(Vector3 oldValue, Vector3 newValue)
    {
        shipPosition = newValue;
    }

    private void Update()
    {
        mouseMove.y += Input.GetAxis("Mouse X") * cameraSensitivity;
        mouseMove.x -= Input.GetAxis("Mouse Y") * cameraSensitivity;
        cameraZoomFactor.value -= (int)Input.mouseScrollDelta.y;
    }

    private void OnLateFixedUpdate()
    {
        if (ShipData.controllingShip.value != null)
            targetCameraRotation.position = shipPosition + ShipData.controllingShip.value.velocityData.velocity.value * Time.fixedDeltaTime;
        else
            targetCameraRotation.position = shipPosition;

        Vector3 targetOldPosition = targetCameraPosition.position;
        targetCameraRotation.Rotate(mouseMove);
        Vector3 targetDifference = targetCameraPosition.position - targetOldPosition;
        Vector3 chaseOldPosition = cameraChasePosition.position;
        cameraChaseRotation.rotation = targetCameraRotation.rotation;
        Vector3 chaseDifference = cameraChasePosition.position - chaseOldPosition;
        cameraChaseRotation.position += targetDifference - chaseDifference;

        mouseMove = Vector3.zero;

        AdjustCameraZoom();
        ChaseTargetCameraPosition();
        SetFinalCameraPosition();
    }

    private void AdjustCameraZoom()
    {
        Vector3 difference = targetCameraZoomPosition - targetCameraPosition.localPosition;
        Vector3 moveAmount = difference * cameraZoomCatchupFactor * Time.fixedDeltaTime;
        targetCameraPosition.localPosition += moveAmount;
    }

    private void ChaseTargetCameraPosition()
    {
        // First make sure the chase camera is on the same forward distance as the target Camera
        UnityEngine.Plane plane = new UnityEngine.Plane(targetCameraPosition.forward, targetCameraPosition.position);
        UnityEngine.Ray planeRay = new UnityEngine.Ray(cameraChasePosition.position, targetCameraPosition.forward);

        float distance;
        if (!plane.Raycast(planeRay, out distance))
        {
            planeRay.direction = -planeRay.direction;
            plane.Raycast(planeRay, out distance);
            distance = -distance;
        }

        cameraChasePosition.position += cameraChasePosition.forward * distance;

        //Now move towards the target camera position
        Vector3 difference = cameraChasePosition.position - targetCameraPosition.position;
        float moveFactorAmount = difference.magnitude * cameraPositionCatchupFactor * Time.fixedDeltaTime;

        moveFactorAmount = Mathf.Max(moveFactorAmount, difference.magnitude - maxCameraDrift);

        Vector3 moveAmount = difference.normalized * moveFactorAmount;
        cameraChasePosition.position -= moveAmount;
    }

    private void SetFinalCameraPosition()
    {
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();

        CollisionFilter collisionFilter = new CollisionFilter()
        {
            BelongsTo = (uint)LayerFlags.Projectiles,
            CollidesWith = (uint)(LayerFlags.Ships | LayerFlags.Walls),
        };
        Vector3 rayPosition = shipPosition + targetCameraRotation.rotation * cameraZoomOffset;
        ColliderCastHit hit;
        PhysicsWorld physicsWorld = buildPhysicsWorld.PhysicsWorld;

        Vector3 differenceToCamera = cameraChasePosition.position - rayPosition;
        if (physicsWorld.SphereCast(rayPosition, cameraDistanceFromWall, differenceToCamera.normalized, differenceToCamera.magnitude, out hit, collisionFilter))
            cameraPosition.position = rayPosition + differenceToCamera.normalized * hit.Fraction * differenceToCamera.magnitude;
        else
            cameraPosition.position = cameraChasePosition.position;

        cameraPosition.rotation = cameraChaseRotation.rotation;
    }

    private void OnDestroy()
    {
        lateFixedUpdate.onLateFixedUpdate -= OnLateFixedUpdate;
        ShipData.controllingShip.onValueChangeImmediate -= OnValueChanged_ControllingShip;
    }
}