
using FMODUnity;
using SheetCodes;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShipEventHandler_Destroy : MonoBehaviour
{
    [SerializeField] private ParticleIdentifier shipExplodeIdentifier = default;
    [SerializeField] private EventReference eventReference = default;

    private ShipSystem_Destroy_EventTrigger checkDeathSystem;

    private void Start()
    {
        checkDeathSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ShipSystem_Destroy_EventTrigger>();
        checkDeathSystem.onShipDestroyed += OnEvent_ShipDeath;
    }

    private void OnEvent_ShipDeath(float3 position)
    {
        ParticleEmitManager.instance.PlayParticle(shipExplodeIdentifier, position, Quaternion.identity);
        RuntimeManager.PlayOneShot(eventReference, position);
    }

    private void OnDestroy()
    {
        checkDeathSystem.onShipDestroyed -= OnEvent_ShipDeath;
    }
}