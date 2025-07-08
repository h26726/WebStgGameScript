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

public static class ActCtrlDelegateFactory
{
    public static Action InitDelegateCoreAction(ActCtrl actCtrl, Action removeDelegate)
    {
        Action totalAction = null;
        var unitCtrlData = actCtrl.unitCtrlData;

        if (actCtrl.stageSetting == null)
            return totalAction;

        DelegateCountDown();
        var unitProp = unitCtrlData.GetUnitProp();
        DelegateMove();
        DelegateRestoreDeadTime(actCtrl);
        return totalAction;

        void DelegateCountDown()
        {
            var setting = actCtrl.stageSetting;
            Action selfAction = () =>
            {
                actCtrl.aTime++;
                unitCtrlData.AddPrintContent($"actCtrl.aTime:{actCtrl.aTime}   {Environment.NewLine}");
                if (setting.actTime == null || setting.actTime == 0)
                    return;
                if (actCtrl.aTime == setting.actTime)
                {
                    actCtrl.moveVector = Vector2.zero;
                    unitCtrlData.OnActTimeEndCustomize();
                    if (actCtrl.Id != unitCtrlData.GetCoreSettingId())
                    {
                        removeDelegate();
                    }
                }
            };
            totalAction += selfAction;
        }

        void DelegateMove()
        {
            List<Action> actions = new List<Action>();
            var setting = actCtrl.stageSetting;
            var actionProp = actCtrl.actionProp;

            if (unitProp.rotateIsMoveAngle == true)
            {
                actions.Add(() =>
                {
                    unitCtrlData.SetRotateZ(actCtrl.actionProp.moveAngle);
                });
            }
            else if (setting.addRotateZ != null)
            {
                actions.Add(() =>
                {
                    var angle = unitCtrlData.GetAngle(setting.addRotateZ, out var isNewAngle);
                    if (isNewAngle) unitCtrlData.SetRotateZ(angle);
                    else unitCtrlData.SetRotateZ(unitCtrlData.GetRotateZ() + angle / 60);
                });
            }


            if (setting.childAddRotateZ != null)
            {
                actions.Add(() =>
                {
                    var angle = unitCtrlData.GetAngle(setting.childAddRotateZ, out var isNewAngle);
                    if (isNewAngle) unitCtrlData.SetChildRotateZ(angle);
                    else unitCtrlData.SetChildRotateZ(unitCtrlData.GetChildRotateZ() + angle / 60);
                });
            }

            if (actionProp.timPosTime != null &&
            actionProp.timPosPos != null &&
            actionProp.timPosMoveDis != null &&
            actionProp.timPosSpeedPoint != null &&
            actionProp.timPosStartSpeed != null &&
            actionProp.timPosEndSpeed != null &&
            actionProp.timPosAddSpeed != null)
            {
                actions.Add(() =>
                {
                    if (actCtrl.aTime < actionProp.timPosTime.Value)
                    {
                        if ((float)actCtrl.aTime / (float)actionProp.timPosTime.Value < actionProp.timPosSpeedPoint.Value)
                        {
                            actionProp.timPosSpeed -= actionProp.timPosAddSpeed.Value;
                        }
                        else if ((float)actCtrl.aTime / (float)actionProp.timPosTime.Value > actionProp.timPosSpeedPoint.Value)
                        {
                            actionProp.timPosSpeed += actionProp.timPosAddSpeed.Value;
                        }
                        actionProp.speed = actionProp.timPosSpeed.Value * 60;
                    }
                    else if (actCtrl.aTime == actionProp.timPosTime.Value)
                    {
                        actionProp.speed = 0;
                        actionProp.timPosAddSpeed = 0;
                        actionProp.timPosMoveDis = 0;
                        actionProp.timPosSpeed = 0;
                        actionProp.timPosTime = 0;
                        actionProp.timPosPos = Vector2.zero;
                    }
                });
            }
            else
            {
                if (setting.addSpeed != null)
                {
                    var addSpeed = setting.addSpeed.Value / 60;
                    actions.Add(() =>
                    {
                        actionProp.speed += addSpeed;
                    });
                }
                if (setting.maxSpeed != null)
                {
                    actions.Add(() =>
                    {
                        if (actionProp.speed > setting.maxSpeed.Value)
                        {
                            actionProp.speed = setting.maxSpeed.Value;
                        }
                    });
                }
                if (setting.minSpeed != null)
                {
                    actions.Add(() =>
                    {
                        if (actionProp.speed < setting.minSpeed)
                        {
                            actionProp.speed = setting.minSpeed.Value;
                        }
                    });
                }

                if (setting.addMoveAngle != null)
                {
                    actions.Add(() =>
                    {
                        var angle = unitCtrlData.GetAngle(setting.addMoveAngle, out var isNewAngle);
                        if (isNewAngle) actionProp.moveAngle = 0;
                        else angle = angle / 60;
                        actionProp.moveAngle += angle;
                    });
                }
            }

            actions.Add(() =>
            {
                actCtrl.moveVector = new Vector2(Mathf.Cos((actionProp.moveAngle) * Mathf.Deg2Rad) * (actionProp.speed / 60), Mathf.Sin((actionProp.moveAngle) * Mathf.Deg2Rad) * (actionProp.speed / 60));
                unitCtrlData.MoveTranslate(actCtrl.moveVector);
                unitCtrlData.AddPrintContent($"moveVector:{actCtrl.moveVector}   {Environment.NewLine}");
            });
            foreach (var action in actions)
            {
                totalAction += action;
            }
        }

        void DelegateRestoreDeadTime(ActCtrl actCtrl)
        {
            var setting = actCtrl.stageSetting;
            if (setting.restoreTime != null)
                AddTimedAction(setting.restoreTime.Value, () => unitCtrlData.TriggerRestore());
            if (setting.deadTime != null)
                AddTimedAction(setting.deadTime.Value, () => unitCtrlData.TriggerDead());
            void AddTimedAction(uint targetTime, Action onTimeReached)
            {
                if (targetTime > 0)
                {
                    Action selfAction = () =>
                    {
                        if (actCtrl.aTime == targetTime)
                        {
                            onTimeReached.Invoke();
                        }
                    };
                    totalAction += selfAction;
                }
            }
        }

    }
    public static void InitDelegateCallTime(ActCtrl actCtrl)
    {
        var unitCtrlData = actCtrl.unitCtrlData;

        if (actCtrl.callRules != null && actCtrl.callRules.Count != 0)
        {
            DelegateCheckTime();
        }
        actCtrl.unitCtrlData.AddPrintContent($"InitDelegateCallTimeEnd {Environment.NewLine}");
        void DelegateCheckTime()
        {
            actCtrl.unitCtrlData.AddPrintContent($"callRules.Count:{actCtrl.callRules.Count}  {Environment.NewLine}");
            while (actCtrl.nowCallRuleKey < actCtrl.callRules.Count)
            {
                var callRule = actCtrl.callRules[(int)actCtrl.nowCallRuleKey];
                if (callRule.callATime != null)
                {
                    actCtrl.unitCtrlData.AddPrintContent(callRule.Print());
                    if (TryCheckTimeHandle(callRule))
                    {
                        return;
                    }
                    actCtrl.callTimeAction = () =>
                    {
                        actCtrl.unitCtrlData.AddPrintContent($"selfAction actTime:{actCtrl.aTime}{Environment.NewLine}");
                        actCtrl.unitCtrlData.AddPrintContent($"callRule actTime:{callRule.callATime}{Environment.NewLine}");
                        TryCheckTimeHandle(callRule);
                    };

                    actCtrl.unitCtrlData.AddPrintContent($" callRule:{callRule.callATime} {Environment.NewLine}");
                    return;
                }
                else
                {
                    actCtrl.nowCallRuleKey++;
                }
            }
        }

        bool TryCheckTimeHandle(CallRule callRule)
        {
            unitCtrlData.AddPrintContent($"actCtrl.Id:{actCtrl.Id},Key:{actCtrl.nowCallRuleKey} ,aTime:{actCtrl.aTime} ,CallATime:{callRule.callATime} ,create:{callRule.createStageSetting?.Id} ,extAct:{callRule.actId}  {Environment.NewLine}");
            if (actCtrl.aTime == callRule.callATime)
            {
                actCtrl.nowCallRuleKey++;
                ActCtrlDelegateFactory.InitDelegateCallTime(actCtrl);
                actCtrl.unitCtrlData.ExtHandle(callRule, actCtrl);
                return true;
            }
            return false;
        }
    }

    public static Action InitDelegateCallPos(ActCtrl actCtrl)
    {
        Action totalAction = null;
        var unitCtrlData = actCtrl.unitCtrlData;
        if (actCtrl.callRules != null && actCtrl.callRules.Count != 0)
        {
            DelegateCheckPos();
        }
        return totalAction;



        void DelegateCheckPos()
        {
            foreach (var callRule in actCtrl.callRules)
            {
                if (callRule.callPosVector == null) continue;
                Action selfAction = () =>
                {
                    TryHandleCheckPos(callRule);
                };
                totalAction += selfAction;
            }
        }

        bool TryHandleCheckPos(CallRule callRule)
        {
            var targetPos = unitCtrlData.GetTransformPos();
            var dis = 0.05f;
            if (callRule.callPosIsActive == true)
            {
                callRule.callPosVector = unitCtrlData.GetPos(callRule.originCallPos);
            }
            var posDis = Vector2.Distance(callRule.callPosVector.Value, targetPos);

            if (callRule.callPosDis != null)
                dis = callRule.callPosDis.Value;
            if (posDis < dis)
            {
                if (posDis < 0.1f)
                {
                    unitCtrlData.MovePos(callRule.callPosVector.Value);
                }
                actCtrl.callPosAction = null;
                actCtrl.unitCtrlData.ExtHandle(callRule, actCtrl);
                return true;
            }
            return false;
        }
    }
}

