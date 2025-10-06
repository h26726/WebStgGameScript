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

public class EnemyUnitCtrl : UnitCtrlBase
{
    public EnemyUnitCtrl(UnitCtrlObj unitCtrlObj) : base(unitCtrlObj)
    {
    }

    public EnemyUnitProp enemyProp
    {
        get => (EnemyUnitProp)unitProp;
        set => unitProp = value;
    }

    public override void Unit3_TriggerDead_OnWaitAni()
    {
        base.Unit3_TriggerDead_OnWaitAni();
        GivePower();
    }
    public void GivePower(uint PowerGiveNum)
    {
        for (int i = 0; i < PowerGiveNum; i++)
        {
            var unitCtrl = GameSelect.powerData.powerCreateStageSettings[i];
            unitProp.propWaitDebutByCreateSettings.Add((unitCtrl));
        }
    }

    public void GivePower()
    {
        if (GameSelect.isPracticeMode)
            return;

        if (createStageSetting == null)
            return;
        if (coreSetting.powerGive == null || coreSetting.powerGive == 0)
            return;

        var PowerGiveNum = coreSetting.powerGive;
        GivePower(PowerGiveNum.Value);
    }
}
