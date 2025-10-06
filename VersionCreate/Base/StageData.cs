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

[Serializable]
public class StageData
{
    public StageData(uint stageKey, Difficult difficult)
    {
        this.stageKey = stageKey;
        this.difficult = difficult;
        var stageFileDirName = GetStageFileDirName();
        var stageXmlStageSettings = XmlStageSettingBuilder.BuildByReadDirPath(stageFileDirName);
        CreateDataBuilder.Build(
            stageXmlStageSettings, out _, out callRuleSchemesByGTime, out callRuleSchemeById);
    }
    public List<CallRuleScheme> callRuleSchemesByGTime = new List<CallRuleScheme>();
    public List<CallRuleScheme> callRuleSchemeById = new List<CallRuleScheme>();
    public Difficult difficult;
    public uint stageKey;

    public string GetStageFileDirName()
    {
        var stageFileDirName = "";
        if (difficult == Difficult.Easy)
        {
            stageFileDirName = GameConfig.EASY_STAGES[(int)stageKey].text;
        }
        else if (difficult == Difficult.Normal)
        {
            stageFileDirName = GameConfig.NORMAL_STAGES[(int)stageKey].text;
        }
        else if (difficult == Difficult.Hard)
        {
            stageFileDirName = GameConfig.HARD_STAGES[(int)stageKey].text;
        }
        else if (difficult == Difficult.Lunatic)
        {
            stageFileDirName = GameConfig.LUNATIC_STAGES[(int)stageKey].text;
        }

        return stageFileDirName;
    }
}