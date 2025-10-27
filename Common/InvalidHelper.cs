using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static EnumData;

public static class InvalidHelper
{
    public static bool IsInvalid(uint v) => v == GameConfig.UINT_INVAILD;
    public static bool IsInvalid(int v) => v == GameConfig.INT_INVAILD;
    // public static bool IsInvalid(float v) => float.IsNaN(v);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInvalid(float v, float epsilon = 0.01f)
    {
        return MathF.Abs(v - GameConfig.FLOAT_INVAILD) < epsilon;
    }
    public static bool IsInvalid(string v) => string.IsNullOrEmpty(v) || v == GameConfig.STRING_INVAILD;
    public static bool IsInvalid(BoolState v) => v == GameConfig.BOOL_INVAILD;
    public static bool IsInvalid(Vector2 v) => IsInvalid(v.x) || IsInvalid(v.y);
    public static bool IsInvalid(Vector3 v) => IsInvalid(v.x) || IsInvalid(v.y) || IsInvalid(v.z);
}