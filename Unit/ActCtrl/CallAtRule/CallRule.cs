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

public class CallRule
{
    public bool? callPlayerDead = null;
    public List<Pos> originCallPos = null; //持續追蹤位置用
    public bool? callPosIsActive = null;
    public Vector2? callPosVector = null; //單次定位用
    public float? callPosDis = null;
    public uint? callATime = null; //比對action atime用
    public CreateStageSetting createStageSetting = null; //創造單位
    public uint? baseId = null; //激活行動
    public uint? actId = null; //激活行動
    public CallRule(ActCtrl actCtrl, CallRuleScheme callParam)
    {
        if (callParam.callPos != null && callParam.callPos.Count > 0)
        {
            originCallPos = callParam.callPos;
            callPosVector = actCtrl.unitCtrlData.GetPos(callParam.callPos);
            callPosDis = callParam.callPosDis;
            callPosIsActive = callParam.callPosIsActive;
        }
        else if (callParam.callPlayerDead == true)
        {
            callPlayerDead = true;
        }
        else if (callParam.callStartAfTime != null || callParam.callEndBfTime != null)
        {
            if (callParam.callStartAfTime == null && (actCtrl.stageSetting.actTime == null || actCtrl.stageSetting.actTime == 0))
            {
                callATime = 0;
            }
            else if (callParam.callStartAfTime != null)
            {
                callATime = callParam.callStartAfTime.Value;
            }
            else if (callParam.callEndBfTime != null)
            {
                callATime = actCtrl.stageSetting.actTime.Value - callParam.callEndBfTime.Value;
            }

        }

        if (callParam.createStageSetting != null)
        {
            createStageSetting = callParam.createStageSetting;
        }
        else
        {
            baseId = callParam.baseId;
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
            print += $"--[CR]baseId: {baseId} actId: {actId} {Environment.NewLine}";
        }
        return print;
    }
}

