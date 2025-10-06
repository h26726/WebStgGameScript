using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerShotUnitProp : UnitPropBase
{
    public uint dmg = 0;

    public PlayerShotUnitProp(UnitCtrlBase unitCtrl) : base(unitCtrl)
    {
    }

    public override void Reset()
    {
        base.Reset();
        dmg = 0;
    }

    public override void RefreshVal(SettingBase setting)
    {
        base.RefreshVal(setting);
        if (setting.dmg != null)
        {
            dmg = setting.dmg.Value;
        }
    }


}

