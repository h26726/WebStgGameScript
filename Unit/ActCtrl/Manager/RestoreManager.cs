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

public class RestoreManager
{
    public SettingBase setting;
    public UnitPropBase unitProp;
    // public bool isRun = false;
    public uint coreSettingId;
    public bool isRun;



    public RestoreManager(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.unitProp = actCtrl.unitProp;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
    }
    public void UpdateTryRestore(uint aTime)
    {
        if (setting == null)
            return;

        if (setting.restoreTime != null && aTime == setting.restoreTime.Value)
        {
            unitProp.isTriggerRestore = true;
            isRun = false;
        }

        if (setting.deadTime != null && aTime == setting.deadTime.Value)
        {
            unitProp.isTriggerDead = true;
            isRun = false;
        }
    }


}

