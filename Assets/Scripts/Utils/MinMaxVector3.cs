
using UnityEngine;

public struct MinMaxVector3
{
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public float zMin;
    public float zMax;

    public float xSize => xMax - xMin;
    public float ySize => yMax - yMin;
    public float zSize => zMax - zMin;

    public MinMaxVector3(Bounds bounds)
    {
        xMin = bounds.min.x;
        xMax = bounds.max.x;
        yMin = bounds.min.y;
        yMax = bounds.max.y;
        zMin = bounds.min.z;
        zMax = bounds.max.z;
    }

    public static MinMaxVector3 operator +(MinMaxVector3 a, MinMaxVector3 b)
    {
        a.xMin = Mathf.Min(a.xMin, b.xMin);
        a.xMax = Mathf.Max(a.xMax, b.xMax);
        a.yMin = Mathf.Min(a.yMin, b.yMin);
        a.yMax = Mathf.Max(a.yMax, b.yMax);
        a.zMin = Mathf.Min(a.zMin, b.zMin);
        a.zMax = Mathf.Max(a.zMax, b.zMax);
        return a;
    }
}