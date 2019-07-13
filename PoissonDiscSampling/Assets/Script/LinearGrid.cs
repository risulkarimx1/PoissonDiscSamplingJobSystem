using System;
using UnityEngine;

[Serializable]
public struct LinearGrid
{
    public int Width;
    public int Height;

    private Vector2[] _values;

    public void CreateGrid()
    {
        _values = new Vector2[Width * Height];
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
    public Vector2 GetValue(int col, int row)
    {
        return _values[GetIndex(col, row)];
    }
}