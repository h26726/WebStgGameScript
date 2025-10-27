using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static SaveJsonData;
using static GameConfig;
using System.Collections.Generic;

public class KeyBoardSelect : SelectBase<KeyBoardSelect, KeyOption>
{
    List<KeyBoardSaveData> tmpKeyBoardSaveData;
    bool isWaitInput = false;
    protected override void ClickHandle()
    {
        foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keycode))
            {
                if (isWaitInput)
                {
                    nowBtn.text.color = Color.white;
                    nowBtn.text.text = keycode.ToString();
                    var keyBoardData = tmpKeyBoardSaveData.FirstOrDefault(r => r.baseKey == nowBtn.keyCode);
                    keyBoardData.setKey = keycode;
                    isWaitInput = false;
                    break;
                }
                if (TransferToTmpSetKey(KeyCode.UpArrow, tmpKeyBoardSaveData) != keycode && TransferToTmpSetKey(KeyCode.DownArrow, tmpKeyBoardSaveData) != keycode
                && TransferToTmpSetKey(KeyCode.LeftArrow, tmpKeyBoardSaveData) != keycode && TransferToTmpSetKey(KeyCode.RightArrow, tmpKeyBoardSaveData) != keycode
                && TransferToTmpSetKey(KeyCode.X, tmpKeyBoardSaveData) != keycode
                 && TransferToTmpSetKey(KeyCode.Escape, tmpKeyBoardSaveData) != keycode)
                {
                    if (nowBtn.name == TextName.儲存並返回)
                    {
                        SaveJsonData.keyBoardSaveDatas = tmpKeyBoardSaveData;
                        SaveKeyBoardData();
                        Hide();
                        TitleSelect.Instance.Show();
                    }
                    else if (nowBtn.name == TextName.取消)
                    {
                        Hide();
                        TitleSelect.Instance.Show();
                        UseKeyBoardSaveDatas();
                    }
                    else
                    {
                        isWaitInput = true;
                        nowBtn.text.color = Color.yellow;
                    }
                    break;
                }
                else if (TransferToTmpSetKey(KeyCode.X, tmpKeyBoardSaveData) == keycode)
                {
                    Hide();
                    TitleSelect.Instance.Show();
                    UseKeyBoardSaveDatas();
                    break;
                }
            }
        }
    }

    public void UseKeyBoardSaveDatas()
    {
        // 深拷貝 SaveJsonData.keyBoardSaveDatas 到 tmpKeyBoardSaveData
        tmpKeyBoardSaveData = SaveJsonData.keyBoardSaveDatas
            .Select(data => new KeyBoardSaveData()
            {
                baseKey = data.baseKey,
                setKey = data.setKey
            })
            .ToList();

        // 更新按鈕文字
        foreach (var btn in btns)
        {
            if (btn.keyCode == KeyCode.None)
                continue;

            var match = tmpKeyBoardSaveData.FirstOrDefault(r => r.baseKey == btn.keyCode);
            if (match != null)
                btn.text.text = match.setKey.ToString();
        }
    }
}
