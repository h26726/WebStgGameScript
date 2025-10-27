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

public static class SaveJsonData
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
    public static string versionSavePath = Application.persistentDataPath + "/Version.json";




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

        public void Reset()
        {
            keyPressTime = 0;
            pressKeyCodes.Clear();
        }

        public ReplayKey Copy()
        {
            // 深拷貝：建立全新的物件與 List
            ReplayKey copy = new ReplayKey();
            copy.keyPressTime = this.keyPressTime;
            copy.pressKeyCodes = new List<KeyCode>(this.pressKeyCodes);
            return copy;
        }

    }

    [System.Serializable]
    public class ReplaySaveData
    {
        public string version;
        public uint No;
        public uint selectPracticeId;
        public string name;
        public string time;
        public Difficult selectDifficult;
        [NonSerialized] public Dictionary<uint, List<KeyCode>> replayKeyDict;
        public List<ReplayKey> replayKeys;
        public uint replayKeyIndex;

        public ReplaySaveData(bool isSave = false)
        {
            replayKeyDict = new Dictionary<uint, List<KeyCode>>();
            replayKeys = new List<ReplayKey>();
            if (!isSave)
            {
                for (int i = 0; i < GameConfig.REPLAY_MAX_KEYTIME; i++)
                {
                    replayKeys.Add(new ReplayKey());
                }
            }
        }

        public void Reset()
        {
            version = "";
            No = 0;
            selectPracticeId = 0;
            name = null;
            time = null;
            selectDifficult = Difficult.Easy;
            for (int i = 0; i < replayKeyIndex; i++)
            {
                replayKeys[i].Reset();
            }
            replayKeyDict.Clear();
        }

        public void AddReplayKey(uint keyPressTime, KeyCode keyCode)
        {
            if (replayKeyDict.ContainsKey(keyPressTime))
            {
                replayKeyDict[keyPressTime].Add(keyCode);
            }
            else
            {
                var replayKey = replayKeys[(int)replayKeyIndex];
                replayKey.keyPressTime = keyPressTime;
                replayKey.pressKeyCodes.Add(keyCode);
                replayKeyDict.Add(keyPressTime, replayKey.pressKeyCodes);
                replayKeyIndex++;
            }
        }

        public ReplaySaveData Copy()
        {
            var newData = new ReplaySaveData(true);
            newData.version = GameConfig.VERSION;
            newData.No = No;
            newData.name = name;
            newData.time = time;
            newData.selectPracticeId = selectPracticeId;
            newData.selectDifficult = selectDifficult;
            newData.replayKeys = new List<ReplayKey>();
            for (int i = 0; i < replayKeyIndex; i++)
            {
                newData.replayKeys.Add(replayKeys[i].Copy());
            }
            return newData;
        }
    }

    public static List<KeyBoardSaveData> LoadKeyBoardSaveDatas()
    {
        try
        {
            if (!File.Exists(keyBoardSavePath))
            {
                var emptySaveDatas = DefaultKeyBoardDatas();
                SaveKeyBoardData(keyBoardSavePath, emptySaveDatas);
                return emptySaveDatas;
            }

            string json = File.ReadAllText(keyBoardSavePath);
            var datas = JsonHelper.FromJson<KeyBoardSaveData>(json);
            if (datas == null)
                throw new Exception("KeyBoardSaveData JSON parse returned null");

            return datas.ToList();
        }
        catch (Exception ex)
        {
            // 如果 JSON 壞掉或其他 I/O 問題，回傳預設資料並覆寫檔案
            Debug.LogError($"LoadKeyBoardSaveDatas Error:{ex}");
            var emptySaveDatas = DefaultKeyBoardDatas();
            SaveKeyBoardData(keyBoardSavePath, emptySaveDatas);
            return emptySaveDatas;
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
                var emptyDatas = new ConfigSaveData();
                SaveConfigSaveData(configSavePath, emptyDatas);
                return emptyDatas;
            }

            string json = File.ReadAllText(configSavePath);
            var datas = JsonUtility.FromJson<ConfigSaveData>(json);
            if (datas == null)
                throw new Exception("KeyBoardSaveData JSON parse returned null");

            return datas;
        }
        catch (Exception)
        {
            // 如果檔案壞掉就重建
            Debug.LogError($"LoadConfigSaveData Error");
            var emptyDatas = new ConfigSaveData();
            SaveConfigSaveData(configSavePath, emptyDatas);
            return emptyDatas;
        }
    }
    public static void SaveKeyBoardData()
    {
        SaveKeyBoardData(keyBoardSavePath, keyBoardSaveDatas);
    }

    public static void SaveKeyBoardData(string path, List<KeyBoardSaveData> newKeyBoardSaveDatas)
    {
        File.WriteAllText(path, JsonHelper.ToJson(newKeyBoardSaveDatas.ToArray()));
    }


    public static void SaveVersionData()
    {
        SaveVersionData(versionSavePath, LoadCtrl.Instance.versionDatas);
        VersionDataJsonHandler.SaveVersionDataList(versionSavePath, LoadCtrl.Instance.versionDatas);
    }

    public static void SaveVersionData(string path, List<VersionData> newVersionDatas)
    {
        Debug.Log("newVersionDatas.ToArray().Count:" + newVersionDatas.ToArray().Length);
        File.WriteAllText(path, JsonHelper.ToJson(newVersionDatas.ToArray()));
    }

    public static List<VersionData> LoadVersionSaveDatas()
    {
        try
        {
            return VersionDataJsonHandler.LoadVersionDataList(versionSavePath);
            // string json = File.ReadAllText(versionSavePath);
            // var datas = JsonHelper.FromJson<VersionData>(json);
            // if (datas == null)
            //     throw new Exception("VersionData JSON parse returned null");

            // return datas.ToList();
        }
        catch (Exception)
        {
            // 如果 JSON 壞掉或其他 I/O 問題，回傳預設資料並覆寫檔案
            TestEnd("versionSavePath not exist");
            return new List<VersionData>();
        }
    }

    public static void SaveConfigSaveData()
    {
        SaveConfigSaveData(configSavePath, configSaveDatas);
    }

    public static void SaveConfigSaveData(string path, ConfigSaveData newConfigSaveDatas)
    {
        File.WriteAllText(path, JsonUtility.ToJson(newConfigSaveDatas));
    }

    public static List<ReplaySaveData> LoadReplaySaveDatas()
    {
        try
        {
            if (!File.Exists(replaySavePath))
            {
                // 建立空的預設檔案
                var emptyList = new List<ReplaySaveData>();
                SaveReplayData(replaySavePath, emptyList);
                return emptyList;
            }

            string json = File.ReadAllText(replaySavePath);
            var datas = JsonHelper.FromJson<ReplaySaveData>(json);
            if (datas == null)
                throw new Exception("KeyBoardSaveData JSON parse returned null");

            return datas.ToList();
        }
        catch (Exception)
        {
            // 如果 JSON 壞掉就回傳空清單並重建檔案
            Debug.LogError($"LoadReplaySaveDatas Error");
            var emptyList = new List<ReplaySaveData>();
            SaveReplayData(replaySavePath, emptyList);
            return emptyList;
        }
    }


    public static void ChangeOneReplayData()
    {
        int index = replaySaveDatas.FindIndex(r => r.No == GameReplay.InputSaveData.No);
        if (index != -1)
        {
            replaySaveDatas[index] = GameReplay.InputSaveData.Copy();
        }
        else
        {
            replaySaveDatas.Add(GameReplay.InputSaveData.Copy());
        }
    }


    public static void SaveReplayData()
    {
        SaveReplayData(replaySavePath, replaySaveDatas);
    }
    public static void SaveReplayData(string path, List<ReplaySaveData> newReplaySaveDatas)
    {
        File.WriteAllText(path, JsonHelper.ToJson(newReplaySaveDatas.ToArray()));
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
