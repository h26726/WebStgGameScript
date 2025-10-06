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

public class CallRule
{
    public bool isDelete = false;
    public bool? callPlayerDead = null;
    public List<Pos> originCallPos = null; //持續追蹤位置用
    public bool? callPosIsActive = null;
    public Vector2? callPosVector = null; //單次定位用
    public float? callPosDis = null;
    public uint? callATime = null; //比對action atime用
    public CreateStageSetting createStageSetting = null; //創造單位
    public uint? coreId = null; //激活行動
    public uint? actId = null; //激活行動
    public CallRule(ActCtrl actCtrl, CallRuleScheme callParam)
    {
        isDelete = false;
        if (callParam.callPos != null && callParam.callPos.Count > 0)
        {
            originCallPos = callParam.callPos;
            callPosVector = actCtrl.unitCtrlObj.GetPos(callParam.callPos);
            callPosDis = callParam.callPosDis;
            callPosIsActive = callParam.callPosIsActive;
        }
        else if (callParam.callPlayerDead == true)
        {
            callPlayerDead = true;
        }
        else if (callParam.callStartAfTime != null || callParam.callEndBfTime != null)
        {
            if (callParam.callStartAfTime == null && (actCtrl.setting.actTime == null || actCtrl.setting.actTime == 0))
            {
                callATime = 0;
            }
            else if (callParam.callStartAfTime != null)
            {
                callATime = callParam.callStartAfTime.Value;
            }
            else if (callParam.callEndBfTime != null)
            {
                callATime = actCtrl.setting.actTime.Value - callParam.callEndBfTime.Value;
            }

        }

        if (callParam.createStageSetting != null)
        {
            createStageSetting = callParam.createStageSetting;
        }
        else
        {
            coreId = callParam.coreId;
            actId = callParam.actId;
        }
    }

    public string Print()
    {
        string print = $"[CR]CallRule: {Environment.NewLine}";
        if (callPlayerDead != null)
        {
            print += $"--[CR]CallPlayerDead: {callPlayerDead} {Environment.NewLine}";
        }
        if (originCallPos != null)
        {
            print += $"--[CR]OriginCallPos: {string.Join(",", originCallPos.Select(p => p.ToString()))} {Environment.NewLine}";
        }
        if (callPosVector != null)
        {
            print += $"--[CR]CallPosVector: {callPosVector} {Environment.NewLine}";
        }
        if (callPosDis != null)
        {
            print += $"--[CR]CallPosDis: {callPosDis} {Environment.NewLine}";
        }
        if (callATime != null)
        {
            print += $"--[CR]CallATime: {callATime} {Environment.NewLine}";
        }
        if (createStageSetting != null)
        {
            print += $"--[CR]CreateStageSetting: {createStageSetting.Id} {Environment.NewLine}";
        }
        else
        {
            print += $"--[CR]baseId: {coreId} actId: {actId} {Environment.NewLine}";
        }
        return print;
    }

    public void Call(UnitPropBase unitProp, ActionProp actionProp)
    {
        if (createStageSetting != null)
        {
            //創造子單位
            unitProp.propWaitDebutByCreateSettings.Add(createStageSetting);
        }
        else if (coreId != null && actId != null)
        {
            unitProp.propWaitCallActs.Add((coreId.Value, actId.Value, actionProp));
            //啟動其他單位行動
            // List<UnitCtrlBase> Units;
            // if (callRule.baseId == actCtrl.coreSettingId)
            // {
            //     Units = new List<UnitCtrlBase>() { this };
            // }
            // else
            // {
            //     Units = GetUnitsById(callRule.baseId.Value);
            // }

            // GameSceneValCtrl.Instance.waitCallActs += () =>
            // {
            //     GameSceneValCtrl.Instance.ExtAct(Units, callRule.actId.Value, actCtrl);
            // };
        }
    }
}

