using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;

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
            GameSystem.Instance.scoreText.text = value.ToString().PadLeft(8, '0');
        }
    }
    public static List<KeyBoardSaveData> keyBoardSaveData = new List<KeyBoardSaveData>();
    public static List<ReplayDataClass> replaySaveDatas = new List<ReplayDataClass>();
    public static ConfigSaveData configSaveDatas = new ConfigSaveData();
    public static string configSavePath = Application.persistentDataPath + "/Config.json";
    public static string keyBoardSavePath = Application.persistentDataPath + "/KeyBoard.json";
    public static string replaySavePath = Application.persistentDataPath + "/Replay.json";

    [System.Serializable]
    public class KeyBoardSaveData
    {
        public KeyCode baseKey;
        public KeyCode setKey;
    }
    public static KeyCode GetSetKey(KeyCode baseKey)
    {
        var data = keyBoardSaveData.FirstOrDefault(r => r.baseKey == baseKey);
        return data != null ? data.setKey : KeyCode.None;
    }

    public static KeyCode GetSetKey(KeyCode baseKey, List<KeyBoardSaveData> keyBoardSaveData)
    {
        var data = keyBoardSaveData.FirstOrDefault(r => r.baseKey == baseKey);
        return data != null ? data.setKey : KeyCode.None;
    }
    [System.Serializable]
    public class CurrentPlayerSaveData
    {
        public string name;
        public uint score;
        public Difficult difficultType;
        public string character;
    }

    [System.Serializable]
    public class ConfigSaveData
    {
        public uint BGMVolume = 60;
        public uint BGSVolume = 60;
        public ScreenMode screenModeType = ScreenMode.FullScreen;
    }
    [System.Serializable]
    public class ReplayKeyClass
    {
        public uint keyTime;
        public List<KeyCode> keyCodes = new List<KeyCode>();

    }

    [System.Serializable]
    public class ReplayDataClass
    {
        public uint No;
        public string name;
        public string time;
        public uint selectPracticeId;
        public Difficult selectDifficult;
        public List<ReplayKeyClass> replayKeys;

        public ReplayDataClass Copy()
        {
            var newData = new ReplayDataClass();
            newData.No = No;
            newData.name = name;
            newData.time = time;
            newData.selectPracticeId = selectPracticeId;
            newData.selectDifficult = selectDifficult;
            newData.replayKeys = replayKeys;
            return newData;
        }
    }



    public enum ScreenMode
    {
        FullScreen,
        Windowed,
    }

    public static List<KeyBoardSaveData> LoadKeyBoardData()
    {
        if (!File.Exists(keyBoardSavePath))
        {
            ResetKeyBoardData();
            string aa = JsonHelper.ToJson(keyBoardSaveData.ToArray());
            File.WriteAllText(keyBoardSavePath, aa);
        }
        string Save = File.ReadAllText(keyBoardSavePath);
        KeyBoardSaveData[] datas = JsonHelper.FromJson<KeyBoardSaveData>(Save);
        return datas.ToList();
    }
    public static void ResetKeyBoardData()
    {
        keyBoardSaveData = ResetKeyBoardData(keyBoardSaveData);
    }
    public static List<KeyBoardSaveData> ResetKeyBoardData(List<KeyBoardSaveData> keyBoardSaveData)
    {
        keyBoardSaveData = new List<KeyBoardSaveData>();
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.UpArrow, setKey = KeyCode.UpArrow });
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.DownArrow, setKey = KeyCode.DownArrow });
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.LeftArrow, setKey = KeyCode.LeftArrow });
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.RightArrow, setKey = KeyCode.RightArrow });
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.Z, setKey = KeyCode.Z });
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.X, setKey = KeyCode.X });
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.LeftShift, setKey = KeyCode.LeftShift });
        keyBoardSaveData.Add(new KeyBoardSaveData { baseKey = KeyCode.Escape, setKey = KeyCode.Escape });
        return keyBoardSaveData;
    }

    public static ConfigSaveData LoadConfigSaveData()
    {
        if (!File.Exists(configSavePath))
        {
            string aa = JsonUtility.ToJson(configSaveDatas);
            File.WriteAllText(configSavePath, aa);
        }
        string ConfigSave = File.ReadAllText(configSavePath);
        return JsonUtility.FromJson<ConfigSaveData>(ConfigSave);
    }

    public static void SaveKeyBoardData()
    {
        File.WriteAllText(keyBoardSavePath, JsonHelper.ToJson(keyBoardSaveData.ToArray()));
    }


    public static void SaveConfigSaveData()
    {
        File.WriteAllText(configSavePath, JsonUtility.ToJson(configSaveDatas));
    }

    public static List<ReplayDataClass> LoadReplayData()
    {
        if (!File.Exists(replaySavePath))
        {
            string aa = JsonHelper.ToJson(new ReplayDataClass[0]);
            File.WriteAllText(replaySavePath, aa);
        }
        string Save = File.ReadAllText(replaySavePath);
        ReplayDataClass[] datas = JsonHelper.FromJson<ReplayDataClass>(Save);
        return datas.ToList();
    }

    public static void SaveReplayData()
    {
        int index = replaySaveDatas.FindIndex(r => r.No == GameSystem.Instance.replaySaveData.No);
        if (index != -1)
        {
            replaySaveDatas[index] = GameSystem.Instance.replaySaveData;
        }
        else
        {
            replaySaveDatas.Add(GameSystem.Instance.replaySaveData.Copy());
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
