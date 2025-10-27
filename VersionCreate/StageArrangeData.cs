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
public class StageArrangeData
{

    public StageArrangeData(VersionData versionData, Difficult selectDifficult, uint selectStageKey)
    {
        this.version = versionData.version;
        this.selectDifficult = selectDifficult;
        this.selectStageKey = selectStageKey;
        var selectStageData = versionData.stageDatas.Where(r => r.difficult == selectDifficult && r.stageKey == selectStageKey).FirstOrDefault();
        if (selectStageData == null)
        {
            Debug.LogError("selectStageData is null. Stage data not found for the selected difficulty and stage key.");
        }
        this.bgm = selectStageData.bgm;

        this.callRuleSchemesByGTime = selectStageData.callRuleSchemesByGTime;

        //待優化
        foreach (var item in GameSelect.playerData.playerCallRuleSchemeById)
        {
            Debug.Log("item:" + item.Print());
        }
        var allCallRuleSchemeById = selectStageData.callRuleSchemeById.Concat(GameSelect.playerData.playerCallRuleSchemeById).Concat(GameSelect.powerData.powerCallRuleSchemeById).ToList();
        foreach (var callRuleScheme in allCallRuleSchemeById)
        {
            var callExistId = callRuleScheme.callExistId;
            if (InvalidHelper.IsInvalid(callExistId))
            {
                Debug.LogError("callExistId is null in callRuleScheme. Cannot add to callRulesSchemeDict.");
            }
            else if (!callRulesSchemesDict.ContainsKey(callExistId))
            {
                this.callRulesSchemesDict.Add(callExistId, new List<CallRuleScheme>());
            }
            this.callRulesSchemesDict[callExistId].Add(callRuleScheme);
        }
    }
    public string version;
    public Difficult selectDifficult;
    public uint selectStageKey;
    public string bgm;
    public List<CallRuleScheme> callRuleSchemesByGTime = new List<CallRuleScheme>();
    public Dictionary<uint, List<CallRuleScheme>> callRulesSchemesDict = new Dictionary<uint, List<CallRuleScheme>>();

}