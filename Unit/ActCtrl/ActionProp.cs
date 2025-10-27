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
public class ActionProp
{
    protected SettingBase setting;
    UnitCtrlObj unitCtrlObj;
    public float moveAngle;
    public float speed;

    public uint timPosTime;
    public Vector2 timPosPos;
    public float timPosMoveDis;
    public float timPosSpeedPoint;
    public float timPosStartSpeed;
    public float timPosEndSpeed;
    public float timPosAddSpeed;
    public float timPosSpeed;

    public void Reset()
    {
        setting = null;
        unitCtrlObj = null;
        moveAngle = 0;
        speed = 0;
        timPosTime = GameConfig.UINT_INVAILD;
        timPosPos = GameConfig.VECTOR2_INVAILD;
        timPosMoveDis = GameConfig.FLOAT_INVAILD;
        timPosSpeedPoint = GameConfig.FLOAT_INVAILD;
        timPosStartSpeed = GameConfig.FLOAT_INVAILD;
        timPosEndSpeed = GameConfig.FLOAT_INVAILD;
        timPosAddSpeed = GameConfig.FLOAT_INVAILD;
        timPosSpeed = GameConfig.FLOAT_INVAILD;
    }
    public void Set(UnitCtrlObj unitCtrlObj, SettingBase setting, float moveAngleVal, float speed)
    {
        this.setting = setting;
        this.moveAngle = moveAngleVal;
        this.speed = speed;
        this.unitCtrlObj = unitCtrlObj;
        SetTimPos();
        SetSpeed();
        SetMoveAngle();
        // unitCtrlObj.AddPrintContent(PrintActionProp());
    }



    public void SetTimPos()
    {
        if (!InvalidHelper.IsInvalid(setting.timPosTime))
        {
            timPosTime = setting.timPosTime;
            Vector2 pos = unitCtrlObj.GetPos(setting.timPosPos);
            timPosPos = pos;
            moveAngle = CalAngle(unitCtrlObj.transform.position, pos);

            timPosMoveDis = Vector2.Distance(unitCtrlObj.transform.position, pos);

            timPosSpeedPoint = 0;
            if (!InvalidHelper.IsInvalid(setting.timPosSpeedPoint))
                timPosSpeedPoint = setting.timPosSpeedPoint;

            timPosStartSpeed = 0;
            if (!InvalidHelper.IsInvalid(setting.timPosStartSpeed))
                timPosStartSpeed = setting.timPosStartSpeed;

            timPosEndSpeed = 0;
            if (!InvalidHelper.IsInvalid(setting.timPosEndSpeed))
                timPosEndSpeed = setting.timPosEndSpeed;

            var speedSum = timPosStartSpeed / 60 * timPosTime * timPosSpeedPoint + timPosEndSpeed / 60 * timPosTime * (1 - timPosSpeedPoint);
            var disSpeed = speedSum - timPosMoveDis;
            var front = (setting.timPosTime * timPosSpeedPoint) - 1f;
            var after = (setting.timPosTime * (1 - timPosSpeedPoint)) - 1f;
            var uFront = (uint)Mathf.Max(0f, front);
            var uAfter = (uint)Mathf.Max(0f, after);
            var LoopSum = CalcArithSum(uFront) + CalcArithSum(uAfter);
            timPosAddSpeed = disSpeed / LoopSum;
            if (timPosSpeedPoint == 0) timPosStartSpeed = timPosEndSpeed - timPosAddSpeed * timPosTime;
            if (timPosSpeedPoint == 1) timPosEndSpeed = timPosStartSpeed + timPosAddSpeed * timPosTime;
            timPosSpeed = timPosStartSpeed;
            // Debug.Log(PrintActionProp());
        }
    }
    public void SetSpeed()
    {
        if (!InvalidHelper.IsInvalid(setting.speed))
        {
            speed = setting.speed;
        }
    }

    public void SetMoveAngle()
    {
        if (setting.moveAngle != null)
        {
            moveAngle = unitCtrlObj.GetAngle(setting.moveAngle, out _);
        }
    }

    string PrintActionProp()
    {
        string print = $"[AP]ActionProp {Environment.NewLine}";
        print += $"--[AP]MoveAngle:{moveAngle} {Environment.NewLine}";
        print += $"--[AP]Speed:{speed} {Environment.NewLine}";
        if (!InvalidHelper.IsInvalid(timPosTime))
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

