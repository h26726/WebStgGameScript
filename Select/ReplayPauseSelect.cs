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
public class ReplayPauseSelect : SelectBase<ReplayPauseSelect, TextOption>
{
    protected override void ClickHandle()
    {
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)))
        {
            Hide();
            switch (nowBtn.name)
            {
                case TextName.解除暫停:

                    break;
                case TextName.回到標題:
                    YesNoSelect.Instance.Show();
                    YesNoSelect.Instance.SetNextAction(() =>
                    {
                        LoadCtrl.Instance.SwitchPage(PageIndex.Title);
                    });
                    YesNoSelect.Instance.SetBackAction(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.重新開始:
                    YesNoSelect.Instance.Show();
                    YesNoSelect.Instance.SetNextAction(() =>
                    {
                        LoadCtrl.Instance.SwitchPage(PageIndex.Game);
                    });
                    YesNoSelect.Instance.SetBackAction(() =>
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
