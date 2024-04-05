using Unity.Mathematics;
using UnityEngine;

public static partial class ByteSerializer
{
    private static void AssignConvertableTypes()
    {
        short ID = 0;
        AssignConvertableType(typeof(PathFindingSaveData_Level), ID++);
        AssignConvertableType(typeof(PathFindingSaveData_Node), ID++);
        AssignConvertableType(typeof(Vector3), ID++);
        AssignConvertableType(typeof(float3), ID++);

    }
}
