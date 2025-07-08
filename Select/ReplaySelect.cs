using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static CommonData;
using static CommonFunc;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using static GameConfig;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ReplaySelect : SelectBase<ReplaySelect, ReplayOption>
{
    protected override bool isSwitchCamara
    {
        get
        {
            if (IsRead)
                return false;
            return true;
        }
    }
    [SerializeField] Image title;
    [SerializeField] Image hint;
    [SerializeField] Sprite[] titleSprites;
    [SerializeField] Sprite[] hintSprites;
    uint nowPage = 1;
    bool isRead = false;
    public bool IsRead
    {
        get
        {
            return isRead;
        }

        set
        {
            isRead = value;
            if (value)
            {
                title.sprite = titleSprites[0];
            }
            else
            {
                title.sprite = titleSprites[1];
            }

        }
    }
    bool isInputText = false;


    protected override void AwakeHandle()
    {
        foreach (var btn in btns)
        {
            btn.line = new Text[4];
            for (int i = 0; i < 4; i++)
            {
                if (i == 1)
                {
                    btn.line[i] = btn.animator.gameObject.transform.GetChild(i).GetChild(0).GetComponent<Text>();
                    btn.inputField = btn.animator.gameObject.transform.GetChild(i).GetChild(1).GetComponent<InputField>();
                    btn.inputField.characterLimit = 15;
                    btn.inputField.interactable = false;
                    btn.inputField.onValidateInput += ValidateEnglishOnly;
                }
                else
                {
                    btn.line[i] = btn.animator.gameObject.transform.GetChild(i).GetComponent<Text>();
                }
            }
        }
    }


    void InputTextHandle()
    {
        if (EventSystem.current.currentSelectedGameObject != nowBtn.inputField.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(nowBtn.inputField.gameObject);
            nowBtn.inputField.Select();
            nowBtn.inputField.ActivateInputField();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            isInputText = false;
            hint.sprite = hintSprites[0];
            nowBtn.inputField.DeactivateInputField();                 // 停止輸入、關閉游標
            nowBtn.inputField.interactable = false;                   // 禁止再次互動
            EventSystem.current.SetSelectedGameObject(null);  // ❗清除目前選取的 UI
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                GameSystem.Instance.replaySaveData.No = nowBtn.no;
                GameSystem.Instance.replaySaveData.time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                GameSystem.Instance.replaySaveData.name = nowBtn.inputField.text;
                PlayerSaveData.SaveReplayData();
            }
            LoadPage();
        }

    }
    protected override bool CheckBlockAndSpecialHandle()
    {
        if (isInputText)
        {
            InputTextHandle();
            return true;
        }
        return false;

    }

    protected override void ClickExtraHandle()
    {
        if (Input.GetKeyDown(GetSetKey(KeyCode.LeftArrow)) && nowPage > 1)
        {
            nowPage -= 1;
            LoadPage();
        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.RightArrow)) && nowPage < 10)
        {
            nowPage += 1;
            LoadPage();
        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
        {
            if (IsRead)
            {
                Hide();
                var ReplayData = PlayerSaveData.replaySaveDatas.FirstOrDefault(r => r.No == nowBtn.no);
                GameSystem.Instance.isReplay = true;
                GameSystem.Instance.selectDifficult = ReplayData.selectDifficult;
                GameSystem.Instance.selectPracticeId = ReplayData.selectPracticeId;
                GameSystem.Instance.playReplayKeys = ReplayData.replayKeys;
                GameSystem.Instance.playReplayMaxKeyTime = (uint)ReplayData.replayKeys.Max(r => r.keyTime);
                LoadingCtrl.Instance.SwitchPage(LoadingCtrl.PageIndex.Game);
            }
            else
            {
                isInputText = true;
                hint.sprite = hintSprites[1];
                nowBtn.inputField.text = "";
                nowBtn.inputField.interactable = true;
                nowBtn.inputField.Select();
                nowBtn.inputField.ActivateInputField();
            }
        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.X)))
        {
            Hide();
            Back();
        }

    }

    private char ValidateEnglishOnly(string text, int charIndex, char addedChar)
    {
        if ((addedChar >= 'a' && addedChar <= 'z') ||
            (addedChar >= 'A' && addedChar <= 'Z') ||
            (addedChar == '-') ||
            (addedChar >= '0' && addedChar <= '9'))
        {
            return addedChar; // 合法字元
        }
        else
        {
            return '\0'; // 不合法，忽略這個輸入
        }
    }

    public void LoadPage()
    {
        var replayDatas = PlayerSaveData.replaySaveDatas.Where(r => r.No > (nowPage - 1) * 10 && r.No <= nowPage * 10).ToList();

        for (int i = 0; i < 10; i++)
        {
            var No = (uint)(i + 1 + (nowPage - 1) * 10);
            btns[i].no = No;
            btns[i].line[0].text = "NO." + No.ToString().PadLeft(2, '0');
            if (!replayDatas.Any(r => r.No == No))
            {
                btns[i].replayData = null;
                // _Btns[i].Line[1].text = "-------------------------";
                btns[i].line[2].text = "----/--/-- --:--:--";
                btns[i].line[3].text = "----";
                UpdateInputField(btns[i].inputField, "---------------");
            }
            else
            {

                var replayData = replayDatas.FirstOrDefault(r => r.No == No);
                btns[i].replayData = replayData;
                btns[i].line[2].text = replayData.time;
                btns[i].line[3].text = DifficultToString(replayData.selectDifficult);
                UpdateInputField(btns[i].inputField, replayData.name);

            }
        }
    }

    void UpdateInputField(InputField inputField, string str)
    {
        inputField.interactable = true;  // 確保互動開啟才更新文字比較保險
        inputField.text = str;
        inputField.ForceLabelUpdate();
        inputField.interactable = false; // 如果你想關閉互動，再關閉
    }
}
