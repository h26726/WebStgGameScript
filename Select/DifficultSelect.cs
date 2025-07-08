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
public class DifficultSelect : SelectBase<DifficultSelect,DifficultOption>
{
    protected override void ClickExtraHandle()
    {
        
        if (Input.GetKeyDown(GetSetKey(KeyCode.Z)))
        {
            Hide();
            GameSystem.Instance.selectDifficult = nowBtn.difficult;
            Next();
        }
        else if (Input.GetKeyDown(GetSetKey(KeyCode.X)))
        {
            Hide();
            Back();
        }
    }
}
