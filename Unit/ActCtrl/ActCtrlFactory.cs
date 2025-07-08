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

public static class ActCtrlFactory
{
    public static Dictionary<uint, ActCtrl> CreateActCtrlDict(UnitCtrlBase unitCtrl)
    {
        var actCtrlDict = new Dictionary<uint, ActCtrl>();
        var coreActCtrl = new ActCtrl().Set(unitCtrl, unitCtrl.coreSetting);
        if (coreActCtrl != null)
        {
            actCtrlDict[coreActCtrl.Id] = coreActCtrl;
        }
        foreach (var setting in unitCtrl.createSetting.actionSettingsDict.Values)
        {

            var actCtrl = new ActCtrl().Set(unitCtrl, setting);
            if (actCtrl != null)
            {
                actCtrlDict[actCtrl.Id] = actCtrl;
            }
            else
            {
                Debug.LogError($"ActCtrlFactory Create ActCtrl Failed for Setting ID: {setting.Id}");
            }
        }
        return actCtrlDict;
    }
}


