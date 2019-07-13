using System;
using UnityEngine;

[Serializable]
public class Grid
{
    public int Width;
    public int Height;

    private Vector2[] Values;

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        Values = new Vector2[width * height];
    }

    public void AddValue(int col, int row, Vector2 Value)
    {
        var index = GetIndex(col, row);
        Values[index] = Value;
    }
    private int GetIndex(int col, int row)
    {
        return col + row * Width;
    }
    public Vector2 GetValue(int col, int row)
    {
        return Values[GetIndex(col, row)];
    }
    public Vector2 GetValue(float col, float row)
    {
        return GetValue((int)col, (int)row);
    }
}