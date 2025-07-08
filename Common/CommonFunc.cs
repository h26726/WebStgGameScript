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
using static GameSystem;
using static CommonData;

public static class CommonFunc
{


    public static void ReadPathXmlToSetting(string path, ref List<XmlStageSetting> xmlStageSettings)
    {
        TextAsset[] xmlFiles = Resources.LoadAll<TextAsset>($"Setting/{path}");

        if (xmlFiles == null || xmlFiles.Length == 0)
        {
            Debug.LogError($"找不到任何 XML 檔案在：Resources/Setting/{path}");
            return;
        }

        foreach (var xmlFile in xmlFiles)
        {
            string fileName = xmlFile.name; 
            Debug.Log($"fileName xml: {fileName}");

            ReadXMLToSetting(path + "/" + fileName, ref xmlStageSettings);
        }
    }

    public static void ReadXMLToSetting(string name, ref List<XmlStageSetting> xmlStageSettings)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{name}");

        if (xmlAsset != null)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlNodeList nodeList = xml.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                if (xe.GetAttribute("Order") == "" || xe.GetAttribute("Type") == "") continue;

                var xmlStageSetting = new XmlStageSetting { };
                xmlStageSetting.table = name;

                SetFormString(xe.GetAttribute("Order"), ref xmlStageSetting.Order);
                SetFormString(xe.GetAttribute("Type"), ref xmlStageSetting.Type);
                SetFormString(xe.GetAttribute("AngleParamType"), ref xmlStageSetting.AngleParamTypeVal);
                SetFormString(xe.GetAttribute("PosParamType"), ref xmlStageSetting.PosParamTypeVal);
                SetFormString(xe.GetElementsByTagName("ParamVals"), ref xmlStageSetting.ParamVals);

                xmlStageSettings.Add(xmlStageSetting);
            }
        }
        else
        {
            Debug.LogError($"找不到 XML 檔案：Setting/{name}");
        }
    }

    public static List<ConfigParam> ReadConfigXML(string name)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{name}");
        List<ConfigParam> configParams = new List<ConfigParam>();
        if (xmlAsset != null)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlNodeList nodeList = xml.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                var str = xe.GetAttribute("Str");
                var preLayerNo = xe.GetAttribute("PreLayerNo");

                if (!string.IsNullOrEmpty(xe.GetAttribute("Float")))
                {
                    var floatVal = float.Parse(xe.GetAttribute("Float"));
                    configParams.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        floatVal = floatVal,
                    });
                }
                else if (!string.IsNullOrEmpty(xe.GetAttribute("Int")))
                {
                    var intVal = int.Parse(xe.GetAttribute("Int"));
                    configParams.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        intVal = intVal,
                    });
                }
                else if (!string.IsNullOrEmpty(xe.GetAttribute("Pos")))
                {
                    var strs = xe.GetAttribute("Pos").Split(',');
                    var pos = new Vector2(float.Parse(strs[0]), float.Parse(strs[1]));
                    configParams.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        pos = pos,
                    });
                }
                else if (!string.IsNullOrEmpty(xe.GetAttribute("Text")))
                {
                    var text = xe.GetAttribute("Text");
                    configParams.Add(new ConfigParam
                    {
                        key = str,
                        PreLayerNo = preLayerNo,
                        text = text,
                    });
                }
            }
        }
        else
        {
            Debug.LogError($"找不到 XML 檔案：Setting/{name}");
        }
        return configParams;
    }

    public static List<DialogSetting> ReadDialogXML(string name)
    {
        var list = new List<DialogSetting>();
        TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{name}");
        if (xmlAsset != null)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text); 

            XmlNodeList nodeList = xml.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                var dialogSetting = new DialogSetting { };
                SetFormString(xe.GetAttribute("Id"), ref dialogSetting.Id);
                SetFormString(xe.GetAttribute("MainId"), ref dialogSetting.mainId);
                SetFormString(xe.GetAttribute("Text"), ref dialogSetting.text);
                SetFormString(xe.GetAttribute("LeftAni"), ref dialogSetting.leftAni);
                SetFormString(xe.GetAttribute("RightAni"), ref dialogSetting.rightAni);
                SetFormString(xe.GetAttribute("Bgm"), ref dialogSetting.bgm);

                list.Add(dialogSetting);
            }
        }
        return list;
    }


    public static Dictionary<uint, PracticeSetting> ReadPracticeXML(string name)
    {
        var list = new Dictionary<uint, PracticeSetting>();
        TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{name}");
        if (xmlAsset != null)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text); 

            XmlNodeList nodeList = xml.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                var practiceSetting = new PracticeSetting { };
                SetFormString(xe.GetAttribute("Id"), ref practiceSetting.Id);
                SetFormString(xe.GetAttribute("Name"), ref practiceSetting.name);
                SetFormString(xe.GetAttribute("BossEnterTime"), ref practiceSetting.bossEnterTime);
                SetFormString(xe.GetAttribute("BossSpellTime"), ref practiceSetting.bossSpellTime);
                SetFormString(xe.GetAttribute("Music"), ref practiceSetting.music);
                list.Add(practiceSetting.Id, practiceSetting);
            }
        }
        return list;
    }

    public static void SetFormString(string str, ref Vector2? data)
    {
        if (string.IsNullOrEmpty(str)) return;

        if (!str.Contains(","))
        {
            TestEnd($"SetFormString:{str}");
        }

        var strs = str.Split(",");
        if (strs.Length != 2)
        {
            TestEnd($"SetFormString:{str}");
        }

        float x = 0, y = 0;

        if (!float.TryParse(strs[0], out x))
        {
            TestEnd($"SetFormString:{str}");
        }

        if (!float.TryParse(strs[1], out y))
        {
            TestEnd($"SetFormString:{str}");
        }

        data ??= Vector2.zero;
        data = new Vector2(data.Value.x + x, data.Value.y + y);
    }
    public static void SetFormString(string str, ref uint[] data)
    {
        if (str == "") return;
        if (!str.Contains(","))
        {
            TestEnd($"SetFormString:{str}");
        }
        var strs = str.Split(",");
        SetFormString(strs, ref data);
    }

    public static void SetFormString(string[] strs, ref uint[] data)
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
                TestEnd($"SetFormString:{strs[i]}");
            };
        }
    }



    private static uint SetFromString(string str, uint original)
    {
        if (string.IsNullOrEmpty(str)) return original;
        if (uint.TryParse(str, out var val))
            return original + val;
        else
            TestEnd($"SetFormString:{str}");
        return original;
    }

    public static void SetFormString(string str, ref uint data)
    {
        data = SetFromString(str, data);
    }

    public static void SetFormString(string str, ref uint? data)
    {
        data ??= 0; 
        data = SetFromString(str, data.Value);
    }

    private static float SetFromString(string str, float original)
    {
        if (string.IsNullOrEmpty(str)) return original;
        if (float.TryParse(str, out var val))
            return original + val;
        else
            TestEnd($"SetFormString:{str}");
        return original;
    }

    public static void SetFormString(string str, ref float data)
    {
        data = SetFromString(str, data);
    }
    public static void SetFormString(string str, ref float? data)
    {
        data ??= 0; 
        data = SetFromString(str, data.Value);
    }
    public static void SetFormString(string str, ref bool? data)
    {
        if (str == "") return;
        if (bool.TryParse(str, out var val))
        {
            data = val;
        }
        else
        {
            TestEnd($"SetFormString:{str}");
        };
    }
    public static void SetFormString(string str, ref string data)
    {
        if (str == "") return;
        data = str;
    }
    public static void SetFormString(string str, ref TypeValue data)
    {
        if (str == "") return;
        if (uint.TryParse(str, out var val))
        {
            data = (TypeValue)val;
        }
        else
        {
            TestEnd($"SetFormString:{str}");
        };
    }

    public static void SetFormString(string str, ref AngleParamType data)
    {
        if (str == "") return;
        if (uint.TryParse(str, out var val))
        {
            data = (AngleParamType)val;
        }
        else
        {
            TestEnd($"SetFormString:{str}");
        };
    }
    public static void SetFormString(string str, ref PosParamType data)
    {
        if (str == "") return;
        if (uint.TryParse(str, out var val))
        {
            data = (PosParamType)val;
        }
        else
        {
            TestEnd($"SetFormString:{str}");
        };
    }

    public static void SetFormString(XmlNodeList content, ref string[] data)
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
            TestEnd($"SetFormString:{content}");
        }
    }




    public static void TestEnd(string str = "")
    {
        Debug.LogError($"TestEnd----{str}");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }


    public static float CalAngle(Vector2 pa, Vector2 pb)
    {
        return (float)(Math.Atan2(pb.y - pa.y, pb.x - pa.x) * 180 / Math.PI);
    }

    public static Vector2 CalPos(float angle, float distance)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * distance, Mathf.Sin(angle * Mathf.Deg2Rad) * distance);
    }

    public static uint CalcArithSum(uint n)
    {
        // 等差數列總和 1 + 2 + ... + n = n * (n + 1) / 2
        return (n * (n + 1)) / 2;
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

    public static bool CheckOutBorderDis(Vector2? Pos, float dis)
    {
        if (Pos.Value.x < GameConfig.BORDER_LEFT - dis) return true;
        else if (Pos.Value.x > GameConfig.BORDER_RIGHT + dis) return true;
        else if (Pos.Value.y < GameConfig.BD_BOTTOM - dis) return true;
        else if (Pos.Value.y > GameConfig.BD_TOP + dis) return true;
        return false;
    }



    public static UnitCtrlBase GetUnitCtrlById(UnitCtrlBase selfUnitCtrl, uint Id)
    {
        UnitCtrlBase unitCtrl = null;
        if ((IdVal)Id == IdVal.Self)
        {
            unitCtrl = selfUnitCtrl;
        }
        if ((IdVal)Id == IdVal.Parent)
        {
            unitCtrl = selfUnitCtrl.parentUnitCtrl;
        }
        else if ((IdVal)Id == IdVal.Player)
        {
            unitCtrl = GameSystem.Instance.playerUnitCtrl;
        }
        else if ((IdVal)Id == IdVal.Boss)
        {
            unitCtrl = GameSystem.Instance.nowEnemyBoss;
        }
        else
        {
            var units = GetUnitsById(Id);
            if (units.Count > 1)
            {
                Debug.LogError($"GetUnitById:{Id}>1");
            }
            else if (units.Count == 1)
            {
                unitCtrl = units.FirstOrDefault();
            }
        }
        return unitCtrl;
    }

    public static IUnitCtrlData GetUnitCtrlDataById(UnitCtrlBase selfUnitCtrl, uint Id)
    {
        return (IUnitCtrlData)GetUnitCtrlById(selfUnitCtrl, Id);
    }

    public static List<UnitCtrlBase> GetUnitsById(uint baseId)
    {
        var unitCtrls = GameSystem.Instance.takeDict.Values.Where(r => r.coreSetting.Id == baseId).ToList();
        return unitCtrls;
    }
}
