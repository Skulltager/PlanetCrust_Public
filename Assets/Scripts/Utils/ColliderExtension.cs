
using UnityEngine;

public static class ColliderExtension
{
    public static readonly RaycastHit[] cashedRaycastHits;
    public static readonly Collider[] cashedCollidersPublic;
    private static readonly Collider[] cashedCollidersLocal;

    static ColliderExtension()
    {
        cashedRaycastHits = new RaycastHit[1024];
        cashedCollidersLocal = new Collider[1024];
        cashedCollidersPublic = new Collider[1024];
    }

    #region Collisions
    public static Collider[] GetCollisions(this Collider[] colliders, out int count)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Avatar");
        return GetCollisions(colliders, out count, layerMask);
    }

    public static Collider[] GetCollisions(this Collider[] colliders, out int count, int layerMask)
    {
        count = 0;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] is BoxCollider boxCollider)
            {
                int range = GetBoxCollisions(boxCollider, layerMask, cashedCollidersLocal);
                for (int j = 0; j < range; j++)
                    if (!cashedCollidersPublic.Contains(cashedCollidersLocal[j], count))
                        cashedCollidersPublic[count++] = cashedCollidersLocal[j];
            }
            else if (colliders[i] is SphereCollider sphereCollider)
            {
                int range = GetSphereCollisions(sphereCollider, layerMask, cashedCollidersLocal);
                for (int j = 0; j < range; j++)
                    if (!cashedCollidersPublic.Contains(cashedCollidersLocal[j], count))
                        cashedCollidersPublic[count++] = cashedCollidersLocal[j];
            }
            else if (colliders[i] is CapsuleCollider capsuleCollider)
            {
                int range = GetCapsuleCollisions(capsuleCollider, layerMask, cashedCollidersLocal);
                for (int j = 0; j < range; j++)
                    if (!cashedCollidersPublic.Contains(cashedCollidersLocal[j], count))
                        cashedCollidersPublic[count++] = cashedCollidersLocal[j];
            }
        }

        return cashedCollidersPublic;
    }
    #endregion

    #region Box Collisions
    public static Collider[] GetBoxCollisions(this BoxCollider[] boxColliders)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Avatar");
        return GetBoxCollisions(boxColliders, layerMask);
    }

    public static Collider[] GetBoxCollisions(this BoxCollider[] boxColliders, int layerMask)
    {
        int count = 0;
        foreach (BoxCollider boxCollider in boxColliders)
        {
            int range = GetBoxCollisions(boxCollider, layerMask, cashedCollidersLocal);
            for (int j = 0; j < range; j++)
                if (!cashedCollidersPublic.Contains(cashedCollidersLocal[j], count))
                    cashedCollidersPublic[count++] = cashedCollidersLocal[j];
        }
        return cashedCollidersPublic;
    }

    public static int GetBoxCollisions(this BoxCollider boxCollider, Collider[] colliders)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Avatar");
        return GetBoxCollisions(boxCollider, layerMask, colliders);
    }

    public static int GetBoxCollisions(this BoxCollider boxCollider, int layerMask, Collider[] colliders)
    {
        Vector3 size = boxCollider.size;
        size.x = size.x * boxCollider.transform.lossyScale.x;
        size.y = size.y * boxCollider.transform.lossyScale.y;
        size.z = size.z * boxCollider.transform.lossyScale.z;

        Vector3 center = boxCollider.center;
        center.x *= boxCollider.transform.lossyScale.x;
        center.y *= boxCollider.transform.lossyScale.y;
        center.z *= boxCollider.transform.lossyScale.z;

        Vector3 globalCenter = boxCollider.transform.rotation * center;
        return Physics.OverlapBoxNonAlloc(boxCollider.transform.position + globalCenter, size / 2, colliders, boxCollider.transform.rotation, layerMask, QueryTriggerInteraction.Collide);
    }
    #endregion

    #region Sphere Collisions
    public static int GetSphereCollisions(this SphereCollider sphereCollider, Collider[] colliders)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Avatar");
        return GetSphereCollisions(sphereCollider, layerMask, colliders);
    }

    public static int GetSphereCollisions(this SphereCollider sphereCollider, int layerMask, Collider[] colliders)
    {
        Vector3 center = sphereCollider.center;
        center.x *= sphereCollider.transform.lossyScale.x;
        center.y *= sphereCollider.transform.lossyScale.y;
        center.z *= sphereCollider.transform.lossyScale.z;

        Vector3 globalCenter = sphereCollider.transform.rotation * center;
        return Physics.OverlapSphereNonAlloc(sphereCollider.transform.position + globalCenter, sphereCollider.radius, colliders, layerMask, QueryTriggerInteraction.Collide);
    }
    #endregion

    #region Capsule Collisions
    public static int GetCapsuleCollisions(this CapsuleCollider capsuleCollider, Collider[] colliders)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Avatar");
        return GetCapsuleCollisions(capsuleCollider, layerMask, colliders);
    }

    public static int GetCapsuleCollisions(this CapsuleCollider capsuleCollider, int layerMask, Collider[] colliders)
    {
        Vector3 center = capsuleCollider.center;
        center.x *= capsuleCollider.transform.lossyScale.x;
        center.y *= capsuleCollider.transform.lossyScale.y;
        center.z *= capsuleCollider.transform.lossyScale.z;
        Vector3 globalCenter = capsuleCollider.transform.rotation * center;

        Vector3 point1;
        Vector3 point2;

        float distance = capsuleCollider.height / 2 - capsuleCollider.radius;
        if (capsuleCollider.direction == 0)
        {
            //X Axis
            point1 = capsuleCollider.transform.position - capsuleCollider.transform.right * distance;
            point2 = capsuleCollider.transform.position + capsuleCollider.transform.right * distance;
        }
        else if (capsuleCollider.direction == 1)
        {
            //Y Axis
            point1 = capsuleCollider.transform.position - capsuleCollider.transform.up * distance;
            point2 = capsuleCollider.transform.position + capsuleCollider.transform.up * distance;
        }
        else
        {
            //Z Axis
            point1 = capsuleCollider.transform.position - capsuleCollider.transform.forward * distance;
            point2 = capsuleCollider.transform.position + capsuleCollider.transform.forward * distance;
        }
        return Physics.OverlapCapsuleNonAlloc(point1, point2, capsuleCollider.radius, colliders, layerMask, QueryTriggerInteraction.Collide);
    }
    #endregion
}
