using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

[Serializable]
public struct LinearGrid
{
    public int Width;
    public int Height;

    private float2[] _values;

    public void CreateGrid()
    {
        _values = new float2[Width * Height];
        for (var i = 0; i < _values.Length; i++)
        {
            _values[i] = new float2(float.MinValue, float.MinValue);
        }
    }

    public void AddValue(int col, int row, Vector2 Value)
    {
        var index = GetIndex(col, row);
        _values[index] = Value;
    }
    private int GetIndex(int col, int row)
    {
        return col + row * Width;
    }
    public float2 GetValue(int col, int row)
    {
        return _values[GetIndex(col, row)];
    }
}