using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolCtrl : SingletonBase<ObjectPoolCtrl>
{
    [Serializable]
    public class ObjectPoolClass
    {

        [SerializeField]
        public string name = "";

        [SerializeField]
        public GameObject obj;
        [SerializeField]

        public uint count = 1;
    }
    [Serializable]
    public class TitlePoolClass
    {

        [SerializeField]
        public string name = "";

        [SerializeField]
        public Animator ani;

    }

    [Serializable]
    public class SpellPoolClass
    {

        [SerializeField]
        public string name = "";

        [SerializeField]
        public Animator ani;
        [SerializeField]
        public Animator bgAni;

    }

    [Serializable]
    public class DialogPoolClass
    {

        [SerializeField]
        public string name = "";

        [SerializeField]
        public DialogCtrl dialogCtrl;

    }

    [Serializable]
    public class SpritePoolClass
    {

        [SerializeField]
        public string name = "";

        [SerializeField]
        public Sprite sprite;

    }
    [Serializable]
    public class MusicPoolClass
    {

        [SerializeField]
        public string name = "";

        [SerializeField]
        public AudioClip obj;
        public float loopStart;
        public float loopEnd;
    }

    public Dictionary<string, Animator> titleDict;
    public Dictionary<string, Animator[]> spellDict;
    public Dictionary<string, DialogCtrl> dialogDict;
    public Dictionary<string, Sprite> spriteDict;
    public Dictionary<string, (AudioClip, float, float)> musicDict;
    public List<ObjectPoolClass> objectPoolList;
    public List<SpellPoolClass> spellPoolList;
    public List<TitlePoolClass> titlePoolList;
    public List<DialogPoolClass> dialogPoolList;
    public List<SpritePoolClass> spritePoolList;
    public List<MusicPoolClass> musicPoolList;
    public Dictionary<string, Stack<UnitCtrlBase>> objectDict;
    public Coroutine loopBGMCoroutine;

    protected override void Awake(){
        base.Awake();
        // titleDict
        titleDict = titlePoolList.ToDictionary(item => item.name, item => item.ani);

        // spellDict
        spellDict = spellPoolList.ToDictionary(item => item.name, item => new Animator[] { item.ani, item.bgAni });

        // dialogDict
        dialogDict = dialogPoolList.ToDictionary(item => item.name, item => item.dialogCtrl);
        spriteDict = spritePoolList.ToDictionary(item => item.name, item => item.sprite);
        musicDict = musicPoolList.ToDictionary(item => item.name, item => (item.obj, item.loopStart, item.loopEnd));
    }

    public void LogNum()
    {
        foreach (var item in objectDict)
        {
            Debug.Log($"pool:{item.Key} num:{item.Value.Count}");
        }
    }

    public IEnumerator Init()
    {
        Stack<UnitCtrlBase> tmpList;
        GameObject tmpGameObject;
        objectDict = new Dictionary<string, Stack<UnitCtrlBase>>();
        foreach (var item in objectPoolList)
        {
            tmpList = new Stack<UnitCtrlBase>();
            for (int i = 0; i < item.count; i++)
            {
                tmpGameObject = Instantiate(item.obj, transform);
                var unitCtrlObj = tmpGameObject.GetComponent<UnitCtrlObj>();
                unitCtrlObj.CloseUnit();
                var unitCtrlBase = UnitCtrlFactory.Init(unitCtrlObj);
                unitCtrlBase.externalPoolName = item.name;
                if (item.name == "Player1")
                {
                    Debug.Log("Player1 init");
                }
                if (unitCtrlObj is PlayerCtrlObj)
                {
                    Debug.Log("Player1 PlayerCtrlObj");
                }
                tmpList.Push(unitCtrlBase);
#if !UNITY_EDITOR
                if (i > 0 && i % 100 == 0)
                    yield return null;
#endif
            }
            objectDict.Add(item.name, tmpList);
        }
        yield break;
    }

    public void PlayBgm(string bgm, Action callback = null)
    {
        LoadCtrl.Instance.audioSource.Stop();
        var (clip,loopStart,loopEnd) = musicDict[bgm];
        LoadCtrl.Instance.audioSource.clip = clip;
        StartCoroutine(DelayPlay(callback));
        if (loopBGMCoroutine != null)
        {
            StopCoroutine(loopBGMCoroutine);
        }
        loopBGMCoroutine = StartCoroutine(LoopSection(loopStart, loopEnd));
    }

    public void PlayTitle(string ani)
    {
        this.StartCoroutine(AnimationHelper.PlayAniCoroutine(titleDict[ani]));
    }

    public void PlayDialog(string obj, uint Id)
    {

        if (dialogDict.ContainsKey(obj))
        {
            DialogCtrl dialogCtrl = dialogDict[obj];
            dialogCtrl.DialogCtrlStart(Id);
        }
    }

    public IEnumerator DelayPlay(Action callback)
    {
        yield return new WaitForSeconds(1f);
        LoadCtrl.Instance.audioSource.time = 0;
        LoadCtrl.Instance.audioSource.Play();
        callback?.Invoke();
    }

    public IEnumerator LoopSection(float loopStartTime, float loopEndTime)
    {
        while (true)
        {
            if (LoadCtrl.Instance.audioSource.isPlaying && LoadCtrl.Instance.audioSource.time >= loopEndTime)
            {
                LoadCtrl.Instance.audioSource.time = loopStartTime;
            }
            yield return null;
        }
    }

    public UnitCtrlBase GetOne(string ObjPoolName)
    {
        // Debug.Log(nameof(GetOneByPool));
        if (!CheckNameExist(ObjPoolName) || !CheckNumExist(ObjPoolName))
        {
            return null;
        }
        var unitCtrlBase = objectDict[ObjPoolName].Pop();
        return unitCtrlBase;
    }

    public void RestoreOne(UnitCtrlBase unitCtrlBase) // 未於LoadingCtrl呼叫
    {
        // Debug.Log(nameof(RestoreOneIntoPool));
        if (!CheckNameExist(unitCtrlBase.externalPoolName))
        {
            return;
        }
        objectDict[unitCtrlBase.externalPoolName].Push(unitCtrlBase);

    }

    public bool CheckNameExist(string ObjPoolName)
    {
        // Debug.Log(nameof(CheckObjPoolNameExist));
        if (!objectDict.ContainsKey(ObjPoolName))
        {
            Debug.LogError($"takeFormPool ParamPoolName not found:{ObjPoolName}");
            return false;
        }
        return true;
    }

    public bool CheckNumExist(string ObjPoolName)
    {
        // Debug.Log(nameof(CheckObjPoolNumExist));
        var stack = objectDict[ObjPoolName];
        if (stack.Count == 0)
        {
            Debug.LogError($"takeFormPool ParamPoolName stack is empty:{ObjPoolName}");
            return false;
        }
        return true;
    }

}
