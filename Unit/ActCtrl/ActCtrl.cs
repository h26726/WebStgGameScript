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

public class ActCtrl
{
    public ActImmediateMachine actImmediateMachine;
    public WaitBirthAniManager onWaitBirthAniManager;
    public ATimeManager onATimeManager;
    public CallTimeManager onCallTimeManager;
    public CallPosManager onCallPosManager;
    public MoveManager onMoveManager;
    public RestoreManager onRestoreManager;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public uint Id { get { return setting.Id; } }

    public SettingBase setting;
    public ActionProp actionProp;
    public ActionProp parentActionProp;
    public uint coreSettingId = 0;
    public List<CallRule> callRules = new List<CallRule>();
    public bool isRun = false;
    public bool isBoss = false;
    public bool isSetActionMoveAngle = false;

    public ActCtrl Set(UnitCtrlBase unitCtrl, SettingBase setting)
    {
        this.coreSettingId = unitCtrl.coreSetting.Id;
        this.unitCtrlObj = unitCtrl.unitCtrlObj;
        this.unitProp = unitCtrl.unitProp;
        this.setting = setting;
        this.isBoss = unitCtrl == GameBoss.nowUnit;

        return this;
    }

    bool CheckNeedBirthAni()
    {
        if (!(unitCtrlObj is ShotCtrlObj))
            return false;
        var shotCtrlObj = (ShotCtrlObj)unitCtrlObj;
        return coreSettingId == setting.Id && setting.birthDurTime != 0f && shotCtrlObj.CheckBirthAninExist();
    }

    public void UpdateHandler()
    {
        if (onWaitBirthAniManager != null && onWaitBirthAniManager.isRun)
        {
            onWaitBirthAniManager.UpdateFadeIn();
            if (!onWaitBirthAniManager.isRun)
            {
                unitCtrlObj.PlayBirthFinishAni();
                Act4_PlayAni();
            }
            return;
        }

        if (onATimeManager == null)
            return;

        unitCtrlObj.actionTimeText.text += $"{setting.Id}:{onATimeManager.aTime} \n";

        if (!onATimeManager.isRun)
        {
            this.isRun = false;
            return;
        }

        onATimeManager.UpdateAddCount();

        if (onRestoreManager != null && onRestoreManager.isRun)
        {
            onRestoreManager.UpdateTryRestore(onATimeManager.aTime);
        }

        if (onMoveManager != null && onMoveManager.isRun)
        {
            onMoveManager.UpdateMove(onATimeManager.aTime);
        }

        if (onCallTimeManager != null && onCallTimeManager.isRun)
        {
            onCallTimeManager.UpdateCall(onATimeManager.aTime);
        }

        if (onCallPosManager != null && onCallPosManager.isRun)
        {
            onCallPosManager.UpdateCall(onATimeManager.aTime);
        }

        if (onATimeManager.isMoveStop)
        {
            onMoveManager.isRun = false;
        }
        if (isSetActionMoveAngle)
        {
            isSetActionMoveAngle = false;
            ActionMoveAngleDictHandle();
        }
    }

    void ActionMoveAngleDictHandle()
    {
        var moveAngle = CalAngle(unitCtrlObj.beforeTransformPos, unitCtrlObj.transform.position);
        unitCtrlObj.AddActionMoveAngleDict(setting.Id, moveAngle);
        if (setting.addIds != null)
        {
            foreach (var addId in setting.addIds)
            {
                unitCtrlObj.AddActionMoveAngleDict(addId, moveAngle);
            }
        }
    }

    //parentActCtrl 繼承actionProp.moveAngle actionProp.speed
    public void Act1_RunAndReset(ActionProp parentActionProp = null)
    {
        this.isRun = true;
        this.parentActionProp = parentActionProp;
        this.actImmediateMachine = null;
        this.onWaitBirthAniManager = null;
        this.onATimeManager = null;
        this.onMoveManager = null;
        this.onCallTimeManager = null;
        this.onCallPosManager = null;

        this.callRules = CallRuleFactory.CreateCallRules(this); //撈出行動觸發執行資料(所有受該行動ID影響的) 整理判斷時間與判斷位置


        Act2_RunImmediate();
    }

    public void Act2_RunImmediate()
    {
        if (isBoss)
        {
            var enemyUnitProp = unitProp as EnemyUnitProp;
            enemyUnitProp.isBoss = true;
        }
        actImmediateMachine = new ActImmediateMachine(this);
        actImmediateMachine.Run();
        Act3_OnWaitBirth();
    }


    public void Act3_OnWaitBirth()
    {
        if (CheckNeedBirthAni())
        {
            onWaitBirthAniManager = new WaitBirthAniManager(this);
            return;
        }
        Act4_PlayAni();
    }
    public void Act4_PlayAni()
    {
        if (setting.ani != null)
        {
            unitCtrlObj.PlayAniName(setting.ani);
        }
        else if (coreSettingId == setting.Id)
        {
            unitCtrlObj.PlayBirthFinishAni();
        }
        Act5_InitActionProp();
    }

    public void Act5_InitActionProp()
    {
        if (parentActionProp == null)
            actionProp = new ActionProp(unitCtrlObj, setting, 0, 0);
        else
            actionProp = new ActionProp(
                unitCtrlObj,
                setting,
                parentActionProp.moveAngle,
                parentActionProp.speed
            );
        Act6_AllowCollision_OnATimeMoveCall();
    }




    public void Act6_AllowCollision_OnATimeMoveCall()
    {
        unitProp.isAllowCollision = true;
        isSetActionMoveAngle = true;
        onATimeManager = new ATimeManager(this, isBoss);
        onMoveManager = new MoveManager(this);
        onCallTimeManager = new CallTimeManager(this);
        onCallPosManager = new CallPosManager(this);
        onRestoreManager = new RestoreManager(this);

    }
}

