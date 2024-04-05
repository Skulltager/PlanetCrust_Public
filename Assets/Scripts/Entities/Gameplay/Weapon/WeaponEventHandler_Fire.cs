
using FMODUnity;
using SheetCodes;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WeaponEventHandler_Fire : MonoBehaviour
{
    [SerializeField] private EventReference eventReference = default;

    private void Start()
    {
        WeaponEventSystem_Fire.onWeaponFire += OnEvent_WeaponFire;
    }

    private void OnEvent_WeaponFire(WeaponEventData_Fire eventData)
    {
        RuntimeManager.PlayOneShot(eventReference, eventData.position);
    }

    private void OnDestroy()
    {
        WeaponEventSystem_Fire.onWeaponFire -= OnEvent_WeaponFire;
    }
}