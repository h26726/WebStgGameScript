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
public class PracticeOverSelect : SelectBase<PracticeOverSelect, TextOption>
{
    protected override void ClickExtraHandle()
    {
        if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
        {
            Hide();
            switch (nowBtn.name)
            {
                case TextName.保存錄像:
                    ReplaySelect.Instance.IsRead = false;
                    ReplaySelect.Instance.Show();
                    ReplaySelect.Instance.AddBack(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.回到標題:
                    LoadingCtrl.Instance.SwitchPage(LoadingCtrl.PageIndex.Title);
                    break;
                case TextName.重新開始:
                    LoadingCtrl.Instance.SwitchPage(LoadingCtrl.PageIndex.Game);
                    break;
                default:
                    break;
            }

        }
    }
}
