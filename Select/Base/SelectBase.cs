using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System;
using System.Linq;
using System.Collections.Generic;

public interface ISelectBaseUpdater
{
    void UpdateHandle();
}
public abstract class SelectBase<T, TBtn> : SingletonBase<T>, ISelectBaseUpdater
    where T : SelectBase<T, TBtn>
    where TBtn : OptionBase
{
    protected virtual bool isSwitchCamara => false;
    [SerializeField] protected TBtn[] btns;
    Animator selfAnimator;
    public CanvasGroup canvasGroup { get; set; }
    public event Action nextAction;
    public event Action backAction;
    protected int nowBtnKey = 0;
    public TBtn nowBtn
    {
        get
        {
            return btns[nowBtnKey];
        }
    }
    void Awake()
    {
        UpdateManager.Instance.selectList.Add(Instance);
        selfAnimator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
        var hides = btns.Where(r => r.isHide).ToList();
        foreach (var hide in hides)
        {
            hide.animator.gameObject.SetActive(false);
        }
        btns[nowBtnKey].animator.Play("Active");
        AwakeHandle();
    }

    protected virtual void AwakeHandle()
    {

    }

   
    public void Show()
    {
        if (isSwitchCamara)
        {
            LoadingCtrl.Instance.titleCamera.enabled = true;
            LoadingCtrl.Instance.gameCamera.enabled = false;
        }

        selfAnimator.Play("Show");
    }

    public void Hide()
    {
        if (isSwitchCamara)
        {
            LoadingCtrl.Instance.titleCamera.enabled = false;
            LoadingCtrl.Instance.gameCamera.enabled = true;
        }
        selfAnimator.Play("Hide");
    }


    public void UpdateHandle()
    {
        if (CheckBlockAndSpecialHandle())
        {
            return;
        }
        if (canvasGroup.alpha < 0.8)
            return;
        ClickHandle();
        ClickExtraHandle();
    }

    public void Next()
    {
        if (nextAction != null)
            nextAction.Invoke();
    }
    public void Back()
    {
        if (backAction != null)
            backAction.Invoke();
    }

    public void AddNext(Action action)
    {
        nextAction = null;
        nextAction += action;
    }
    public void AddBack(Action action)
    {
        backAction = null;
        backAction += action;
    }


    protected void ClickUpDownHandle(List<KeyBoardSaveData> keyBoardSaveData = null)
    {
        var upDown = KeyBoardOnceToDirectVal(keyBoardSaveData);
        if (upDown != 0)
        {
            AutoSkipNotBtnAndOutBtn(ref nowBtnKey, upDown, btns);
        }
    }

    protected abstract void ClickExtraHandle();


    protected virtual void ClickHandle()
    {
        ClickUpDownHandle(null);
    }

    protected virtual bool CheckBlockAndSpecialHandle()
    {
        return false;
    }
}