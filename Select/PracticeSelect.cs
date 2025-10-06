using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static GameConfig;
using System.Collections.Generic;
public class PracticeSelect : SelectBase<PracticeSelect, PracticeOption>
{
    protected override void ClickHandle()
    {

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
