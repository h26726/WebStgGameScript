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
public class DifficultSelect : SelectBase<DifficultSelect,DifficultOption>
{
    protected override void ClickHandle()
    {
        
        if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.Z)))
        {
            Hide();
            GameSelect.difficult = nowBtn.difficult;
            Next();
        }
        else if (Input.GetKeyDown(TransferToPlayerSetKey(KeyCode.X)))
        {
            Hide();
            Back();
        }
    }
}
