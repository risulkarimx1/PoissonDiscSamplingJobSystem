using Unity.Mathematics;
using UnityEngine;

public struct AreaMeasurement 
{
    public Transform V0,V1,V2,V3;
    public float2 Measure()
    {
        var width = math.abs(V0.position.x - V1.position.x);
        var height = math.abs(V0.position.z - V3.position.z);
        return new float2(width,height);
    }
}
