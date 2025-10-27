using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static SaveJsonData;
using System;
using System.Linq;
using System.Collections.Generic;

[SerializeField]
public interface ISelectBaseUpdater
{
    void UpdateHandler();
    void Init();
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
    protected int time = 0;
    public TBtn nowBtn
    {
        get
        {
            return btns[nowBtnKey];
        }
    }


    public virtual void Init()
    {
        selfAnimator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
        var hides = btns.Where(r => r.isHide).ToList();
        foreach (var hide in hides)
        {
            hide.animator.gameObject.SetActive(false);
        }
        btns[nowBtnKey].animator.Play("Active");
    }


    public void Show()
    {
        gameObject.SetActive(true);
        if (isSwitchCamara)
        {
            LoadCtrl.Instance.SwitchTitleCamera();
        }
        selfAnimator.Play("Show");
        BtnChange(ref nowBtnKey, 0, btns);
    }

    public void Hide()
    {
        if (isSwitchCamara)
        {
            LoadCtrl.Instance.SwitchGameCamera();
        }
        selfAnimator.Play("Hide");
        time = 0;
    }


    public virtual void UpdateHandler()
    {
        if (gameObject.activeSelf == false)
            return;
        if (canvasGroup.alpha == 0)
        {
            time++;
            if (time >= GameConfig.SELECT_AUTO_CLOSE_TIME)
            {
                gameObject.SetActive(false);
                time = 0;
            }
            return;
        }
        if (canvasGroup.alpha < 0.8)
            return;
        PressUpDownHandle();
        ClickHandle();
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

    public void SetNextAction(Action action)
    {
        nextAction = null;
        nextAction += action;
    }
    public void SetBackAction(Action action)
    {
        backAction = null;
        backAction += action;
    }


    protected void PressUpDownHandle()
    {
        var upDown = GetKeyOneVal_UpDown(null);
        if (upDown != 0)
        {
            BtnChange(ref nowBtnKey, upDown, btns);
        }
    }

    protected abstract void ClickHandle();

    protected int AutoNextAbleUseBtnKey(int btnKey, bool direct, OptionBase[] _Btns)
    {
        uint Count = 0;
        while (true)
        {
            if (_Btns.Length <= btnKey)
            {
                btnKey = 0;
            }
            else if (btnKey < 0)
            {
                btnKey = _Btns.Length - 1;
            }


            if (!_Btns[btnKey].isHide && !_Btns[btnKey].isDisable)
            {
                break;
            }
            if (direct)
                btnKey++;
            else
                btnKey--;

            Count++;
            if (Count > 100)
            {
                Debug.LogError("Count > 100");
                break;
            }
        }
        return btnKey;
    }

    public void BtnChange(ref int btnKey, int change, OptionBase[] btns)
    {
        var oldBtnKey = btnKey;
        btnKey += change;
        btnKey = AutoNextAbleUseBtnKey(btnKey, change >= 0, btns);
        ActveBtn(oldBtnKey, btnKey, btns);
    }

    public void BtnChange(ref int btnKey, int newBtnKey, bool direct, OptionBase[] btns)
    {
        var oldBtnKey = btnKey;
        btnKey = newBtnKey;
        btnKey = AutoNextAbleUseBtnKey(btnKey, direct, btns);
        ActveBtn(oldBtnKey, btnKey, btns);
    }

    public void ActveBtn(int oldBtnKey, int newBtnKey, OptionBase[] btns)
    {
        if (oldBtnKey != newBtnKey)
        {
            btns[oldBtnKey].animator.Play("Idle");
        }
        ActveBtn(newBtnKey, btns);
    }

    public void ActveBtn(int newBtnKey, OptionBase[] btns)
    {
        btns[newBtnKey].animator.Play("Active");
    }





}