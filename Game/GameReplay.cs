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

public static class GameReplay
{
    public static uint keyPressTime = 0;
    public static ReplaySaveData InputSaveData = new ReplaySaveData();
    public static List<ReplayKey> playKeys = new List<ReplayKey>();
    public static uint playMaxKeyPressTime;
    public static bool isReplayMode
    {
        get
        {
            return playKeys != null;
        }
    }

    public static bool CheckPlayEnd()
    {
        return isReplayMode && playMaxKeyPressTime == keyPressTime;
    }

    public static void Init()
    {
        InputSaveData = new ReplaySaveData();
        InputSaveData.replayKeys = new List<ReplayKey>();
        InputSaveData.selectDifficult = GameSelect.difficult;
        InputSaveData.selectPracticeId = GameSelect.practiceId;
        keyPressTime = 0;
    }



    public static void UpdateHandler(out bool isSkipRemain)
    {
        isSkipRemain = false;
        keyPressTime++;
        if (GameReplay.CheckPlayEnd())
        {
            GameMainCtrl.Instance.Pause();
            ReplayOverSelect.Instance.Show();
            isSkipRemain = true;
        }
    }












}
