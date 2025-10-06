using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System;
using System.Linq;
using System.Collections.Generic;

public interface ISelectBaseUpdater
{
    void UpdateHandler();
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
    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        LoadCtrl.Instance.selectList.Add(Instance);
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
        AutoSelectNextAbleUseBtn(ref nowBtnKey, 0, btns);

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
            AutoSelectNextAbleUseBtn(ref nowBtnKey, upDown, btns);
        }
    }

    protected abstract void ClickHandle();

    void AutoSelectNextAbleUseBtn(ref int _BtnKey, int upDown, OptionBase[] _Btns)
    {
        if (upDown != 0)
            _Btns[_BtnKey].animator.Play("Idle");
        _BtnKey += upDown;
        uint Count = 0;
        while (true)
        {
            if (_Btns.Length <= _BtnKey)
            {
                _BtnKey = 0;
            }
            else if (_BtnKey < 0)
            {
                _BtnKey = _Btns.Length - 1;
            }


            if (!_Btns[_BtnKey].isHide && !_Btns[_BtnKey].isDisable)
            {
                break;
            }
            _BtnKey = _BtnKey + upDown;

            Count++;
            if (Count > 100)
            {
                Debug.LogError("Count > 100");
                break;
            }
        }
        _Btns[_BtnKey].animator.Play("Active");
    }





}