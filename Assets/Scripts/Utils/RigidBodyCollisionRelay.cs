
using System;
using UnityEngine;

public class RigidBodyCollisionRelay : MonoBehaviour
{
    public event Action<Collision> onCollisionEnter;
    public event Action<Collision> onCollisionExit;

    private void OnCollisionEnter(Collision collision)
    {
        if(onCollisionEnter != null)
            onCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (onCollisionExit != null)
            onCollisionExit(collision);
    }
}