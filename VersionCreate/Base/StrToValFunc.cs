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
using static CommonHelper;
using static CreateSettingData;
using static SaveJsonData;
using static EnumData;

public static class StrToValFunc
{
    public static void Set(string str, ref Vector2 data)
    {
        if (string.IsNullOrEmpty(str)) return;

        if (!str.Contains(","))
        {
            TestEnd($"Set:{str}");
        }

        var strs = str.Split(",");
        if (strs.Length != 2)
        {
            TestEnd($"Set:{str}");
        }

        float x = 0, y = 0;

        if (!float.TryParse(strs[0], out x))
        {
            TestEnd($"Set:{str}");
        }

        if (!float.TryParse(strs[1], out y))
        {
            TestEnd($"Set:{str}");
        }

        data = !InvalidHelper.IsInvalid(data) ? data : Vector2.zero;

        data = new Vector2(data.x + x, data.y + y);
    }
    public static void Set(string str, ref uint[] data)
    {
        if (str == "") return;
        if (!str.Contains(","))
        {
            TestEnd($"Set:{str}");
        }
        var strs = str.Split(",");
        Set(strs, ref data);
    }

    public static void Set(string[] strs, ref uint[] data)
    {
        data = new uint[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            if (uint.TryParse(strs[i], out var val))
            {
                data[i] += val;
            }
            else
            {
                TestEnd($"Set:{strs[i]}");
            };
        }
    }



    private static uint Set(string str, uint original)
    {
        if (string.IsNullOrEmpty(str)) return original;
        if (uint.TryParse(str, out var val))
            return original + val;
        else
            TestEnd($"Set:{str}");
        return original;
    }

    public static void Set(string str, ref uint data)
    {
        data = !InvalidHelper.IsInvalid(data) ? data : 0;
        data = Set(str, data);
    }

    private static float Set(string str, float original)
    {
        if (string.IsNullOrEmpty(str)) return original;
        if (float.TryParse(str, out var val))
            return original + val;
        else
            TestEnd($"Set:{str}");
        return original;
    }

    public static void Set(string str, ref float data)
    {
        data = !InvalidHelper.IsInvalid(data) ? data : 0;
        data = Set(str, data);
    }
    public static void Set(string str, ref bool data)
    {
        if (str == "") return;
        if (bool.TryParse(str, out var val))
        {
            data = val;
        }
        else
        {
            TestEnd($"Set:{str}");
        };
    }

    public static void Set(string str, ref BoolState data)
    {
        if (str == "") return;
        if (bool.TryParse(str, out var val))
        {
            data = val ? BoolState.True : BoolState.False;
        }
        else
        {
            TestEnd($"Set:{str}");
        };
    }
    public static void Set(string str, ref string data)
    {
        if (str == "") return;
        data = str;
    }
    public static void Set(string str, ref TypeValue data)
    {
        if (str == "") return;
        if (uint.TryParse(str, out var val))
        {
            data = (TypeValue)val;
        }
        else
        {
            TestEnd($"Set:{str}");
        };
    }

    public static void Set(string str, ref AngleParamType data)
    {
        if (str == "") return;
        if (uint.TryParse(str, out var val))
        {
            data = (AngleParamType)val;
        }
        else
        {
            TestEnd($"Set:{str}");
        };
    }
    public static void Set(string str, ref PosParamType data)
    {
        if (str == "") return;
        if (uint.TryParse(str, out var val))
        {
            data = (PosParamType)val;
        }
        else
        {
            TestEnd($"Set:{str}");
        };
    }

    public static void Set(XmlNodeList content, ref string[] data)
    {
        data = new string[0];
        if (content != null)
        {
            data = new string[content.Count];
            for (int i = 0; i < content.Count; i++)
            {
                data[i] = content[i].InnerText;
            }
        }
        else
        {
            TestEnd($"Set:{content}");
        }
    }
}
