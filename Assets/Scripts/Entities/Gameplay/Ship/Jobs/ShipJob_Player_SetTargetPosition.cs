
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PrepareSystemGroup))]
public partial class ShipSystem_Player_SetTargetPosition : SystemBase
{
    private CollisionFilter collisionFilter;
    private BuildPhysicsWorld buildPhysicsWorld;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        collisionFilter = new CollisionFilter()
        {
            BelongsTo = (uint)LayerFlags.Projectiles,
            CollidesWith = (uint)(LayerFlags.Ships_Detail | LayerFlags.Walls),
        };
    }

    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_Player_SetTargetPosition
        {
            ref_Translation = GetComponentDataFromEntity<Translation>(),
            cameraPosition = Camera.main.transform.position,
            cameraRotation = Camera.main.transform.rotation,
            filter = collisionFilter,
            physicsWorld = buildPhysicsWorld.PhysicsWorld,
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_PlayerControlled))]
    private partial struct ShipJob_Player_SetTargetPosition : IJobEntity
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> ref_Translation;
        [ReadOnly] public float3 cameraPosition;
        [ReadOnly] public quaternion cameraRotation;
        [ReadOnly] public CollisionFilter filter;
        [ReadOnly] public PhysicsWorld physicsWorld;

        private void Execute(
            ref ShipEntityData_TargetPosition targetPosition,
            in ShipEntityData_Player_AimDistance aimDistance,
            in Translation translation)
        {
            float3 shipPosition = translation.Value;
            targetPosition.targetPosition = GetPrimaryAimedPosition(shipPosition, aimDistance.minDistance, aimDistance.maxDistance);
        }

        private float3 GetPrimaryAimedPosition(float3 shipPosition, float weaponMinAimDistance, float weaponMaxAimDistance)
        {
            float3 forward = math.mul(cameraRotation, math.forward());
            float3 rayPosition = IntersectPoint(forward, cameraPosition, -forward, shipPosition);
            rayPosition += forward * weaponMinAimDistance;
            float aimDistance = weaponMaxAimDistance - weaponMinAimDistance;
            RaycastInput raycastInput = new RaycastInput
            {
                Start = rayPosition,
                End = rayPosition + forward * aimDistance,
                Filter = filter,
            };

            if (physicsWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit closestHit))
                return closestHit.Position;

            return rayPosition + forward * (weaponMaxAimDistance - weaponMinAimDistance);
        }

        private float3 IntersectPoint(float3 rayVector, float3 rayPoint, float3 planeNormal, float3 planePoint)
        {
            var diff = rayPoint - planePoint;
            var prod1 = math.dot(diff, planeNormal);
            var prod2 = math.dot(rayVector, planeNormal);
            var prod3 = prod1 / prod2;
            return rayPoint - rayVector * prod3;
        }
    }
}
