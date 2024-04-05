
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    private List<ParticleSystem> particleSystems;

    private void Awake()
    {
        particleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }

    private void Update()
    {
        for(int i = particleSystems.Count - 1; i >= 0; i--)
        {
            if (particleSystems[i].particleCount == 0)
                particleSystems.RemoveAt(i);
        }
        
        if(particleSystems.Count == 0)
            GameObject.Destroy(gameObject);
    }
}