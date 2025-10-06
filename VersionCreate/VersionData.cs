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

[Serializable]
public class VersionData
{
    
    public VersionData(string version, VersionGetType getType)
    {
        this.version = version;
        if (getType == VersionGetType.InsideCreateByXml)
        {
            dialogSettings = ReadDialogXML(GameConfig.CONFIG_FILE_STR_DIALOG);
            practiceSettings = ReadPracticeXML(GameConfig.CONFIG_FILE_STR_PRACTICE);
            playerDatas = GetPlayerDatas();
            stageDatas = GetStageDatas();
            powerData = new PowerData(GameConfig.CONFIG_FILE_STR_POWER);
        }
        else
        {

        }
    }

    public List<PlayerData> playerDatas = new List<PlayerData>();

    public List<StageData> stageDatas = new List<StageData>();

    public PowerData powerData = null;

    public List<PracticeSetting> practiceSettings = new List<PracticeSetting>();
    public List<DialogSetting> dialogSettings;
    public string version;

    public List<PlayerData> GetPlayerDatas()
    {
        Debug.Log("ameConfig.PLAYER_LIST.Count:" + GameConfig.PLAYER_LIST.Count);

        foreach (var item in GameConfig.PLAYER_LIST)
        {
            Debug.Log("item.text:" + item.text);
        }
        var playerFileNames = GameConfig.PLAYER_LIST.Select(r => r.text).ToList();
        var newPlayerDatas = new List<PlayerData>();
        if (playerFileNames.Count > 0)
        {
            foreach (var playerFileName in playerFileNames)
            {
                newPlayerDatas.Add(new PlayerData(playerFileName));
            }

        }
        else
        {
            Debug.LogError("Player XML path is null.");
        }
        return newPlayerDatas;

        // string jsonStr = JsonHelper.ToJson(playerAndPowerCreateStageSettings.ToArray());
    }

    public List<StageData> GetStageDatas()
    {
        var newStageDatas = new List<StageData>();
        for (int i = 0; i < 7; i++)
        {
            foreach (Difficult difficulty in Enum.GetValues(typeof(Difficult)))
            {
                var stageData = new StageData((uint)i, difficulty);
                newStageDatas.Add(stageData);
            }
        }
        return newStageDatas;
    }

    
    public List<DialogSetting> ReadDialogXML(string name)
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
                StrToValFunc.Set(xe.GetAttribute("Id"), ref dialogSetting.Id);
                StrToValFunc.Set(xe.GetAttribute("MainId"), ref dialogSetting.mainId);
                StrToValFunc.Set(xe.GetAttribute("Text"), ref dialogSetting.text);
                StrToValFunc.Set(xe.GetAttribute("LeftAni"), ref dialogSetting.leftAni);
                StrToValFunc.Set(xe.GetAttribute("RightAni"), ref dialogSetting.rightAni);
                StrToValFunc.Set(xe.GetAttribute("Bgm"), ref dialogSetting.bgm);

                list.Add(dialogSetting);
            }
        }
        return list;
    }


    public List<PracticeSetting> ReadPracticeXML(string name)
    {
        var list = new List<PracticeSetting>();
        TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{name}");
        if (xmlAsset != null)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlNodeList nodeList = xml.SelectSingleNode("root").ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                var practiceSetting = new PracticeSetting { };
                StrToValFunc.Set(xe.GetAttribute("Id"), ref practiceSetting.Id);
                StrToValFunc.Set(xe.GetAttribute("Name"), ref practiceSetting.name);
                StrToValFunc.Set(xe.GetAttribute("BossEnterTime"), ref practiceSetting.bossEnterTime);
                StrToValFunc.Set(xe.GetAttribute("BossSpellTime"), ref practiceSetting.bossSpellTime);
                StrToValFunc.Set(xe.GetAttribute("Music"), ref practiceSetting.music);
                list.Add(practiceSetting);
            }
        }
        return list;
    }






}