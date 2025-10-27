using UnityEngine;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static GameConfig;
using static PlayerKeyHelper;
using static SaveJsonData;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyUnitProp : UnitPropBase
{
    public uint hp = DEFAULT_ENEMY_HP;

    public EnemyUnitProp(UnitCtrlBase unitCtrl) : base(unitCtrl)
    {
    }

    public override void Reset()
    {
        base.Reset();
        hp = DEFAULT_ENEMY_HP;
    }

    

    public override void RefreshVal(SettingBase setting)
    {
        base.RefreshVal(setting);
        if (!InvalidHelper.IsInvalid(setting.hp))
        {
            hp = setting.hp;
        }
    }

    
}

