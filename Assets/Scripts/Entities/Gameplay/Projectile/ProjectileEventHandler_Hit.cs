
using FMODUnity;
using SheetCodes;
using Unity.Entities;
using UnityEngine;

public class ProjectileEventHandler_Hit : MonoBehaviour
{
    [SerializeField] private ParticleIdentifier wallHitIdentifier = default;
    [SerializeField] private ParticleIdentifier shipHitIdentifier = default;

    [SerializeField] private EventReference wallHitEventReference = default;
    [SerializeField] private EventReference shipHitEventReference = default;

    private ProjectileSystem_Move projectileMoveSystem;

    private void Start()
    {
        projectileMoveSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ProjectileSystem_Move>();
        projectileMoveSystem.onProjectileHitWall += OnEvent_ProjectileHitWall;
        projectileMoveSystem.onProjectileHitShip += OnEvent_ProjectileHitShip;
    }

    private void OnEvent_ProjectileHitWall(ProjectileHitEventData eventData)
    {
        RuntimeManager.PlayOneShot(wallHitEventReference, eventData.point);
        Quaternion rotation = Quaternion.LookRotation(eventData.normalHit);
        ParticleEmitManager.instance.PlayParticle(wallHitIdentifier, eventData.point, rotation);
    }

    private void OnEvent_ProjectileHitShip(ProjectileHitEventData eventData)
    {
        RuntimeManager.PlayOneShot(shipHitEventReference, eventData.point);
        Quaternion rotation = Quaternion.LookRotation(eventData.normalHit);
        ParticleEmitManager.instance.PlayParticle(shipHitIdentifier, eventData.point, rotation);
    }

    private void OnDestroy()
    {
        projectileMoveSystem.onProjectileHitWall -= OnEvent_ProjectileHitWall;
        projectileMoveSystem.onProjectileHitShip -= OnEvent_ProjectileHitShip;
    }
}