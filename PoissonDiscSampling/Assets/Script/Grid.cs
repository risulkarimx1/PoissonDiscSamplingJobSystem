using System;
using UnityEngine;

[Serializable]
public struct Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private Vector2[] _values;

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _values = new Vector2[width * height];
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
    public Vector2 GetValue(float col, float row)
    {
        return GetValue((int)col, (int)row);
    }
}