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

public static class UnitPropFactory
{
    public static UnitPropBase Create(UnitCtrlBase unit)
    {
        if (unit is EnemyBossUnitCtrl)
        {
            return new EnemyBossUnitProp().Set();
        }
        else if (unit is EnemyUnitCtrl)
        {
            return new EnemyUnitProp().Set();
        }
        else if (unit is EnemyShotUnitCtrl)
        {
            return new EnemyShotUnitProp().Set();
        }
        else if (unit is PlayerUnitCtrl)
        {
            return new PlayerUnitProp().Set();
        }
        else if (unit is PlayerShotUnitCtrl)
        {
            return new PlayerShotUnitProp().Set();
        }
        else if (unit is PowerUnitCtrl)
        {
            return new PowerUnitProp().Set();
        }
        Debug.LogError("UnitPropFactory Input Type Not Correct");
        return new EnemyUnitProp().Set();
    }
}

