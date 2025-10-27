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
using static SaveJsonData;
using static GameConfig;

public class EnemyBossUnitCtrl : EnemyUnitCtrl
{
    public EnemyBossUnitCtrl(UnitCtrlObj unitCtrlObj) : base(unitCtrlObj)
    {
    }

    public void SetDef()
    {
        var enemyBossUnitProp = unitProp as EnemyBossUnitProp;
        enemyBossUnitProp.SetDef();
    }

    public void SetKeepData((uint debutNo, uint powerGiveNum) keepData)
    {
        this.debutNo = keepData.debutNo;
        GivePower(keepData.powerGiveNum);
    }

}