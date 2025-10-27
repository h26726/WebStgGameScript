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

public class CallTimeManager
{
    public bool isRun;
    public uint nowCallRuleKey;
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public ActionProp actionProp;
    // public bool isRun = false;
    public uint coreSettingId;
    public CallRule[] callRules;
    public uint callRulesUseCount;


    public CallTimeManager()
    {
        Reset();
    }
    public void Set(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.unitProp = actCtrl.unitProp;
        this.actionProp = actCtrl.actionProp;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
        this.callRules = actCtrl.callRules;
        this.callRulesUseCount = actCtrl.callRulesUseCount;
        this.nowCallRuleKey = 0;
    }

    public void Reset()
    {
        this.isRun = false;
        this.unitProp = null;
        this.actionProp = null;
        this.unitCtrlObj = null;
        this.setting = null;
        this.coreSettingId = 0;
        this.callRules = null;
        this.callRulesUseCount = 0;
        this.nowCallRuleKey = 0;

    }
    public void UpdateCall(uint aTime)
    {
        if (callRulesUseCount == 0)
        {
            this.isRun = false;
            return;
        }
        TryRunCall(aTime);
    }

    void TryRunCall(uint aTime)
    {
        if (nowCallRuleKey >= callRulesUseCount)
        {
            isRun = false;
            return;
        }
        var callRule = callRules[(int)nowCallRuleKey];
        // if (setting.Id == 171011)
        // {
        //     Debug.Log(171011);
        //     Debug.Log("nowCallRuleKey:" + nowCallRuleKey);
        //     Debug.Log("callRulesUseCount:" + callRulesUseCount);
        //     Debug.Log(callRule.Print());
        // }



        if (!callRule.callTriggerFlag.HasFlag(CallRuleScheme.CallTriggerFlag.IdTime))
        {
            ContinueTryRunNextCall(aTime);
            return;
        }

        if (callRule.callATime == aTime)
        {
            callRule.Call(unitProp, actionProp);
            ContinueTryRunNextCall(aTime);
        }
    }

    void ContinueTryRunNextCall(uint aTime)
    {
        nowCallRuleKey++;
        TryRunCall(aTime);
    }

}

