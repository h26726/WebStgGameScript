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

public class ActCtrl
{
    public ActImmediateMachine actImmediateMachine;
    public WaitBirthAniManager onWaitBirthAniManager;
    public ATimeManager onATimeManager;
    public CallTimeManager onCallTimeManager;
    public CallPosManager onCallPosManager;
    public MoveManager onMoveManager;
    public RestoreManager onRestoreManager;
    public CallRuleCollection callRuleCollection;
    public UnitCtrlObj unitCtrlObj;
    public UnitPropBase unitProp;
    public uint Id { get { return setting.Id; } }

    public SettingBase setting;
    public ActionProp actionProp;
    public ActionProp parentActionProp;
    public uint coreSettingId;
    public CallRule[] callRules
    {
        get
        {
            return callRuleCollection.callRules;
        }
    }
    public uint callRulesUseCount
    {
        get
        {
            return callRuleCollection.useCount;
        }
    }
    public bool isRun;
    public bool isBoss;
    public bool isSetActionMoveAngle;

    public ActCtrl()
    {
        actionProp = new ActionProp();
        callRuleCollection = new CallRuleCollection();
        actImmediateMachine = new ActImmediateMachine();
        onWaitBirthAniManager = new WaitBirthAniManager();
        onATimeManager = new ATimeManager();
        onCallTimeManager = new CallTimeManager();
        onCallPosManager = new CallPosManager();
        onMoveManager = new MoveManager();
        onRestoreManager = new RestoreManager();
        Reset();
    }

    public void Reset()
    {
        coreSettingId = 0;
        unitCtrlObj = null;
        unitProp = null;
        setting = null;
        isBoss = false;
        actionProp.Reset();
        ResetCtrl();
    }
    public void ResetCtrl()
    {
        isRun = false;
        isSetActionMoveAngle = false;
        parentActionProp = null;
        // callRuleCollection = new CallRuleCollection();
        // actImmediateMachine = new ActImmediateMachine();
        // onWaitBirthAniManager = new WaitBirthAniManager();
        // onATimeManager = new ATimeManager();
        // onCallTimeManager = new CallTimeManager();
        // onCallPosManager = new CallPosManager();
        // onMoveManager = new MoveManager();
        // onRestoreManager = new RestoreManager();
        callRuleCollection.Reset();
        actImmediateMachine.Reset();
        onWaitBirthAniManager.Reset();
        onATimeManager.Reset();
        onCallTimeManager.Reset();
        onCallPosManager.Reset();
        onMoveManager.Reset();
        onRestoreManager.Reset();
    }


    public void Set(UnitCtrlBase unitCtrl, SettingBase setting)
    {
        this.coreSettingId = unitCtrl.coreSetting.Id;
        this.unitCtrlObj = unitCtrl.unitCtrlObj;
        this.unitProp = unitCtrl.unitProp;
        this.setting = setting;
        this.isBoss = unitCtrl == GameBoss.nowUnit;
        if (this.isBoss)
        {
            var enemyBossUnitProp = unitProp as EnemyBossUnitProp;
            enemyBossUnitProp.SetDef();
        }
    }

    bool CheckNeedBirthAni()
    {
        if (!(unitCtrlObj is ShotCtrlObj))
            return false;
        var shotCtrlObj = (ShotCtrlObj)unitCtrlObj;
        return coreSettingId == setting.Id && setting.birthDurTime != 0f && shotCtrlObj.CheckBirthAnimExist();
    }

    public void UpdateHandler()
    {
        if (onWaitBirthAniManager.isRun)
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
            ResetCtrl();
            return;
        }

        onATimeManager.UpdateAddCount();

        if (onRestoreManager.isRun)
        {
            onRestoreManager.UpdateTryRestore(onATimeManager.aTime);
        }

        if (onMoveManager.isRun)
        {
            onMoveManager.UpdateMove(onATimeManager.aTime);
        }

        if (onCallTimeManager.isRun)
        {
            onCallTimeManager.UpdateCall(onATimeManager.aTime);
        }

        if (onCallPosManager.isRun)
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
        ResetCtrl();
        this.isRun = true;
        this.parentActionProp = parentActionProp;

        this.callRuleCollection.Set(this); //撈出行動觸發執行資料(所有受該行動ID影響的) 整理判斷時間與判斷位置


        Act2_RunImmediate();
    }

    public void Act2_RunImmediate()
    {
        actImmediateMachine.Set(this); ;
        actImmediateMachine.Run();
        Act3_OnWaitBirth();
    }


    public void Act3_OnWaitBirth()
    {
        if (CheckNeedBirthAni())
        {
            onWaitBirthAniManager.SetAndPlay(this);
            return;
        }
        Act4_PlayAni();
    }
    public void Act4_PlayAni()
    {
        if (!InvalidHelper.IsInvalid(setting.ani))
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
            actionProp.Set(unitCtrlObj, setting, 0, 0);
        else
            actionProp.Set(
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
        onATimeManager.Set(this, isBoss);
        onMoveManager.Set(this);
        onCallTimeManager.Set(this);
        onCallPosManager.Set(this);
        onRestoreManager.Set(this);

    }
}

