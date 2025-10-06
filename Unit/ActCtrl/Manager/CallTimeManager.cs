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

public class CallTimeManager
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public ActionProp actionProp;
    // public bool isRun = false;
    public uint coreSettingId;
    public List<CallRule> callRules;
    public uint nowCallRuleKey;
    public bool isRun;



    public CallTimeManager(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.unitProp = actCtrl.unitProp;
        this.actionProp = actCtrl.actionProp;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
        this.callRules = actCtrl.callRules;
        this.nowCallRuleKey = 0;
    }
    public void UpdateCall(uint aTime)
    {
        if (callRules == null || callRules.Count == 0)
        {
            this.isRun = false;
            return;
        }
        TryRunCall(aTime);
    }

    void TryRunCall(uint aTime)
    {
        if (nowCallRuleKey >= callRules.Count)
        {
            isRun = false;
            return;
        }
        var callRule = callRules[(int)nowCallRuleKey];
        if (callRule.callATime == null)
        {
            ContinueTryRunNextCall(aTime);
        }

        if (callRule.callATime == aTime)
        {
            callRule.Call(unitProp,actionProp);
            ContinueTryRunNextCall(aTime);
        }
    }

    void ContinueTryRunNextCall(uint aTime)
    {
        nowCallRuleKey++;
        TryRunCall(aTime);
    }

}

