using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Text;
using System.Linq;
using static EnumData;
using static CreateSettingData;
using static CommonHelper;
using static PlayerKeyHelper;
using static PlayerSaveData;
using static GameConfig;

public class ShotUnitCtrl : UnitCtrlBase
{
    public ShotUnitCtrl(UnitCtrlObj unitCtrlObj) : base(unitCtrlObj)
    {
    }

    public ShotCtrlObj shotCtrlObj
    {
        get => (ShotCtrlObj)unitCtrlObj;
        set => unitCtrlObj = value;
    }

    public void TryCollisionDead()
    {
        if (unitProp.isThrough == true || shotCtrlObj.isThrough == true)
            return; //穿透子彈不回收
        unitProp.isTriggerDead = true;
    }
}
