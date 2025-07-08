using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using static GameConfig;

using UnityEngine.UI;
using System;

public class DialogCtrl : MonoBehaviour
{
    [SerializeField] Animator leftPaintAnimator;
    [SerializeField] Animator rightPaintAnimator;

    public static DialogCtrl nowInstance;

    List<DialogSetting> dialogSettings = new List<DialogSetting>();
    uint nowDialogSettingsKey;

    DialogSetting nowDialogSetting
    {
        get
        {
            if (nowDialogSettingsKey >= dialogSettings.Count) return null;
            return dialogSettings[(int)nowDialogSettingsKey];
        }
    }

    uint nextKeyTime;

    public event Action eventUpdateHandle;
    public void RunEventUpdate()
    {
        if (eventUpdateHandle != null)
            eventUpdateHandle.Invoke();
    }
    void ChangeEventUpdate(Action action)
    {
        eventUpdateHandle = null;
        eventUpdateHandle += action;
    }

    public void DialogStart(uint MainId)
    {
        if (nowInstance != null)
        {
            Debug.LogError("nowInstance != null");
            return;
        }
        dialogSettings = LoadingCtrl.Instance.dialogSettings.Where(r => r.mainId == MainId).ToList();
        if (IS_OPEN_DIALOG)
        {
            gameObject.SetActive(true);
            GameSystem.Instance.nowGameProgressState = GameProgressState.Dialog;
            nowInstance = this;
            nowDialogSettingsKey = 0;

            OpenDialogBox();
        }
    }



    void OpenDialogBox()
    {
        GameSystem.Instance.dialogBox.SetActive(true);
        GameSystem.Instance.dialogBoxAnimator.Play("Show");
        nextKeyTime = GameSystem.Instance.keyTime + DIALOG_DELAY_KEY_TIME;
        ChangeEventUpdate(WaitDialogBoxShow);
    }

    void HideDialogBox()
    {
        GameSystem.Instance.dialogBoxText.text = "";
        GameSystem.Instance.dialogBoxAnimator.Play("Hide");
        if (!leftPaintAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hide")) leftPaintAnimator.Play("Hide");
        if (!rightPaintAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hide")) rightPaintAnimator.Play("Hide");
        nextKeyTime = GameSystem.Instance.keyTime + DIALOG_DELAY_KEY_TIME;
        ChangeEventUpdate(WaitDialogBoxHide);
    }

    public void DialogEnd()
    {
        eventUpdateHandle = null;
        GameSystem.Instance.dialogBox.SetActive(false);
        gameObject.SetActive(false);
        nowInstance = null;
        GameSystem.Instance.nowGameProgressState = GameProgressState.Stage;

    }



    void WaitDialogBoxShow()
    {
        if (nextKeyTime == GameSystem.Instance.keyTime)
        {
            ChangeEventUpdate(WaitDialogClick);
        }
    }
    void WaitDialogBoxHide()
    {
        if (nextKeyTime == GameSystem.Instance.keyTime)
        {
            DialogEnd();
        }
    }

    void WaitDialogClick()
    {
        if (nowDialogSettingsKey < dialogSettings.Count)
        {
            leftPaintAnimator.Play(nowDialogSetting.leftAni);
            rightPaintAnimator.Play(nowDialogSetting.rightAni);
            if (nowDialogSetting.bgm != null)
            {
                LoadingCtrl.Instance.pool.PlayBgm(nowDialogSetting.bgm);
            }
            GameSystem.Instance.dialogBoxText.text = nowDialogSetting.text;
            if (!GameSystem.Instance.isReplay)
            {
                if (Input.GetKeyDown(GetSetKey(KeyCode.Z)) || Input.GetKeyDown(KeyCode.Joystick1Button1))
                {
                    var ReplayKey = new ReplayKeyClass();
                    if (!GameSystem.Instance.replaySaveData.replayKeys.Any(r => r.keyTime == GameSystem.Instance.keyTime))
                    {
                        ReplayKey.keyTime = GameSystem.Instance.keyTime;
                        GameSystem.Instance.replaySaveData.replayKeys.Add(ReplayKey);
                    }
                    else
                    {
                        ReplayKey = GameSystem.Instance.replaySaveData.replayKeys.FirstOrDefault(r => r.keyTime == GameSystem.Instance.keyTime);
                    }
                    ReplayKey.keyCodes.Add(KeyCode.Z);
                    nowDialogSettingsKey++;
                }
            }
            else
            {
                if (GameSystem.Instance.playReplayKeys.Any(r => r.keyTime == GameSystem.Instance.keyTime))
                {
                    if (GameSystem.Instance.playReplayKeys.Where(r => r.keyTime == GameSystem.Instance.keyTime).Count() > 1)
                    {
                        Debug.LogError($"keyTime: {GameSystem.Instance.keyTime} Repeat");
                    }
                    var playKeyCodes = GameSystem.Instance.playReplayKeys.FirstOrDefault(r => r.keyTime == GameSystem.Instance.keyTime).keyCodes;
                    if (playKeyCodes.Contains(KeyCode.Z))
                    {
                        nowDialogSettingsKey++;
                    }
                }
            }
        }
        else
        {
            HideDialogBox();
            ChangeEventUpdate(WaitDialogBoxHide);
        }

    }
}
