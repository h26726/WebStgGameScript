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
public class PracticeSelect : SelectBase<PracticeSelect, PracticeOption>
{
    protected override void ClickExtraHandle()
    {
        
        if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
        {
            Hide();
            GameSystem.Instance.selectPracticeId = nowBtn.practiceId;
            LoadingCtrl.Instance.SwitchPage(LoadingCtrl.PageIndex.Game);

        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.X)))
        {
            Hide();
            DifficultSelect.Instance.Show();
        }
    }

    public void LoadData()
    {
        foreach (var btn in btns)
        {
            if (LoadingCtrl.Instance.practiceSettings.ContainsKey(btn.practiceId))
            {
                if (btn.text == null)
                {
                    btn.text = btn.animator.gameObject.GetComponent<Text>();
                }
                btn.text.text = LoadingCtrl.Instance.practiceSettings[btn.practiceId].name;
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
