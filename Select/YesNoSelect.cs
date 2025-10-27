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
public class YesNoSelect : SelectBase<YesNoSelect, TextOption>
{
    protected override void ClickHandle()
    {
        
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)))
        {
            Hide();
            switch (nowBtn.name)
            {
                case TextName.是:
                    Next();
                    break;
                case TextName.否:
                    Back();
                    break;
                default:
                    break;
            }

        }
    }
}
