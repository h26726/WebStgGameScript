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

[Serializable]
public class StageData
{
    public void Set(string version, uint stageKey, Difficult difficult)
    {
        this.stageKey = stageKey;
        this.difficult = difficult;
        var (stageFileDirName, bgm) = GetStageFileDirNameAndBgm();
        this.bgm = bgm;

        var stageXmlStageSettings = XmlStageSettingBuilder.BuildByReadDirPath(version, stageFileDirName);
        CreateDataBuilder.Build(
            stageXmlStageSettings, out _, out callRuleSchemesByGTime, out callRuleSchemeById);
    }
    public List<CallRuleScheme> callRuleSchemesByGTime = new List<CallRuleScheme>();
    public List<CallRuleScheme> callRuleSchemeById = new List<CallRuleScheme>();
    public Difficult difficult;
    public uint stageKey;
    public string bgm;

    public (string stageFileDirName, string bgm) GetStageFileDirNameAndBgm()
    {
        var stageFileDirName = "";
        var bgm = "";
        if (difficult == Difficult.Easy)
        {
            stageFileDirName = GameConfig.EASY_STAGES[(int)stageKey].text;
            bgm = GameConfig.EASY_STAGES[(int)stageKey].text2;
        }
        else if (difficult == Difficult.Normal)
        {
            stageFileDirName = GameConfig.NORMAL_STAGES[(int)stageKey].text;
            bgm = GameConfig.NORMAL_STAGES[(int)stageKey].text2;
        }
        else if (difficult == Difficult.Hard)
        {
            stageFileDirName = GameConfig.HARD_STAGES[(int)stageKey].text;
            bgm = GameConfig.HARD_STAGES[(int)stageKey].text2;
        }
        else if (difficult == Difficult.Lunatic)
        {
            stageFileDirName = GameConfig.LUNATIC_STAGES[(int)stageKey].text;
            bgm = GameConfig.LUNATIC_STAGES[(int)stageKey].text2;
        }
        return (stageFileDirName, bgm);
    }
}