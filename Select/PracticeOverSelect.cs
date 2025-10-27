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
public class PracticeOverSelect : SelectBase<PracticeOverSelect, TextOption>
{
    protected override void ClickHandle()
    {
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)))
        {
            Hide();
            switch (nowBtn.name)
            {
                case TextName.保存錄像:
                    ReplaySelect.Instance.IsRead = false;
                    ReplaySelect.Instance.Show();
                    ReplaySelect.Instance.SetBackAction(() =>
                    {
                        Show();
                    });
                    break;
                case TextName.回到標題:
                    LoadCtrl.Instance.SwitchPage(PageIndex.Title);
                    break;
                case TextName.重新開始:
                    LoadCtrl.Instance.SwitchPage(PageIndex.Game);
                    break;
                default:
                    break;
            }

        }
    }
}
