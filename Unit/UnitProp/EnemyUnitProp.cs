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

public class EnemyUnitProp : UnitPropBase
{
    public uint hp = DEFAULT_ENEMY_HP;

    protected override void SetCustomize()
    {
        
    }

    protected override void UseSettingValCustomize(SettingBase setting)
    {
        if (setting.hp != null)
        {
            hp = setting.hp.Value;
        }
    }
}

