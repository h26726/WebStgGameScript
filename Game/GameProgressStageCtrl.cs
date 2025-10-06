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

public static class GameProgressStageCtrl
{

    public static uint nowGTimeCallRuleSchemeKey = 0;
    public static uint gTime = 0;

    public static void Init()
    {
        if (GameSelect.isPracticeMode)
        {
            gTime = GameSelect.practiceSetting.bossEnterTime - 100;
            LoadCtrl.Instance.pool.PlayBgm(GameSelect.practiceSetting.music);
        }
        else
        {
            gTime = InitGTimeByConfig();
            PlayStartBgmByGTime();
        }
        nowGTimeCallRuleSchemeKey = InitNowCallKeyByGTime();

    }
    static uint InitNowCallKeyByGTime()
    {
        uint key = 0;
        while (gTime >= GameSelect.callRuleSchemesByGTime[(int)key].callGameTime)
        {
            Debug.Log($"skip CallGameTime:{GameSelect.callRuleSchemesByGTime[(int)key].callGameTime}");
            key++;
        }
        return key;
    }

    static void PlayStartBgmByGTime()
    {
        if (gTime == 0)
            return;

        var bgm = GameConfig.CONFIG_PARAMS.FirstOrDefault(r => r.key == "BGM")?.text;
        var bgmStart = GameConfig.CONFIG_PARAMS.FirstOrDefault(r => r.key == "BGMStart")?.intVal;
        if (!string.IsNullOrEmpty(bgm))
        {
            LoadCtrl.Instance.pool.PlayBgm(bgm, () =>
            {
                SetBgmStartSecond(bgmStart);
            });
        }
        else
        {
            Debug.LogWarning("BGM not found in CONFIG_PARAMS.");
        }
    }

    static void SetBgmStartSecond(int? bgmStart)
    {
        int frameTime = bgmStart ?? (int)gTime;
        LoadCtrl.Instance.audioSource.time = frameTime / (float)GameConfig.TARGET_FRAME_RATE;
    }
    static uint InitGTimeByConfig()
    {
        var gameTimeConfig = GameConfig.CONFIG_PARAMS.FirstOrDefault(r => r.key == "GameTime");
        return gameTimeConfig != null && gameTimeConfig.intVal.HasValue
            ? (uint)gameTimeConfig.intVal.Value
            : 0;
    }
    public static void UpdateHandler()
    {
        gTime++;
        gTime = GameSelect.CalArrivePractictTime(gTime);
        (gTime, nowGTimeCallRuleSchemeKey) = CalArriveConfigArrangeTime(gTime, nowGTimeCallRuleSchemeKey);

        while (GameSelect.TryGetNowCallData(nowGTimeCallRuleSchemeKey, gTime, out var callData))
        {
            var (createStageSetting, coreId, actId) = callData;
            if (createStageSetting != null)
            {
                QueneDebutHandle(createStageSetting);
            }
            else if (coreId != null && actId != null)
            {
                GameDebut.waitCallActs.Add((coreId.Value, actId.Value));
            }
            nowGTimeCallRuleSchemeKey++;
        }
    }

    static (uint gTime, uint nowGTimeCallRuleSchemeKey) CalArriveConfigArrangeTime(uint gTime, uint nowGTimeCallRuleSchemeKey)
    {
        if (gTime % 100 == 0)
        {
            //測試用
            while (CheckConfigSkipTime(gTime))
            {
                gTime += 100;
            }
            Debug.Log($"gTime:{gTime}");

            while (GameSelect.CheckNowTimeCallIsPast(nowGTimeCallRuleSchemeKey, gTime, out var skiptime))
            {
                Debug.Log($"CheckNowCallIsPast skiptime:{skiptime}");
                nowGTimeCallRuleSchemeKey++;
            }
        }
        return (gTime, nowGTimeCallRuleSchemeKey);
    }

    static void QueneDebutHandle(CreateStageSetting createStageSetting)
    {
        var coreSetting = createStageSetting.coreSetting;
        GameDebut.isNotDebutEnemyShot = false;
        switch (createStageSetting.type)
        {
            case TypeValue.對話:
                LoadCtrl.Instance.pool.PlayDialog(coreSetting.obj, coreSetting.Id);
                break;

            case TypeValue.標題:
                LoadCtrl.Instance.pool.PlayTitle(coreSetting.ani);
                break;

            case TypeValue.播放音樂:
                LoadCtrl.Instance.pool.PlayBgm(coreSetting.obj);
                break;

            // case TypeValue.下一關:
            //     GameSelect.NextStage();
            //     break;

            case TypeValue.BOSSDEAD:
                GameMainCtrl.Instance.nowGameProgressState = GameProgressState.Stage;
                GameBoss.nowUnit.unitProp.isTriggerDead = true;
                break;

            case TypeValue.關卡結束:
                // GameEndSelect.Show();
                break;

            case TypeValue.符卡:
            case TypeValue.復位:
            case TypeValue.BOSSLEAVE:
            case TypeValue.BOSS:
                GameMainCtrl.Instance.EnterBossTime(createStageSetting, coreSetting);
                break;
            default:
                GameDebut.waitDebutByCreateSettings.Add(createStageSetting);
                break;
        }
    }



    static bool CheckConfigSkipTime(uint gTime)
    {
        return GameConfig.CONFIG_PARAMS.Any(r => r.key == "SkipTime" && gTime == r.intVal);
    }



}
