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

public class CallRule
{
    public CallRuleScheme.CallTriggerFlag callTriggerFlag;
    public CallRuleScheme.CallTargetFlag callTargetFlag;
    public bool callPlayerDead;
    public List<Pos> callPos; //持續追蹤位置用
    public bool callPosIsActive;
    public float callPosDis;

    public CreateStageSetting createStageSetting; //創造單位
    public uint coreId; //激活行動
    public uint actId; //激活行動
    public Vector2 callPosVector; //單次定位用
    public uint callATime; //比對action atime用

    public bool isActive;



    public void Reset()
    {
        callPlayerDead = false;
        callPos = null;
        callPosIsActive = false;
        callPosDis = GameConfig.FLOAT_INVAILD;
        createStageSetting = null;
        coreId = GameConfig.UINT_INVAILD;
        actId = GameConfig.UINT_INVAILD;
        callPosVector = GameConfig.VECTOR2_INVAILD;
        callATime = GameConfig.UINT_INVAILD;
        callTriggerFlag = CallRuleScheme.CallTriggerFlag.None;
        callTargetFlag = CallRuleScheme.CallTargetFlag.None;
        isActive = false;
    }


    public void Set(ActCtrl actCtrl, CallRuleScheme callRuleScheme)
    {
        var unitCtrlObj = actCtrl.unitCtrlObj;
        var callPos = callRuleScheme.callPos;
        var callStartAfTime = callRuleScheme.callStartAfTime;
        var callEndBfTime = callRuleScheme.callEndBfTime;
        var actTime = actCtrl.setting.actTime;

        this.callPlayerDead = callRuleScheme.callPlayerDead;
        this.callPos = callRuleScheme.callPos;
        this.callPosIsActive = callRuleScheme.callPosIsActive;
        this.callPosDis = callRuleScheme.callPosDis;
        this.callTriggerFlag = callRuleScheme.callTriggerFlag;
        this.callTargetFlag = callRuleScheme.callTargetFlag;
        this.createStageSetting = callRuleScheme.createStageSetting;
        this.coreId = callRuleScheme.coreId;
        this.actId = callRuleScheme.actId;
        this.isActive = true;


        if (this.callTriggerFlag.HasFlag(CallRuleScheme.CallTriggerFlag.Pos))
        {
            Set(unitCtrlObj, callPos);
        }
        else if (this.callTriggerFlag.HasFlag(CallRuleScheme.CallTriggerFlag.IdTime))
        {
            Set(callStartAfTime, callEndBfTime, actTime);
        }




    }

    public void Set(UnitCtrlObj unitCtrlObj, List<Pos> callPos)
    {
        callPosVector = unitCtrlObj.GetPos(callPos);
    }

    public void Set(uint callStartAfTime, uint callEndBfTime, uint actTime)
    {
        if (!InvalidHelper.IsInvalid(callStartAfTime))
        {
            callATime = callStartAfTime;
        }
        else if (!InvalidHelper.IsInvalid(callEndBfTime) && !InvalidHelper.IsInvalid(actTime))
        {
            callATime = actTime - callEndBfTime;
        }
        else
        {
            Debug.LogError("CallRule Not Set Time:");
            Debug.LogError(Print());
            callATime = 0;
        }
    }

    public void Call(UnitPropBase unitProp, ActionProp actionProp)
    {
        if (callTargetFlag.HasFlag(CallRuleScheme.CallTargetFlag.ActRun))
        {
            CallActRun(unitProp, actionProp);
        }
        else if ((callTargetFlag.HasFlag(CallRuleScheme.CallTargetFlag.Create)))
        {
            CallCreate(unitProp);
        }
    }

    public void CallActRun(UnitPropBase unitProp, ActionProp actionProp)
    {
        isActive = false;
        unitProp.propLateCallActs.Add((coreId, actId, actionProp));
    }

    public void CallCreate(UnitPropBase unitProp)
    {
        isActive = false;
        unitProp.propLateDebutByCreateSettings.Add(createStageSetting);
    }

    public string Print()
    {
        string str = $"------------{Environment.NewLine}";
        str += $"[CR]  callTriggerFlag= {callTriggerFlag} {Environment.NewLine}";
        str += $"[CR]  callTargetFlag= {callTargetFlag} {Environment.NewLine}";

        if (!InvalidHelper.IsInvalid(coreId))
            str += $"[CR]  baseId= {coreId} , actId= {actId}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(actId))
            str += $"[CR]  actId= {actId}{Environment.NewLine}";

        if (createStageSetting != null)
        {
            str += $"[CR]  createStageSetting {Environment.NewLine}";
            str += createStageSetting.Print();
        }

        if (callPos != null)
            str += $"[CR]  CallPos= {string.Join(" , ", callPos.Select(r => r.Print()))} {Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(callPosDis))
            str += $"[CR]  CallPosDis= {callPosDis}{Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(callATime))
            str += $"[CR]  CallATime= {callATime}{Environment.NewLine}";

        str += $"[CR]  isActive= {isActive}{Environment.NewLine}";
        str += $"[CR]  CallPlayerDead= {callPlayerDead}{Environment.NewLine}";
        str += $"[CR]  CallPosIsActive= {callPosIsActive}{Environment.NewLine}";
        return str;
    }




}

