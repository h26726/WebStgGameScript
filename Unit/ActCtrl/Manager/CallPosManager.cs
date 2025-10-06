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

public class CallPosManager
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public ActionProp actionProp;
    public uint coreSettingId;
    public List<CallRule> callRulePoses;
    public bool isRun;



    public CallPosManager(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.unitProp = actCtrl.unitProp;
        this.actionProp = actCtrl.actionProp;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
        this.callRulePoses = actCtrl.callRules?.Where(r => r.callPosVector != null).ToList();
    }
    public void UpdateCall(uint aTime)
    {
        isRun = false;
        if (callRulePoses == null || callRulePoses.Count == 0)
            return;

        var targetPos = unitCtrlObj.transform.position;
        foreach (var callRule in callRulePoses)
        {
            if (callRule.isDelete)
                continue;

            isRun = true;
            TryRefreshCallPos(callRule);
            bool isTriggerCall = TryCall(callRule, targetPos, out var currentDis);
            if (isTriggerCall && currentDis < 0.1f)
            {
                unitCtrlObj.MovePos(callRule.callPosVector.Value);
            }
        }
    }

    void TryRefreshCallPos(CallRule callRule)
    {
        if (callRule.callPosIsActive == true)
        {
            callRule.callPosVector = unitCtrlObj.GetPos(callRule.originCallPos);
        }
    }

    bool TryCall(CallRule callRule, Vector2 targetPos, out float currentDis)
    {
        currentDis = Vector2.Distance(callRule.callPosVector.Value, targetPos);
        var callDis = callRule.callPosDis != null ? callRule.callPosDis.Value : 0.05f;
        if (currentDis < callDis)
        {
            callRule.isDelete = true;
            callRule.Call(unitProp,actionProp);
            return true;
        }
        return false;
    }
}

