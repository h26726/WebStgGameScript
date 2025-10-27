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
using static SaveJsonData;
using System.Linq;
using System.IO;
using System.Xml;

[Serializable]
public class VersionData
{


    public VersionData(string version)
    {
        this.version = version;
        this.dialogSettings = ReadDialogXML(version, GameConfig.CONFIG_FILE_STR_DIALOG);
        this.practiceSettings = ReadPracticeXML(version, GameConfig.CONFIG_FILE_STR_PRACTICE);
        this.playerDatas = GetPlayerDatas(version);
        this.stageDatas = GetStageDatas(version);
        var powerData = new PowerData();
        powerData.Set(version, GameConfig.CONFIG_FILE_STR_POWER);
        this.powerData = powerData;
    }

    public string version;
    public List<PlayerData> playerDatas;

    public List<StageData> stageDatas;

    public PowerData powerData;

    public List<PracticeSetting> practiceSettings;
    public List<DialogSetting> dialogSettings;

    public List<PlayerData> GetPlayerDatas(string version)
    {
        // Debug.Log("ameConfig.PLAYER_LIST.Count:" + GameConfig.PLAYER_LIST.Count);

        // foreach (var item in GameConfig.PLAYER_LIST)
        // {
        //     Debug.Log("item.text:" + item.text);
        // }
        var playerFileNames = GameConfig.PLAYER_LIST.Select(r => r.text).ToList();
        var newPlayerDatas = new List<PlayerData>();
        if (playerFileNames.Count > 0)
        {
            foreach (var playerFileName in playerFileNames)
            {
                var playerData = new PlayerData();
                playerData.Set(version, playerFileName);
                newPlayerDatas.Add(playerData);
            }

        }
        else
        {
            Debug.LogError("Player XML path is null.");
        }
        return newPlayerDatas;

        // string jsonStr = JsonHelper.ToJson(playerAndPowerCreateStageSettings.ToArray());
    }

    public List<StageData> GetStageDatas(string version)
    {
        var newStageDatas = new List<StageData>();
        for (int i = 0; i < 7; i++)
        {
            foreach (Difficult difficulty in Enum.GetValues(typeof(Difficult)))
            {
                var stageData = new StageData();
                stageData.Set(version, (uint)i, difficulty);
                newStageDatas.Add(stageData);
            }
        }
        return newStageDatas;
    }


    public static List<DialogSetting> ReadDialogXML(string version, string name)
    {
        var list = new List<DialogSetting>();
        try
        {
            TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{version}/{name}");
            if (xmlAsset == null)
            {
                Debug.LogWarning($"[ReadDialogXML] XML 檔案不存在: Setting/{version}/{name}");
                return list;
            }

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlNode root = xml.SelectSingleNode("root");
            if (root == null)
            {
                Debug.LogWarning($"[ReadDialogXML] root 節點缺失: {name}");
                return list;
            }

            foreach (XmlElement xe in root.ChildNodes)
            {
                try
                {
                    var dialogSetting = new DialogSetting();

                    StrToValFunc.Set(xe.GetAttribute("Id"), ref dialogSetting.Id);
                    StrToValFunc.Set(xe.GetAttribute("MainId"), ref dialogSetting.mainId);
                    StrToValFunc.Set(xe.GetAttribute("Text"), ref dialogSetting.text);
                    StrToValFunc.Set(xe.GetAttribute("LeftAni"), ref dialogSetting.leftAni);
                    StrToValFunc.Set(xe.GetAttribute("RightAni"), ref dialogSetting.rightAni);
                    StrToValFunc.Set(xe.GetAttribute("Bgm"), ref dialogSetting.bgm);

                    list.Add(dialogSetting);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ReadDialogXML] 無法解析節點 ({name}): {xe.OuterXml}\n{e}");
                }
            }
        }
        catch (XmlException e)
        {
            Debug.LogError($"[ReadDialogXML] XML 格式錯誤 ({name}): {e}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ReadDialogXML] 未預期錯誤 ({name}): {e}");
        }

        return list;
    }

    public static List<PracticeSetting> ReadPracticeXML(string version, string name)
    {
        var list = new List<PracticeSetting>();
        try
        {
            TextAsset xmlAsset = Resources.Load<TextAsset>($"Setting/{version}/{name}");
            if (xmlAsset == null)
            {
                Debug.LogWarning($"[ReadPracticeXML] XML 檔案不存在: Setting/{version}/{name}");
                return list;
            }

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlNode root = xml.SelectSingleNode("root");
            if (root == null)
            {
                Debug.LogWarning($"[ReadPracticeXML] root 節點缺失: {name}");
                return list;
            }

            foreach (XmlElement xe in root.ChildNodes)
            {
                try
                {
                    var practiceSetting = new PracticeSetting();

                    StrToValFunc.Set(xe.GetAttribute("Id"), ref practiceSetting.Id);
                    StrToValFunc.Set(xe.GetAttribute("Name"), ref practiceSetting.name);
                    StrToValFunc.Set(xe.GetAttribute("BossEnterTime"), ref practiceSetting.bossEnterTime);
                    StrToValFunc.Set(xe.GetAttribute("BossSpellTime"), ref practiceSetting.bossSpellTime);
                    StrToValFunc.Set(xe.GetAttribute("Music"), ref practiceSetting.music);

                    list.Add(practiceSetting);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ReadPracticeXML] 無法解析節點 ({name}): {xe.OuterXml}\n{e}");
                }
            }
        }
        catch (XmlException e)
        {
            Debug.LogError($"[ReadPracticeXML] XML 格式錯誤 ({name}): {e}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ReadPracticeXML] 未預期錯誤 ({name}): {e}");
        }

        return list;
    }






}