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
public class PracticeSelect : SelectBase<PracticeSelect, PracticeOption>
{
    protected override void ClickHandle()
    {
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.LeftArrow)))
        {
            PressLeftHandle();
        }
        else if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.RightArrow)))
        {
            PressRightHandle();
        }
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)))
        {
            Hide();
            GameSelect.practiceId = nowBtn.practiceId;
            LoadCtrl.Instance.SwitchPage(PageIndex.Game);

        }
        else if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.X)))
        {
            Hide();
            DifficultSelect.Instance.Show();
        }
    }

    protected void PressLeftHandle()
    {
        var newBtnKey = nowBtnKey - GameConfig.PRACTICE_ROW_PER_COUNT;
        if (newBtnKey < 0)
        {
            newBtnKey += GameConfig.PRACTICE_GRID;
        }
        BtnChange(ref nowBtnKey, newBtnKey, true, btns);
    }

    protected void PressRightHandle()
    {
        var newBtnKey = nowBtnKey + GameConfig.PRACTICE_ROW_PER_COUNT;
        if (newBtnKey > GameConfig.PRACTICE_GRID)
        {
            newBtnKey -= GameConfig.PRACTICE_GRID;
        }
        BtnChange(ref nowBtnKey, newBtnKey, true, btns);
    }

    public void UseVersionData()
    {
        foreach (var btn in btns)
        {
            if (LoadCtrl.Instance.selectVersionData.practiceSettings.Any(r => r.Id == btn.practiceId))
            {
                if (btn.text == null)
                {
                    btn.text = btn.animator.gameObject.GetComponent<Text>();
                }
                btn.text.text = LoadCtrl.Instance.selectVersionData.practiceSettings.First(r => r.Id == btn.practiceId).name;
            }
            else
            {
                btn.isHide = true;
            }
        }
        var hides = btns.Where(r => r.isHide).ToList();
        foreach (var hide in hides)
        {
            hide.animator.gameObject.SetActive(false);
        }
    }
}
