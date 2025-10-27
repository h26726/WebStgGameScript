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

public static class UnitPropFactory
{
    public static UnitPropBase Create(UnitCtrlBase unitCtrl)
    {
        if (unitCtrl is EnemyBossUnitCtrl)
        {
            return new EnemyBossUnitProp(unitCtrl);
        }
        else if (unitCtrl is EnemyUnitCtrl)
        {
            return new EnemyUnitProp(unitCtrl);
        }
        else if (unitCtrl is EnemyShotUnitCtrl)
        {
            return new EnemyShotUnitProp(unitCtrl);
        }
        else if (unitCtrl is PlayerUnitCtrl)
        {
            return new PlayerUnitProp(unitCtrl);
        }
        else if (unitCtrl is PlayerShotUnitCtrl)
        {
            return new PlayerShotUnitProp(unitCtrl);
        }
        else if (unitCtrl is PowerUnitCtrl)
        {
            return new PowerUnitProp(unitCtrl);
        }
        Debug.LogError("UnitPropFactory Input Type Not Correct");
        return new EnemyUnitProp(unitCtrl);
    }
}

