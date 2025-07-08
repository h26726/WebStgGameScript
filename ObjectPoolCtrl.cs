using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolCtrl : MonoBehaviour
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
                var UnitCtrlBase = tmpGameObject.GetComponent<UnitCtrlBase>();
                UnitCtrlBase.paramPoolName = item.name;
                tmpGameObject.SetActive(false);
                tmpList.Push(UnitCtrlBase);
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

    void Start()
    {
    }

    public void PlayBgm(string bgm,Action callback = null)
    {
        LoadingCtrl.Instance.audioSource.Stop();
        var stageSetting = musicPoolList.FirstOrDefault(r => r.name == bgm);
        LoadingCtrl.Instance.audioSource.clip = stageSetting.obj;
        StartCoroutine(DelayPlay(callback));
        if (loopBGMCoroutine != null)
        {
            StopCoroutine(loopBGMCoroutine);
        }
        loopBGMCoroutine = StartCoroutine(LoopSection(stageSetting.loopStart, stageSetting.loopEnd));
    }

    public IEnumerator DelayPlay(Action callback)
    {
        yield return new WaitForSeconds(1f);
        LoadingCtrl.Instance.audioSource.time = 0;
        LoadingCtrl.Instance.audioSource.Play();
        callback?.Invoke();
    }

    public IEnumerator LoopSection(float loopStartTime, float loopEndTime)
    {
        while (true)
        {
            if (LoadingCtrl.Instance.audioSource.isPlaying && LoadingCtrl.Instance.audioSource.time >= loopEndTime)
            {
                LoadingCtrl.Instance.audioSource.time = loopStartTime;
            }
            yield return null;
        }
    }


}
