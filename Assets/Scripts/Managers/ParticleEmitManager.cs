using SheetCodes;
using System;
using UnityEngine;

public class ParticleEmitManager : MonoBehaviour
{
    public static ParticleEmitManager instance { private set; get; }

    private readonly EventDictionary<ParticleIdentifier, ParticleSystem[]> particleSystems;

    private ParticleEmitManager()
    {
        particleSystems = new EventDictionary<ParticleIdentifier, ParticleSystem[]>();
    }

    public void Initialize()
    {
        ParticleIdentifier[] identifiers = Enum.GetValues(typeof(ParticleIdentifier)) as ParticleIdentifier[];
        foreach (ParticleIdentifier identifier in identifiers)
        {
            if (identifier == ParticleIdentifier.None)
                continue;

            GameObject instance = GameObject.Instantiate(identifier.GetRecord().ParticleSystem, transform).gameObject;
            particleSystems.Add(identifier, instance.GetComponentsInChildren<ParticleSystem>());
        }
        instance = this;
    }

    public void PlayParticle(ParticleIdentifier identifier, Vector3 worldPosition, Quaternion rotation)
    {
        ParticleSystem[] subSystems = particleSystems[identifier];
        subSystems[0].transform.position = worldPosition;
        subSystems[0].transform.rotation = rotation;

        foreach (ParticleSystem particleSystem in subSystems)
            particleSystem.Emit(1);
    }

    public void Reset()
    {
        foreach (ParticleSystem[] particleSystemCollection in particleSystems.Values)
            foreach (ParticleSystem particleSystem in particleSystemCollection)
                particleSystem.Clear(false);
    }
}
