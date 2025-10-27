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

public class RestoreManager
{
    public SettingBase setting;
    public UnitPropBase unitProp;
    // public bool isRun = false;
    public uint coreSettingId;
    public bool isRun;



    public RestoreManager()
    {
        Reset();
    }

    public void Set(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.unitProp = actCtrl.unitProp;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
    }

    public void Reset()
    {
        this.isRun = false;
        this.unitProp = null;
        this.setting = null;
        this.coreSettingId = 0;
    }
    public void UpdateTryRestore(uint aTime)
    {
        if (setting == null)
            return;

        if (!InvalidHelper.IsInvalid(setting.restoreTime) && aTime == setting.restoreTime)
        {
            unitProp.isTriggerRestore = true;
            isRun = false;
        }

        if (!InvalidHelper.IsInvalid(setting.deadTime) && aTime == setting.deadTime)
        {
            unitProp.isTriggerDead = true;
            isRun = false;
        }
    }


}

