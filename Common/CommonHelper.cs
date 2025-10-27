using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static GameReplay;
using static EnumData;
using static CreateSettingData;
using static SaveJsonData;

public static class CommonHelper
{

    public static void TestEnd(string str = "")
    {
        Debug.LogError($"TestEnd----{str}");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }



    public static string DifficultToString(Difficult difficult)
    {
        switch (difficult)
        {
            case Difficult.Easy:
                return "Easy";
            case Difficult.Normal:
                return "Normal";
            case Difficult.Hard:
                return "Hard";
            case Difficult.Lunatic:
                return "Lunatic";
        }
        return "";
    }

    public static uint CalcArithSum(uint n)
    {
        // 等差數列總和 1 + 2 + ... + n = n * (n + 1) / 2
        return (n * (n + 1)) / 2;
    }

    public static float CalAngle(Vector2 pa, Vector2 pb)
    {
        return Mathf.Atan2(pb.y - pa.y, pb.x - pa.x) * Mathf.Rad2Deg;
    }

    public static Vector2 CalPos(float angle, float distance)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad) * distance, Mathf.Sin(rad) * distance);
    }




}
