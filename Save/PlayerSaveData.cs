using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;

public static class PlayerSaveData
{
    static uint _score = 0;
    public static uint score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            GameObjCtrl.Instance.UpdateScoreText();
        }
    }
    public static List<KeyBoardSaveData> keyBoardSaveDatas = new List<KeyBoardSaveData>();
    public static List<ReplaySaveData> replaySaveDatas = new List<ReplaySaveData>();
    public static ConfigSaveData configSaveDatas = new ConfigSaveData();

    // public static List<StageSettingData> playerStageSettingDatas = new List<StageSettingData>();

    public static string configSavePath = Application.persistentDataPath + "/Config.json";
    public static string keyBoardSavePath = Application.persistentDataPath + "/KeyBoard.json";
    public static string replaySavePath = Application.persistentDataPath + "/Replay.json";
    public static string playerStageDataPath = Application.persistentDataPath + "/playerStage.json";




    [System.Serializable]
    public class KeyBoardSaveData
    {
        public KeyCode baseKey;
        public KeyCode setKey;
    }
    public static KeyCode TransferToPlayerSetKey(KeyCode baseKey)
    {
        var data = keyBoardSaveDatas.FirstOrDefault(r => r.baseKey == baseKey);
        return data != null ? data.setKey : KeyCode.None;
    }

    public static KeyCode TransferToTmpSetKey(KeyCode baseKey, List<KeyBoardSaveData> tmpkeyBoardSaveData)
    {
        var data = tmpkeyBoardSaveData.FirstOrDefault(r => r.baseKey == baseKey);
        return data != null ? data.setKey : KeyCode.None;
    }

    [System.Serializable]
    public class ConfigSaveData
    {
        public uint BGMVolume = 60;
        public uint BGSVolume = 60;
        public ScreenMode screenModeType = ScreenMode.FullScreen;
    }
    [System.Serializable]
    public class ReplayKey
    {
        public uint keyPressTime;
        public List<KeyCode> pressKeyCodes = new List<KeyCode>();

    }

    [System.Serializable]
    public class ReplaySaveData
    {
        public uint No;
        public string name;
        public string time;
        public uint selectPracticeId;
        public Difficult selectDifficult;
        public List<ReplayKey> replayKeys;

        public ReplaySaveData Copy()
        {
            var newData = new ReplaySaveData();
            newData.No = No;
            newData.name = name;
            newData.time = time;
            newData.selectPracticeId = selectPracticeId;
            newData.selectDifficult = selectDifficult;
            newData.replayKeys = replayKeys;
            return newData;
        }
    }





    public static List<KeyBoardSaveData> LoadKeyBoardSaveDatas()
    {
        try
        {
            if (!File.Exists(keyBoardSavePath))
            {
                keyBoardSaveDatas = DefaultKeyBoardDatas();
                string defaultJson = JsonHelper.ToJson(keyBoardSaveDatas.ToArray());
                File.WriteAllText(keyBoardSavePath, defaultJson);
                return keyBoardSaveDatas;
            }

            string json = File.ReadAllText(keyBoardSavePath);
            var datas = JsonHelper.FromJson<KeyBoardSaveData>(json);
            return datas?.ToList() ?? new List<KeyBoardSaveData>();
        }
        catch (Exception ex)
        {
            // 如果 JSON 壞掉或其他 I/O 問題，回傳預設資料並覆寫檔案
            Debug.LogError($"LoadKeyBoardSaveDatas Error:{ex}");
            keyBoardSaveDatas = DefaultKeyBoardDatas();
            string fallbackJson = JsonHelper.ToJson(keyBoardSaveDatas.ToArray());
            File.WriteAllText(keyBoardSavePath, fallbackJson);
            return keyBoardSaveDatas;
        }
    }

    public static List<KeyBoardSaveData> DefaultKeyBoardDatas()
    {
        var newKeyBoardSaveDatas = new List<KeyBoardSaveData>();
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.UpArrow, setKey = KeyCode.UpArrow });
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.DownArrow, setKey = KeyCode.DownArrow });
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.LeftArrow, setKey = KeyCode.LeftArrow });
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.RightArrow, setKey = KeyCode.RightArrow });
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.Z, setKey = KeyCode.Z });
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.X, setKey = KeyCode.X });
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.LeftShift, setKey = KeyCode.LeftShift });
        newKeyBoardSaveDatas.Add(new KeyBoardSaveData { baseKey = KeyCode.Escape, setKey = KeyCode.Escape });
        return newKeyBoardSaveDatas;
    }

    public static ConfigSaveData LoadConfigSaveData()
    {
        try
        {
            if (!File.Exists(configSavePath))
            {
                // 建立預設設定
                configSaveDatas = new ConfigSaveData();
                string defaultJson = JsonUtility.ToJson(configSaveDatas, true);
                File.WriteAllText(configSavePath, defaultJson);
                return configSaveDatas;
            }

            string json = File.ReadAllText(configSavePath);
            return JsonUtility.FromJson<ConfigSaveData>(json) ?? new ConfigSaveData();
        }
        catch (Exception)
        {
            // 如果檔案壞掉就重建
            Debug.LogError($"LoadConfigSaveData Error");
            configSaveDatas = new ConfigSaveData();
            string fallbackJson = JsonUtility.ToJson(configSaveDatas, true);
            File.WriteAllText(configSavePath, fallbackJson);
            return configSaveDatas;
        }
    }
    public static void SaveKeyBoardData()
    {
        File.WriteAllText(keyBoardSavePath, JsonHelper.ToJson(keyBoardSaveDatas.ToArray()));
    }

    public static void SaveConfigSaveData()
    {
        File.WriteAllText(configSavePath, JsonUtility.ToJson(configSaveDatas));
    }

    public static List<ReplaySaveData> LoadReplaySaveDatas()
    {
        try
        {
            if (!File.Exists(replaySavePath))
            {
                // 建立空的預設檔案
                var emptyList = new List<ReplaySaveData>();
                string defaultJson = JsonHelper.ToJson(emptyList.ToArray());
                File.WriteAllText(replaySavePath, defaultJson);
                return emptyList;
            }

            string json = File.ReadAllText(replaySavePath);
            var datas = JsonHelper.FromJson<ReplaySaveData>(json);
            return datas?.ToList() ?? new List<ReplaySaveData>();
        }
        catch (Exception)
        {
            // 如果 JSON 壞掉就回傳空清單並重建檔案
            Debug.LogError($"LoadReplaySaveDatas Error");
            var emptyList = new List<ReplaySaveData>();
            string fallbackJson = JsonHelper.ToJson(emptyList.ToArray());
            File.WriteAllText(replaySavePath, fallbackJson);
            return emptyList;
        }
    }


    public static void SaveReplayData()
    {
        int index = replaySaveDatas.FindIndex(r => r.No == GameReplay.InputSaveData.No);
        if (index != -1)
        {
            replaySaveDatas[index] = GameReplay.InputSaveData;
        }
        else
        {
            replaySaveDatas.Add(GameReplay.InputSaveData.Copy());
        }

        File.WriteAllText(replaySavePath, JsonHelper.ToJson(replaySaveDatas.ToArray()));
    }





    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }




}
