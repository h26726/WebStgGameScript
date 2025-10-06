using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System.Linq;
using System.IO;
using System.Xml;

public static class XmlStageSettingBuilder
{
    public class XmlStageSetting
    {
        public string table = "";
        public uint Order = 0;
        public TypeValue Type = TypeValue.None;
        public PosParamType PosParamTypeVal = PosParamType.None;
        public AngleParamType AngleParamTypeVal = AngleParamType.None;


        public string[] ParamVals = new string[0];
    }
    public static List<XmlStageSetting> BuildByReadFileName(string name)
    {
        var xmlStageSettings = new List<XmlStageSetting>();
        ReadAndMergeXmlSettings(name, ref xmlStageSettings);
        return xmlStageSettings;
    }

    public static List<XmlStageSetting> BuildByReadDirPath(string path)
    {
        var xmlStageSettings = new List<XmlStageSetting>();

        TextAsset[] xmlFiles = Resources.LoadAll<TextAsset>($"Setting/{path}");
        if (xmlFiles == null || xmlFiles.Length == 0)
        {
            // Debug.LogError($"找不到任何 XML 檔案在：Resources/Setting/{path}");
            return xmlStageSettings;
        }

        foreach (var xmlFile in xmlFiles)
        {
            string fileName = xmlFile.name;
            Debug.Log($"fileName xml: {fileName}");

            ReadAndMergeXmlSettings(path + "/" + fileName, ref xmlStageSettings);
        }
        return xmlStageSettings;
    }

    public static void ReadAndMergeXmlSettings(string name, ref List<XmlStageSetting> xmlStageSettings)
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

                StrToValFunc.Set(xe.GetAttribute("Order"), ref xmlStageSetting.Order);
                StrToValFunc.Set(xe.GetAttribute("Type"), ref xmlStageSetting.Type);
                StrToValFunc.Set(xe.GetAttribute("AngleParamType"), ref xmlStageSetting.AngleParamTypeVal);
                StrToValFunc.Set(xe.GetAttribute("PosParamType"), ref xmlStageSetting.PosParamTypeVal);
                StrToValFunc.Set(xe.GetElementsByTagName("ParamVals"), ref xmlStageSetting.ParamVals);

                xmlStageSettings.Add(xmlStageSetting);
            }
        }
        else
        {
            Debug.LogError($"找不到 XML 檔案：Setting/{name}");
        }
    }

}

