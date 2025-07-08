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
public class YesNoSelect : SelectBase<YesNoSelect, TextOption>
{
    protected override void ClickExtraHandle()
    {
        
        if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
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
