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

public static class UnitCtrlFactory
{
    public static UnitCtrlBase InitSelfAndCollision(UnitCtrlObj obj)
    {
        var unitCtrl = Init(obj);
        var collision = obj.mainObjTransform.GetComponent<CollisionCtrlBase>();
        collision.Init(unitCtrl);
        return unitCtrl;
    }

    static UnitCtrlBase Init(UnitCtrlObj obj)
    {
        if (obj is EnemyCtrlObj)
        {
            return new EnemyUnitCtrl(obj);
        }
        else if (obj is EnemyShotCtrlObj)
        {
            return new EnemyShotUnitCtrl(obj);
        }
        else if (obj is PlayerCtrlObj)
        {
            return new PlayerUnitCtrl(obj);
        }
        else if (obj is PlayerShotCtrlObj)
        {
            return new PlayerShotUnitCtrl(obj);
        }
        else if (obj is PowerCtrlObj)
        {
            return new PowerUnitCtrl(obj);
        }
        Debug.LogError("UnitCtrlFactory Input Type Not Correct");
        return new EnemyUnitCtrl(obj);
    }
}

