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

public class KeyBoardSelect : SelectBase<KeyBoardSelect, KeyOption>
{
    List<KeyBoardSaveData> keyBoardSaveData;
    bool isWaitInput = false;
    protected override void ClickExtraHandle()
    {
        foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keycode))
            {
                if (isWaitInput)
                {
                    nowBtn.text.color = Color.white;
                    nowBtn.text.text = keycode.ToString();
                    var keyBoardData = keyBoardSaveData.FirstOrDefault(r => r.baseKey == nowBtn.keyCode);
                    keyBoardData.setKey = keycode;
                    isWaitInput = false;
                    break;
                }
                if (GetSetKey(KeyCode.UpArrow, keyBoardSaveData) != keycode && GetSetKey(KeyCode.DownArrow, keyBoardSaveData) != keycode
                && GetSetKey(KeyCode.LeftArrow, keyBoardSaveData) != keycode && GetSetKey(KeyCode.RightArrow, keyBoardSaveData) != keycode
                && GetSetKey(KeyCode.X, keyBoardSaveData) != keycode && GetSetKey(KeyCode.Escape, keyBoardSaveData) != keycode)
                {
                    if (nowBtn.name == TextName.儲存並返回)
                    {
                        PlayerSaveData.keyBoardSaveData = keyBoardSaveData;
                        SaveKeyBoardData();
                        Hide();
                        TitleSelect.Instance.Show();
                    }
                    else if (nowBtn.name == TextName.取消)
                    {
                        Hide();
                        TitleSelect.Instance.Show();
                        LoadData();
                    }
                    else
                    {
                        isWaitInput = true;
                        nowBtn.text.color = Color.yellow;
                    }
                    break;
                }
                else if (GetSetKey(KeyCode.X, keyBoardSaveData) == keycode)
                {
                    Hide();
                    TitleSelect.Instance.Show();
                    LoadData();
                    break;
                }
            }
        }
    }

    public void LoadData()
    {
        keyBoardSaveData = new List<KeyBoardSaveData>();
        foreach (var btn in btns)
        {
            if (btn.keyCode == KeyCode.None)
                continue;
            var setKey = PlayerSaveData.keyBoardSaveData.FirstOrDefault(r => r.baseKey == btn.keyCode).setKey;
            btn.text.text = setKey.ToString();
            keyBoardSaveData.Add(new KeyBoardSaveData()
            {
                baseKey = btn.keyCode,
                setKey = setKey,
            });
        }
    }
}
