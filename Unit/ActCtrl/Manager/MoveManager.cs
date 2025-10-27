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
public class MoveManager
{
    public bool isRun = false;
    public uint coreSettingId;
    public Vector2 moveVector;
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public ActionProp actionProp;

    public MoveManager()
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
    }
    public void Reset()
    {
        this.isRun = false;
        this.moveVector = Vector2.zero;
        this.unitProp = null;
        this.actionProp = null;
        this.unitCtrlObj = null;
        this.setting = null;
        this.coreSettingId = 0;
    }
    public void UpdateMove(uint aTime)
    {
        if (setting == null)
            return;

        // 旋轉處理
        if (unitProp.rotateIsMoveAngle)
        {
            unitCtrlObj.SetRotateZ(actionProp.moveAngle);
        }
        else if (setting.addRotateZ != null)
        {
            unitCtrlObj.SetAddRotateZ(setting.addRotateZ);
        }

        if (setting.childAddRotateZ != null)
        {
            unitCtrlObj.SetChildAddRotateZ(setting.childAddRotateZ);
        }

        // 計時位置控制
        var timPosTime = actionProp.timPosTime;
        if (!InvalidHelper.IsInvalid(timPosTime))
        {
            var timPosSpeedPoint = actionProp.timPosSpeedPoint;
            var timPosAddSpeed = actionProp.timPosAddSpeed;
            var timPosSpeed = actionProp.timPosSpeed;

            if (aTime < timPosTime)
            {
                float t = (float)aTime / timPosTime;

                if (t < timPosSpeedPoint)
                    actionProp.timPosSpeed -= timPosAddSpeed;
                else if (t > timPosSpeedPoint)
                    actionProp.timPosSpeed += timPosAddSpeed;

                actionProp.speed = timPosSpeed * 60f;
            }
            else if (aTime == timPosTime)
            {
                actionProp.speed = 0;
                actionProp.timPosAddSpeed = 0;
                actionProp.timPosMoveDis = 0;
                actionProp.timPosSpeed = 0;
                actionProp.timPosTime = 0;
                actionProp.timPosPos = Vector2.zero;
            }
        }
        else
        {

            // 加速度限制處理
            if (!InvalidHelper.IsInvalid(setting.addSpeed))
                actionProp.speed += setting.addSpeed * (1f / 60f);

            if (!InvalidHelper.IsInvalid(setting.maxSpeed) && actionProp.speed > setting.maxSpeed)
                actionProp.speed = setting.maxSpeed;

            if (!InvalidHelper.IsInvalid(setting.minSpeed) && actionProp.speed < setting.minSpeed)
                actionProp.speed = setting.minSpeed;

            // 角度增量
            if (setting.addMoveAngle != null)
            {
                var angle = unitCtrlObj.GetAngle(setting.addMoveAngle, out var isNewAngle);
                if (isNewAngle)
                    actionProp.moveAngle = 0;
                else
                    angle *= 1f / 60f;

                actionProp.moveAngle += angle;
            }
        }

        // 計算移動向量
        float rad = actionProp.moveAngle * Mathf.Deg2Rad;
        float speedPerFrame = actionProp.speed * (1f / 60f);

        moveVector.x = Mathf.Cos(rad) * speedPerFrame;
        moveVector.y = Mathf.Sin(rad) * speedPerFrame;

        unitCtrlObj.MoveTranslate(moveVector);
    }
}

