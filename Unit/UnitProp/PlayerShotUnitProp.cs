using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerShotUnitProp : UnitPropBase
{
    public uint dmg = 0;

    protected override void UseSettingValCustomize(SettingBase setting)
    {
        if (setting.dmg != null)
        {
            dmg = setting.dmg.Value;
        }
    }


}

