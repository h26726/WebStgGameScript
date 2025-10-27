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

public class CallPosManager
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public ActionProp actionProp;
    public uint coreSettingId;
    public CallRule[] callRules;
    public uint callRulesUseCount;
    public bool isRun;

    public CallPosManager()
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
    }
    public void UpdateCall(uint aTime)
    {
        isRun = false;
        if (callRulesUseCount == 0)
            return;

        var targetPos = unitCtrlObj.transform.position;
        for (int i = 0; i < callRulesUseCount; i++)
        {
            var callRule = callRules[i];
            if (!callRule.isActive || !callRule.callTriggerFlag.HasFlag(CallRuleScheme.CallTriggerFlag.Pos))
                continue;

            isRun = true;
            TryRefreshCallPos(callRule);
            bool isTriggerCall = TryCall(callRule, targetPos, out var currentDis);
            if (isTriggerCall && currentDis < 0.1f)
            {
                unitCtrlObj.MovePos(callRule.callPosVector);
            }
        }
    }

    void TryRefreshCallPos(CallRule callRule)
    {
        if (callRule.callPosIsActive == true)
        {
            callRule.callPosVector = unitCtrlObj.GetPos(callRule.callPos);
        }
    }

    bool TryCall(CallRule callRule, Vector2 targetPos, out float currentDis)
    {
        currentDis = Vector2.Distance(callRule.callPosVector, targetPos);
        var callDis = !InvalidHelper.IsInvalid(callRule.callPosDis) ? callRule.callPosDis : 0.05f;
        if (currentDis < callDis)
        {
            callRule.Call(unitProp, actionProp);
            return true;
        }
        return false;
    }
}

