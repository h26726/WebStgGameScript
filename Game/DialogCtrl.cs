using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static GameConfig;

using UnityEngine.UI;
using System;

public class DialogCtrl : MonoBehaviour
{
    [SerializeField] Animator leftPaintAnimator;
    [SerializeField] Animator rightPaintAnimator;


    List<DialogSetting> selectDialogSettings = new List<DialogSetting>();
    uint nowDialogSettingsKey;
    public static DialogCtrl nowInstance = null;
    public UpdateFlag dialogCtrlUpdateFlag = UpdateFlag.None;
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

    uint waitEventTargetTime;

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
        if (nowInstance != null)
        {
            Debug.LogError("DialogCtrlStart called while another DialogCtrl instance is active.");
        }
        nowInstance = this;
        selectDialogSettings = GetDialogSettings(mainId);
        if (IS_OPEN_DIALOG)
        {
            gameObject.SetActive(true);
            GameMainCtrl.Instance.nowGameProgressState = GameProgressState.Dialog;
            waitEventTargetTime = GameReplay.keyPressTime + DIALOG_DELAY_KEY_TIME;
            nowDialogSettingsKey = 0;
            GameObjCtrl.Instance.ShowDialogBox();
            dialogCtrlUpdateFlag = UpdateFlag.WaitDialogBoxShow;
        }
    }



    void HideDialogBox()
    {
        GameObjCtrl.Instance.HideDialogBoxStep1();
        if (!leftPaintAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hide")) leftPaintAnimator.Play("Hide");
        if (!rightPaintAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hide")) rightPaintAnimator.Play("Hide");
        waitEventTargetTime = GameReplay.keyPressTime + DIALOG_DELAY_KEY_TIME;
        dialogCtrlUpdateFlag = UpdateFlag.WaitDialogBoxHide;
    }

    public void DialogCtrlStop()
    {
        nowInstance = null;
        dialogCtrlUpdateFlag = UpdateFlag.None;
        GameObjCtrl.Instance.HideDialogBoxStep2();
        gameObject.SetActive(false);
        GameMainCtrl.Instance.nowGameProgressState = GameProgressState.Stage;
    }



    void WaitDialogBoxShowUpdate()
    {
        if (waitEventTargetTime != GameReplay.keyPressTime)
            return;
            
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
        if (!GameReplay.playKeys.Any(r => r.keyPressTime == GameReplay.keyPressTime))
            return;

        if (GameReplay.playKeys.Where(r => r.keyPressTime == GameReplay.keyPressTime).Count() > 1)
        {
            Debug.LogError($"keyPressTime: {GameReplay.keyPressTime} Repeat");
            return;
        }

        var playKeyCodes = GameReplay.playKeys.FirstOrDefault(r => r.keyPressTime == GameReplay.keyPressTime).pressKeyCodes;
        if (playKeyCodes.Contains(KeyCode.Z))
        {
            EnterNextDialog();
        }
    }

    void WaitDialogClickUpdate()
    {
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            AddNowClickIntoReplaySaveData();
            EnterNextDialog();
        }
    }

    void AddNowClickIntoReplaySaveData()
    {
        if (!GameReplay.InputSaveData.replayKeys.Any(r => r.keyPressTime == GameReplay.keyPressTime))
        {
            var ReplayKey = new ReplayKey();
            ReplayKey.keyPressTime = GameReplay.keyPressTime;
            ReplayKey.pressKeyCodes.Add(KeyCode.Z);
            GameReplay.InputSaveData.replayKeys.Add(ReplayKey);
        }
        else
        {
            //同時有上下左右移動之類的按鍵紀錄 則多塞Z的按鍵至原本的
            var oldReplayKey = GameReplay.InputSaveData.replayKeys.FirstOrDefault(r => r.keyPressTime == GameReplay.keyPressTime);
            oldReplayKey.pressKeyCodes.Add(KeyCode.Z);
        }
    }

    void EnterNextDialog()
    {
        nowDialogSettingsKey++;
        if (nowDialogSettingsKey < selectDialogSettings.Count)
        {
            leftPaintAnimator.Play(nowDialogSetting.leftAni);
            rightPaintAnimator.Play(nowDialogSetting.rightAni);
            if (nowDialogSetting.bgm != null)
            {
                LoadCtrl.Instance.pool.PlayBgm(nowDialogSetting.bgm);
            }
            GameObjCtrl.Instance.DialogBoxChangeText(nowDialogSetting.text);
        }
        else
        {
            HideDialogBox();
        }

    }
}
