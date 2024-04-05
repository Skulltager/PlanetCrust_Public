
using UnityEngine;

public struct Point3D
{
    public int x;
    public int y;
    public int z;

    public int distanceSquared => x * x + y * y + z * z;
    public float distance => Mathf.Sqrt(distanceSquared);
    public float blockDistance => Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);

    public Point3D(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Point3D operator -(Point3D a, Point3D b)
    {
        return new Point3D(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Point3D operator +(Point3D a, Point3D b)
    {
        return new Point3D(a.x + b.x, a.y + b.y, a.z + b.z);
    }
}
