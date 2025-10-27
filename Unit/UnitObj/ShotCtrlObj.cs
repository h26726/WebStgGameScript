using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static SaveJsonData;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class ShotCtrlObj : UnitCtrlObj
{
    public bool isSkipBirthAni = false;//透過Perfab設定是否跳過出生動畫
    public bool isThrough = false;//透過Perfab設定是否穿透
    public bool CheckBirthAnimExist()
    {
        if (isSkipBirthAni || animator == null || !animator.HasState(0, Animator.StringToHash("Idle")))
        {
            return false;
        }
        return true;
    }
}

