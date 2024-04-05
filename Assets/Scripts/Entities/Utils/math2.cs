
using Unity.Mathematics;
using UnityEngine;

public static class math2
{
    public static float2 Rotate(float2 v, float degrees)
    {
        float rads = degrees * Mathf.Deg2Rad;
        float sin = math.sin(rads);
        float cos = math.cos(rads);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float MoveTowards(float current, float target, float maxDistanceDelta)
    {
        float distance = target - current;

        if (math.abs(distance) <= maxDistanceDelta)
            return target;

        if (distance > 0)
            return current + maxDistanceDelta;
        else
            return current - maxDistanceDelta;
    }

    public static float2 MoveTowards(float2 current, float2 target, float maxDistanceDelta)
    {
        float deltaX = target.x - current.x;
        float deltaY = target.y - current.y;
        float sqdist = deltaX * deltaX + deltaY * deltaY;

        if (sqdist <= maxDistanceDelta * maxDistanceDelta)
            return target;

        float dist = math.sqrt(sqdist);
        return new float2(current.x + deltaX / dist * maxDistanceDelta, current.y + deltaY / dist * maxDistanceDelta);
    }

    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
    {
        float deltaX = target.x - current.x;
        float deltaY = target.y - current.y;
        float deltaZ = target.z - current.z;
        float sqdist = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
        if (sqdist == 0 || sqdist <= maxDistanceDelta * maxDistanceDelta)
            return target;

        float dist = (float)math.sqrt(sqdist);
        return new float3(current.x + deltaX / dist * maxDistanceDelta,
            current.y + deltaY / dist * maxDistanceDelta,
            current.z + deltaZ / dist * maxDistanceDelta);
    }

    public static float Angle(float3 from, float3 to)
    {
        return math.degrees(math.acos(math.dot(math.normalize(from), math.normalize(to))));
    }

    public static float SignedAngle(float3 from, float3 to, float3 axis)
    {
        float angle = math.acos(math.dot(math.normalize(from), math.normalize(to)));
        float sign = math.sign(math.dot(axis, math.cross(from, to)));
        return math.degrees(angle * sign);
    }

    public static float3 RotateRound(float3 position, float3 axis, float angle)
    {
        return math.mul(quaternion.AxisAngle(axis, angle), position);
    }

    public static float3 RotateTowards(float3 current, float3 target, float maxAngle)
    {
        float dot = math.dot(current, target);
        dot = math.clamp(dot, -1, 1);

        float theta = math.min(math.acos(dot), maxAngle);
        float3 relativeVec = math.normalize(target - current * dot);
        return ((current * math.cos(theta)) + (relativeVec * math.sin(theta)));
    }

    public static bool TryCalculateInterceptCourse(float3 targetPosition, float3 targetSpeed, float3 interceptorPosition, float interceptopSpeed, out float3 interceptDirection)
    {
        float3 targetDir = targetPosition - interceptorPosition;
        float iSpeed2 = interceptopSpeed * interceptopSpeed;
        float tSpeed2 = math.lengthsq(targetSpeed);
        float fDot1 = math.dot(targetDir, targetSpeed);
        float targetDist2 = math.lengthsq(targetDir);
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
        {
            interceptDirection = Vector3.zero;
            return false;
        }
        float sqrt = math.sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;
        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
            {
                interceptDirection = float3.zero;
                return false;
            }

            interceptDirection = math.normalize((S2) * targetDir + targetSpeed);
        }
        else if (S2 < 0.0001f)
            interceptDirection = math.normalize((S1) * targetDir + targetSpeed);
        else if (S1 < S2)
            interceptDirection = math.normalize((S2) * targetDir + targetSpeed);
        else
            interceptDirection = math.normalize((S1) * targetDir + targetSpeed);

        return true;
    }
}