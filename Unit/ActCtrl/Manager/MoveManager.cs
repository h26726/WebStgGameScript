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
public class MoveManager
{
    public SettingBase setting;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public ActionProp actionProp;
    public bool isRun = false;
    public uint coreSettingId;
    public Vector2 moveVector;


    public MoveManager(ActCtrl actCtrl)
    {
        this.isRun = true;
        this.unitProp = actCtrl.unitProp;
        this.actionProp = actCtrl.actionProp;
        this.unitCtrlObj = actCtrl.unitCtrlObj;
        this.setting = actCtrl.setting;
        this.coreSettingId = actCtrl.coreSettingId;
    }
    public void UpdateMove(uint aTime)
    {
        if (setting == null)
            return;

        if (unitProp.rotateIsMoveAngle == true)
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

        if (actionProp.timPosTime != null 
        // actionProp.timPosPos != null &&
        // actionProp.timPosMoveDis != null &&
        // actionProp.timPosSpeedPoint != null &&
        // actionProp.timPosStartSpeed != null &&
        // actionProp.timPosEndSpeed != null &&
        // actionProp.timPosAddSpeed != null
        )
        {

            if (aTime < actionProp.timPosTime.Value)
            {
                if ((float)aTime / (float)actionProp.timPosTime.Value < actionProp.timPosSpeedPoint.Value)
                {
                    actionProp.timPosSpeed -= actionProp.timPosAddSpeed.Value;
                }
                else if ((float)aTime / (float)actionProp.timPosTime.Value > actionProp.timPosSpeedPoint.Value)
                {
                    actionProp.timPosSpeed += actionProp.timPosAddSpeed.Value;
                }
                actionProp.speed = actionProp.timPosSpeed.Value * 60;
            }
            else if (aTime == actionProp.timPosTime.Value)
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
            if (setting.addSpeed != null)
            {
                var addSpeed = setting.addSpeed.Value / 60;
                actionProp.speed += addSpeed;
            }
            if (setting.maxSpeed != null)
            {
                if (actionProp.speed > setting.maxSpeed.Value)
                {
                    actionProp.speed = setting.maxSpeed.Value;
                }
            }
            if (setting.minSpeed != null)
            {
                if (actionProp.speed < setting.minSpeed)
                {
                    actionProp.speed = setting.minSpeed.Value;
                }
            }

            if (setting.addMoveAngle != null)
            {
                var angle = unitCtrlObj.GetAngle(setting.addMoveAngle, out var isNewAngle);
                if (isNewAngle) actionProp.moveAngle = 0;
                else angle = angle / 60;
                actionProp.moveAngle += angle;
            }
        }
        moveVector = new Vector2(Mathf.Cos((actionProp.moveAngle) * Mathf.Deg2Rad) * (actionProp.speed / 60), Mathf.Sin((actionProp.moveAngle) * Mathf.Deg2Rad) * (actionProp.speed / 60));
        unitCtrlObj.MoveTranslate(moveVector);
        unitCtrlObj.AddPrintContent($"moveVector:{moveVector}   {Environment.NewLine}");
    }
}

