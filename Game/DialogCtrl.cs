using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using static GameConfig;

using UnityEngine.UI;
using System;

public class DialogCtrl : MonoBehaviour
{
    [SerializeField] Animator leftPaintAnimator;
    [SerializeField] Animator rightPaintAnimator;


    uint waitEventTargetTime;
    uint nowDialogSettingsKey;
    public static DialogCtrl nowInstance;
    public UpdateFlag dialogCtrlUpdateFlag { get; set; }
    List<DialogSetting> selectDialogSettings;
    public enum UpdateFlag
    {
        None,
        WaitDialogBoxShow,
        WaitDialogBoxHide,
        WaitDialogClick,
        WaitDialogReplay,
    }



    DialogSetting nowDialogSetting
    {
        get
        {
            if (nowDialogSettingsKey >= selectDialogSettings.Count) return null;
            return selectDialogSettings[(int)nowDialogSettingsKey];
        }
    }


    void Awake()
    {
        selectDialogSettings = new List<DialogSetting>();
        Reset();
    }

    public void Reset()
    {
        waitEventTargetTime = 0;
        nowDialogSettingsKey = 0;
        dialogCtrlUpdateFlag = UpdateFlag.None;
        selectDialogSettings.Clear();
    }

    public void Close()
    {
        leftPaintAnimator.Rebind();
        rightPaintAnimator.Rebind();
        leftPaintAnimator.Update(0f);
        rightPaintAnimator.Update(0f);
        gameObject.SetActive(false);
        nowInstance = null;
    }

    public void UpdateHandler()
    {
        switch (dialogCtrlUpdateFlag)
        {
            case UpdateFlag.WaitDialogBoxShow:
                {
                    WaitDialogBoxShowUpdate();
                    break;
                }
            case UpdateFlag.WaitDialogBoxHide:
                {
                    WaitDialogBoxHideUpdate();
                    break;
                }
            case UpdateFlag.WaitDialogReplay:
                {
                    WaitDialogReplayUpdate();
                    break;
                }
            case UpdateFlag.WaitDialogClick:
                {
                    WaitDialogClickUpdate();
                    break;
                }
            case UpdateFlag.None:
            default:
                {
                    // 無需處理
                    break;
                }
        }
    }

    public List<DialogSetting> GetDialogSettings(uint mainId)
    {
        return LoadCtrl.Instance.selectVersionData.dialogSettings.Where(r => r.mainId == mainId).ToList();
    }

    public void DialogCtrlStart(uint mainId)
    {

        if (IS_OPEN_DIALOG)
        {
            if (nowInstance != null)
            {
                Debug.LogError("DialogCtrlStart called while another DialogCtrl instance is active.");
            }
            gameObject.SetActive(true);
            nowInstance = this;
            selectDialogSettings = GetDialogSettings(mainId);
            GameMainCtrl.Instance.nowGameProgressState = GameProgressState.Dialog;
            waitEventTargetTime = GameReplay.keyPressTime + DIALOG_DELAY_KEY_TIME;
            nowDialogSettingsKey = 0;
            GameObjCtrl.Instance.ShowDialogBox();
            ShowDialog();
            dialogCtrlUpdateFlag = UpdateFlag.WaitDialogBoxShow;
        }
    }



    public void HideDialogBox()
    {
        GameObjCtrl.Instance.DialogChangeText("");
        GameObjCtrl.Instance.DialogCloseText();
        GameObjCtrl.Instance.HideDialogBoxStep1();
        if (!leftPaintAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hide")) leftPaintAnimator.Play("Hide");
        if (!rightPaintAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hide")) rightPaintAnimator.Play("Hide");
        waitEventTargetTime = GameReplay.keyPressTime + DIALOG_DELAY_KEY_TIME;
        Debug.Log("waitEventTargetTime:" + waitEventTargetTime);
        dialogCtrlUpdateFlag = UpdateFlag.WaitDialogBoxHide;
    }

    public void DialogCtrlStop()
    {
        Reset();
        Close();
        GameObjCtrl.Instance.HideDialogBoxStep2();
        GameMainCtrl.Instance.nowGameProgressState = GameProgressState.Stage;
    }


    void WaitDialogBoxShowUpdate()
    {
        if (waitEventTargetTime != GameReplay.keyPressTime)
            return;

        GameObjCtrl.Instance.DialogOpenText();
        if (GameReplay.isReplayMode)
        {
            dialogCtrlUpdateFlag = UpdateFlag.WaitDialogReplay;
        }
        else
        {
            dialogCtrlUpdateFlag = UpdateFlag.WaitDialogClick;
        }
    }

    void WaitDialogBoxHideUpdate()
    {
        if (waitEventTargetTime == GameReplay.keyPressTime)
        {
            DialogCtrlStop();
        }
    }

    void WaitDialogReplayUpdate()
    {
        if (!GameReplay.CheckPlayKeyExist())
            return;

        var playKeyCodes = GameReplay.GetNowPlayKeyCodes();
        if (playKeyCodes.Contains(KeyCode.D))
        {
            EnterNextDialog();
        }
    }

    void WaitDialogClickUpdate()
    {
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            GameReplay.InputSaveData.AddReplayKey(GameReplay.keyPressTime, KeyCode.D);
            EnterNextDialog();
        }
    }

    void EnterNextDialog()
    {
        nowDialogSettingsKey++;
        ShowDialog();
    }

    void ShowDialog()
    {
        if (nowDialogSettingsKey < selectDialogSettings.Count)
        {
            leftPaintAnimator.Play(nowDialogSetting.leftAni);
            rightPaintAnimator.Play(nowDialogSetting.rightAni);
            if (!InvalidHelper.IsInvalid(nowDialogSetting.bgm))
            {
                LoadCtrl.Instance.pool.PlayBgm(nowDialogSetting.bgm);
            }
            GameObjCtrl.Instance.DialogChangeText(nowDialogSetting.text);
        }
        else
        {
            HideDialogBox();
        }
    }
}
