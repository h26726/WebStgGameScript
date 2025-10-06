using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;

public static class GameSelect
{
    public static Difficult difficult { get; set; } = Difficult.Normal;
    public static uint stageKey { get; set; } = 6;
    public static uint playerId { get; set; } = 101;
    public static uint practiceId { get; set; } = 0;
    public static bool isPracticeMode
    {
        get
        {
            return practiceId > 0;
        }
    }
    public static PracticeSetting practiceSetting
    {
        get
        {
            if (isPracticeMode)
            {
                return LoadCtrl.Instance.selectVersionData.practiceSettings.FirstOrDefault(r => r.Id == practiceId);
            }
            return null;
        }
    }

    // 整合載入過需保存的資料
    public static List<StageArrangeData> tmpSaveGameCtrlDatasList = new List<StageArrangeData>();

    public static StageArrangeData gameCtrlDatas = null;
    //依遊戲時間創造CallRuleScheme資料
    public static List<CallRuleScheme> callRuleSchemesByGTime
    {
        get
        {
            if (gameCtrlDatas == null)
            {
                Debug.LogError("select gameCtrlDatas is null in gTimeCreateCallRuleSchemes getter.");
                return null;
            }
            return gameCtrlDatas.callRuleSchemesByGTime;
        }
    }

    //依單位ID行動時創造或激活行動資料 
    public static Dictionary<uint, List<CallRuleScheme>> callRuleSchemeById
    {
        get
        {
            if (gameCtrlDatas == null)
            {
                Debug.LogError("select gameCtrlDatas is null in gTimeCreateCallRuleSchemes getter.");
                return null;
            }
            return gameCtrlDatas.callRulesSchemesDict;
        }
    }
    public static PlayerData playerData;

    public static PowerData powerData
    {
        get
        {
            return LoadCtrl.Instance.selectVersionData.powerData;
        }
    }

    public static uint CalArrivePractictTime(uint gTime)
    {
        if (isPracticeMode && gTime > practiceSetting.bossEnterTime && gTime < practiceSetting.bossSpellTime)
        {
            return practiceSetting.bossSpellTime;
        }
        return gTime;
    }

    public static bool CheckNowTimeCallIsPast(uint nowGTimeCallRuleSchemeKey, uint gTime, out uint skipTime)
    {
        if (callRuleSchemesByGTime[(int)nowGTimeCallRuleSchemeKey].callGameTime == null)
        {
            Debug.LogError($"nowGTimeCallRuleSchemeKey:{nowGTimeCallRuleSchemeKey} callGameTime is null in CheckNowCallIsPast.");
        }
        skipTime = callRuleSchemesByGTime[(int)nowGTimeCallRuleSchemeKey].callGameTime.Value;
        return callRuleSchemesByGTime[(int)nowGTimeCallRuleSchemeKey].callGameTime < gTime;
    }

    public static bool TryGetNowCallData(uint nowGTimeCallRuleSchemeKey, uint gTime, out (CreateStageSetting createStageSetting, uint? baseId, uint? actId) data)
    {
        data = (null, null, null);
        if (nowGTimeCallRuleSchemeKey < callRuleSchemesByGTime.Count && gTime == callRuleSchemesByGTime[(int)nowGTimeCallRuleSchemeKey].callGameTime)
        {
            data = GetNowCallData(nowGTimeCallRuleSchemeKey);
            return true;
        }
        return false;
    }

    public static (CreateStageSetting createStageSetting, uint? baseId, uint? actId) GetNowCallData(uint nowGTimeCallRuleSchemeKey)
    {
        var callRuleScheme = callRuleSchemesByGTime[(int)nowGTimeCallRuleSchemeKey];
        return (callRuleScheme.createStageSetting, callRuleScheme.coreId, callRuleScheme.actId);
    }

    public static void NextStage()
    {
        stageKey++;
        LoadCtrl.Instance.SwitchPage(PageIndex.Game);
    }

    public static void InitPlayerAndGameCtrlDatas()
    {
        playerData = LoadCtrl.Instance.selectVersionData.playerDatas.FirstOrDefault(r => r.Id == playerId);
        if (tmpSaveGameCtrlDatasList.Any(r => r.selectDifficult == difficult && r.selectStageKey == stageKey))
        {
            gameCtrlDatas = tmpSaveGameCtrlDatasList.FirstOrDefault(r => r.selectDifficult == difficult && r.selectStageKey == stageKey);
        }
        else
        {
            gameCtrlDatas = new StageArrangeData(LoadCtrl.Instance.selectVersionData, difficult, stageKey);
            tmpSaveGameCtrlDatasList.Add(gameCtrlDatas);
        }

        if (!LoadCtrl.Instance.selectVersionData.playerDatas.Any(r => r.Id == playerId))
        {
            Debug.LogError("No matching playerData found for the selected player ID.");
        }

        if (callRuleSchemesByGTime.Count == 0)
        {
            Debug.LogError("No CallRuleSchemes found for the current game time.");
        }

        if (playerData == null || playerData.playerCreateStageSetting == null)
        {
            Debug.LogError("playerCreateStageSetting is null in waitCreates callback.");
        }


    }
    public static void DefaultSelectDifficultStageByConfigParam()
    {
        var configSelectDifficultParams = GameConfig.CONFIG_PARAMS.FirstOrDefault(r => r.key == GameConfig.CONFIG_SELECT_DIFFICULT_STR);
        if (configSelectDifficultParams != null)
            difficult = (Difficult)configSelectDifficultParams.intVal;

        var configSelectStageKeyParams = GameConfig.CONFIG_PARAMS.FirstOrDefault(r => r.key == GameConfig.CONFIG_SELECT_STAGE_KEY_STR);
        if (configSelectStageKeyParams != null)
            stageKey = (uint)configSelectStageKeyParams.intVal;
    }














}
