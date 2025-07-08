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
public class ReplayPauseSelect : SelectBase<ReplayPauseSelect, TextOption>
{
    protected override void ClickExtraHandle()
    {
        if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
        {
            Hide();
            switch (nowBtn.name)
            {
                case TextName.解除暫停:

                    break;
                case TextName.回到標題:
                    YesNoSelect.Instance.Show();
                    YesNoSelect.Instance.AddNext(() =>
                    {
                        LoadingCtrl.Instance.SwitchPage(LoadingCtrl.PageIndex.Title);
                    });
                    YesNoSelect.Instance.AddBack(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.重新開始:
                    YesNoSelect.Instance.Show();
                    YesNoSelect.Instance.AddNext(() =>
                    {
                        LoadingCtrl.Instance.SwitchPage(LoadingCtrl.PageIndex.Game);
                    });
                    YesNoSelect.Instance.AddBack(() =>
                    {
                        Show();
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
