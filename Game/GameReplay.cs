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
using static SaveJsonData;
using static GameConfig;
using System.Linq;
using static LoadCtrl;

public static class GameReplay
{
    public static uint keyPressTime = 0;
    public static ReplaySaveData InputSaveData;
    public static bool isReplayMode;
    public static uint playMaxKeyPressTime;
    // public static List<ReplayKey> playKeys;
    public static Dictionary<uint, List<KeyCode>> playKeyDict;

    public static void Init()
    {
        InputSaveData = new ReplaySaveData();
        // InputSaveData.tmpReplayKeys = new List<ReplayKey>();
        playKeyDict = new Dictionary<uint, List<KeyCode>>();
        ResetInput();
        ResetRead();
    }

    public static void SetPlayReplayData(List<ReplayKey> playKeys)
    {
        playKeyDict.Clear(); // 清空舊資料以防重複 key

        int count = playKeys.Count;
        Debug.Log("playKeys.Count:" + playKeys.Count);
        uint maxKeyTime = 0;
        for (int i = 0; i < count; i++)
        {
            var playKey = playKeys[i];
            playKeyDict[playKey.keyPressTime] = playKey.pressKeyCodes;

            if (playKey.keyPressTime > maxKeyTime)
                maxKeyTime = playKey.keyPressTime;
        }
        playMaxKeyPressTime = maxKeyTime;
    }

    public static void ResetInput()
    {
        keyPressTime = 0;
        InputSaveData.Reset();
    }

    public static void ResetRead()
    {
        isReplayMode = false;
        playMaxKeyPressTime = 0;
        playKeyDict.Clear();
    }

    public static bool CheckPlayKeyExist()
    {
        if (!playKeyDict.ContainsKey(keyPressTime))
            return false;
        return true;
    }

    public static List<KeyCode> GetNowPlayKeyCodes()
    {
        return playKeyDict[keyPressTime];
    }

    public static bool CheckPlayEnd()
    {
        return isReplayMode && playMaxKeyPressTime == keyPressTime;
    }

    public static void GameStartSet()
    {
        InputSaveData.version = GameConfig.VERSION;
        InputSaveData.selectDifficult = GameSelect.difficult;
        InputSaveData.selectPracticeId = GameSelect.practiceId;
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
