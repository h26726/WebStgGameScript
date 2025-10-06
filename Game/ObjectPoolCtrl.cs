using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolCtrl : SingletonBase<ObjectPoolCtrl>
{
    public Dictionary<string, Stack<UnitCtrlBase>> objectDict;
    public Dictionary<string, Animator> titleDict;
    public Dictionary<string, Animator[]> spellDict;
    public Dictionary<string, DialogCtrl> dialogDict;
    public Coroutine loopBGMCoroutine;


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

    public List<ObjectPoolClass> objectPoolList;


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

    public List<SpellPoolClass> spellPoolList;
    public List<TitlePoolClass> titlePoolList;
    public List<DialogPoolClass> dialogPoolList;


    [Serializable]
    public class SpritePoolClass
    {

        [SerializeField]
        public string name = "";

        [SerializeField]
        public Sprite sprite;

    }

    public List<SpritePoolClass> spritePoolList;

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
    public List<MusicPoolClass> musicPoolList;


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
                var unitCtrlBase = UnitCtrlFactory.InitSelfAndCollision(unitCtrlObj);
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

        titleDict = new Dictionary<string, Animator>();

        foreach (var item in titlePoolList)
        {
            titleDict.Add(item.name, item.ani);
        }

        spellDict = new Dictionary<string, Animator[]>();

        foreach (var item in spellPoolList)
        {
            spellDict.Add(item.name, new Animator[] { item.ani, item.bgAni });
        }

        dialogDict = new Dictionary<string, DialogCtrl>();

        foreach (var item in dialogPoolList)
        {
            dialogDict.Add(item.name, item.dialogCtrl);
        }

        yield break;
    }

    public void PlayBgm(string bgm, Action callback = null)
    {
        LoadCtrl.Instance.audioSource.Stop();
        var stageSetting = musicPoolList.FirstOrDefault(r => r.name == bgm);
        LoadCtrl.Instance.audioSource.clip = stageSetting.obj;
        StartCoroutine(DelayPlay(callback));
        if (loopBGMCoroutine != null)
        {
            StopCoroutine(loopBGMCoroutine);
        }
        loopBGMCoroutine = StartCoroutine(LoopSection(stageSetting.loopStart, stageSetting.loopEnd));
    }

    public void PlayTitle(string ani)
    {
        AnimationHelper.PlayAniCoroutine(titleDict[ani]);
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
        unitCtrlBase.externalPoolName = ObjPoolName;
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
