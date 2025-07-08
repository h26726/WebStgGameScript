using UnityEngine;
using static CommonData;
using static CommonFunc;
using static GameConfig;
using static PlayerKeyCtrl;
using static PlayerSaveData;
using System;
using System.Linq;
using UnityEngine.UI;
public class ActionProp
{
    protected SettingBase setting = null;
    IUnitCtrlData unitCtrlData = null;
    public float moveAngle = 0;
    public float speed = 0;

    public uint? timPosTime = null;
    public Vector2? timPosPos = null;
    public float? timPosMoveDis = null;
    public float? timPosSpeedPoint = null;
    public float? timPosStartSpeed = null;
    public float? timPosEndSpeed = null;
    public float? timPosAddSpeed = null;
    public float? timPosSpeed = null;


    public ActionProp(IUnitCtrlData unitCtrlData, SettingBase setting, float moveAngleVal, float speed)
    {
        this.setting = setting;
        this.moveAngle = moveAngleVal;
        this.speed = speed;
        this.unitCtrlData = unitCtrlData;
        SetTimPos();
        SetSpeed();
        SetMoveAngle();
        unitCtrlData.AddPrintContent(PrintActionProp());
    }



    public void SetTimPos()
    {
        if (setting.timPosTime != null)
        {
            timPosTime = setting.timPosTime.Value;
            Vector2 pos = unitCtrlData.GetPos(setting.timPosPos);
            timPosPos = pos;
            moveAngle = CalAngle(unitCtrlData.GetTransformPos(), pos);
            timPosMoveDis = Vector2.Distance(unitCtrlData.GetTransformPos(), pos);

            timPosSpeedPoint = 0;
            if (setting.timPosSpeedPoint != null)
                timPosSpeedPoint = setting.timPosSpeedPoint.Value;

            timPosStartSpeed = 0;
            if (setting.timPosStartSpeed != null)
                timPosStartSpeed = setting.timPosStartSpeed.Value;

            timPosEndSpeed = 0;
            if (setting.timPosEndSpeed != null)
                timPosEndSpeed = setting.timPosEndSpeed.Value;

            var speedSum = timPosStartSpeed / 60 * timPosTime * timPosSpeedPoint + timPosEndSpeed / 60 * timPosTime * (1 - timPosSpeedPoint);
            var disSpeed = speedSum - timPosMoveDis;
            var front = (setting.timPosTime * timPosSpeedPoint) - 1f;
            var after = (setting.timPosTime * (1 - timPosSpeedPoint)) - 1f;
            var uFront = (uint)Mathf.Max(0f, front.Value);
            var uAfter = (uint)Mathf.Max(0f, after.Value);
            var LoopSum = CalcArithSum(uFront) + CalcArithSum(uAfter);
            timPosAddSpeed = disSpeed / LoopSum;
            if (timPosSpeedPoint == 0) timPosStartSpeed = timPosEndSpeed - timPosAddSpeed * timPosTime;
            if (timPosSpeedPoint == 1) timPosEndSpeed = timPosStartSpeed + timPosAddSpeed * timPosTime;
            timPosSpeed = timPosStartSpeed;
        }
    }
    public void SetSpeed()
    {
        if (setting.speed != null)
        {
            speed = setting.speed.Value;
        }
    }

    public void SetMoveAngle()
    {
        if (setting.moveAngle != null)
        {
            moveAngle = unitCtrlData.GetAngle(setting.moveAngle, out _);
        }
    }

    string PrintActionProp()
    {
        string print = $"[AP]ActionProp {Environment.NewLine}";
        print += $"--[AP]MoveAngle:{moveAngle} {Environment.NewLine}";
        print += $"--[AP]Speed:{speed} {Environment.NewLine}";
        if (timPosTime != null)
        {
            print += $"--[AP]TimPosTime:{timPosTime} {Environment.NewLine}";
            print += $"--[AP]TimPosPos:{timPosPos} {Environment.NewLine}";
            print += $"--[AP]TimPosMoveDis:{timPosMoveDis} {Environment.NewLine}";
            print += $"--[AP]TimPosSpeedPoint:{timPosSpeedPoint} {Environment.NewLine}";
            print += $"--[AP]TimPosStartSpeed:{timPosStartSpeed} {Environment.NewLine}";
            print += $"--[AP]TimPosEndSpeed:{timPosEndSpeed} {Environment.NewLine}";
            print += $"--[AP]TimPosAddSpeed:{timPosAddSpeed} {Environment.NewLine}";
            print += $"--[AP]TimPosSpeed:{timPosSpeed} {Environment.NewLine}";
        }
        return print;
    }


}

