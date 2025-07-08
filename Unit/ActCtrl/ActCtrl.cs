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

public class ActCtrl
{
    public IUnitCtrlData unitCtrlData;
    public uint Id { get { return stageSetting.Id; } }
    public Action coreAction;
    public Action callTimeAction;
    public Action callPosAction;
    public Action stopAction;

    public Vector2 moveVector = Vector2.zero;

    public SettingBase stageSetting;
    public ActionProp actionProp;
    public bool isExt;
    public uint aTime = 0;
    public uint nowCallRuleKey = 0;
    public List<CallRule> callRules = new List<CallRule>();

    public ActCtrl Set(IUnitCtrlData unitCtrlData, SettingBase stageSetting)
    {
        this.unitCtrlData = unitCtrlData;
        this.stageSetting = stageSetting;
        this.callRules = CallRuleFactory.CreateCallRules(this); //撈出行動觸發執行資料(所有受該行動ID影響的) 整理判斷時間與判斷位置

        return this;
    }

    public void Active(ActCtrl parentActCtrl)
    {
        unitCtrlData.ClearAllAction(this);

        aTime = 0;
        nowCallRuleKey = 0;

        if (parentActCtrl == null)
            actionProp = new ActionProp(unitCtrlData, stageSetting, 0, 0);
        else
            actionProp = new ActionProp(
                unitCtrlData,
                stageSetting,
                parentActCtrl.actionProp.moveAngle,
                parentActCtrl.actionProp.speed
            );

        moveVector = Vector2.zero;
        coreAction = ActCtrlDelegateFactory.InitDelegateCoreAction(this, () =>
        {
            stopAction = () =>
            {
                unitCtrlData.ClearAllAction(this);
            };
        });
        ActCtrlDelegateFactory.InitDelegateCallTime(this);
        callPosAction = ActCtrlDelegateFactory.InitDelegateCallPos(this);
    }
}

